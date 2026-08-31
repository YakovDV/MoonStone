Shader "Hidden/IncrementalMining/Project/TerrainTrackStamp"
{
    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            Name "TrackStamp"

            Cull Off
            ZWrite Off
            ZTest Always
            Blend One One
            BlendOp Add
            ColorMask R

            HLSLPROGRAM

            #pragma target 3.0
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float3 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float4 _TrackWorldMin;
            float4 _TrackWorldInvSize;

            float _SegmentLength;
            float _TrackHalfWidth;
            float _TrackEdgeSoftness;

            Varyings Vert(Attributes input)
            {
                Varyings output;

                float3 positionWS = TransformObjectToWorld(input.positionOS);
                float2 maskUV = (positionWS.xz - _TrackWorldMin.xy) * _TrackWorldInvSize.xy;
                float2 positionNDC = maskUV * 2.0 - 1.0;

                #if UNITY_UV_STARTS_AT_TOP
                    positionNDC.y = -positionNDC.y;
                #endif

                output.positionHCS = float4(positionNDC, 0.0, 1.0);
                output.uv = input.uv;

                return output;
            }

            half4 Frag(Varyings input) : SV_Target
            {
                float trackWidth = _TrackHalfWidth * 2.0;
                float totalLength = _SegmentLength + trackWidth;

                float2 localPosition;
                localPosition.x = (input.uv.x - 0.5) * trackWidth;
                localPosition.y = (input.uv.y - 0.5) * totalLength;

                float distancePastEnd = max(abs(localPosition.y) - _SegmentLength * 0.5, 0.0);
                float distanceToSegment = length(float2(localPosition.x, distancePastEnd));

                half mask = 1.0h - smoothstep(
                    _TrackHalfWidth - _TrackEdgeSoftness,
                    _TrackHalfWidth,
                    distanceToSegment
                );

                return half4(mask, 0.0h, 0.0h, 1.0h);
            }

            ENDHLSL
        }
    }
}