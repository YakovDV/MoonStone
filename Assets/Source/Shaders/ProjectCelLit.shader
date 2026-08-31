Shader "IncrementalMining/Project/CelLit"
{
    Properties
    {
        [MainTexture] _BaseMap("Base Map", 2D) = "white" {}
        [MainColor] _BaseColor("Base Color", Color) = (1, 1, 1, 1)

        _ShadowColor("Shadow Tint", Color) = (0.4, 0.45, 0.55, 1)
        _ShadowThreshold("Shadow Threshold", Range(0, 1)) = 0.45
        _ShadowSoftness("Shadow Softness", Range(0.001, 0.25)) = 0.03
        _IndirectStrength("Indirect Strength", Range(0, 2)) = 0.3
        _AdditionalLightsStrength("Additional Lights Strength", Range(0, 2)) = 1

        _SpecularColor("Specular Color", Color) = (1, 1, 1, 1)
        _SpecularSize("Specular Size", Range(0.001, 1)) = 0.15
        _SpecularSoftness("Specular Softness", Range(0.001, 0.25)) = 0.02
        _SpecularIntensity("Specular Intensity", Range(0, 4)) = 0.5

        _BandAntiAliasing("Band Anti-Aliasing", Range(0, 4)) = 1
        _RealtimeShadowThreshold("Realtime Shadow Threshold", Range(0, 1)) = 0.5
        _RealtimeShadowSoftness("Realtime Shadow Softness", Range(0.001, 0.25)) = 0.04

        [Toggle(_NORMALMAP_ON)] _UseNormalMap("Use Normal Map", Float) = 0
        [Normal][NoScaleOffset] _NormalMap("Normal Map", 2D) = "bump" {}
        _NormalStrength("Normal Strength", Range(0, 2)) = 1

        [Toggle(_EMISSION_ON)] _UseEmission("Use Emission", Float) = 0
        [NoScaleOffset] _EmissionMap("Emission Mask", 2D) = "white" {}
        [HDR] _EmissionColor("Emission Color", Color) = (0, 0, 0, 1)

        _EmissionPulseSpeed("Emission Pulse Speed", Range(0, 5)) = 0.7
        _EmissionPulseAmount("Emission Pulse Amount", Range(0, 1)) = 0.2
        _EmissionPulsePhase("Emission Pulse Phase", Range(0, 1)) = 0
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Opaque"
            "Queue" = "Geometry"
        }

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            Cull Back
            ZWrite On

            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment Frag

            #pragma shader_feature_local _NORMALMAP_ON
            #pragma shader_feature_local _EMISSION_ON

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #pragma multi_compile _ _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fog

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float3 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;

                #if defined(_NORMALMAP_ON)
                    half4 tangentOS : TANGENT;
                #endif
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                half3 normalWS : TEXCOORD1;
                float3 positionWS : TEXCOORD2;
                half2 fogAndPulse : TEXCOORD5;

                #if defined(_NORMALMAP_ON)
                    half3 tangentWS : TEXCOORD3;
                    half3 bitangentWS : TEXCOORD4;
                #endif
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            TEXTURE2D(_NormalMap);
            SAMPLER(sampler_NormalMap);

            TEXTURE2D(_EmissionMap);
            SAMPLER(sampler_EmissionMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                half4 _BaseColor;
                half4 _ShadowColor;
                half4 _SpecularColor;
                half4 _EmissionColor;
                half _ShadowThreshold;
                half _ShadowSoftness;
                half _IndirectStrength;
                half _AdditionalLightsStrength;
                half _SpecularSize;
                half _SpecularSoftness;
                half _SpecularIntensity;
                half _NormalStrength;
                half _UseNormalMap;
                half _UseEmission;
                half _BandAntiAliasing;
                half _RealtimeShadowThreshold;
                half _RealtimeShadowSoftness;
                half _EmissionPulseSpeed;
                half _EmissionPulseAmount;
                half _EmissionPulsePhase;
            CBUFFER_END

            half CalculateBand(float value, half threshold, half softness)
            {
                float transitionWidth = max((float)softness, fwidth(value) * _BandAntiAliasing);
                return smoothstep(threshold - transitionWidth, threshold + transitionWidth, value);
            }

            half CalculateLightBand(float NdotL)
            {
                return CalculateBand(NdotL, _ShadowThreshold, _ShadowSoftness);
            }

            half CalculateRealtimeShadowBand(float shadowAttenuation)
            {
                return CalculateBand(shadowAttenuation, _RealtimeShadowThreshold, _RealtimeShadowSoftness);
            }

            half CalculateSpecularBand(half3 normalWS, half3 lightDirectionWS, half3 viewDirectionWS)
            {
                half3 halfDirectionWS = SafeNormalize(lightDirectionWS + viewDirectionWS);
                float NdotH = saturate(dot(normalWS, halfDirectionWS));
                float threshold = 1.0 - _SpecularSize;
                float transitionWidth = max((float)_SpecularSoftness, fwidth(NdotH) * _BandAntiAliasing);

                return smoothstep(threshold - transitionWidth, threshold + transitionWidth, NdotH) * _SpecularIntensity;
            }

            Varyings Vert(Attributes input)
            {
                Varyings output;

                VertexPositionInputs positionInputs = GetVertexPositionInputs(input.positionOS);

                output.positionHCS = positionInputs.positionCS;
                output.positionWS = positionInputs.positionWS;
                output.uv = TRANSFORM_TEX(input.uv, _BaseMap);
                output.fogAndPulse = half2(ComputeFogFactor(positionInputs.positionCS.z), 1.0h);

                #if defined(_EMISSION_ON)
                    float pulseAngle = (_Time.y * _EmissionPulseSpeed + _EmissionPulsePhase) * 6.2831853;
                    output.fogAndPulse.y = 1.0h + sin(pulseAngle) * _EmissionPulseAmount;
                #endif

                #if defined(_NORMALMAP_ON)
                    VertexNormalInputs normalInputs = GetVertexNormalInputs(input.normalOS, input.tangentOS);

                    output.normalWS = normalInputs.normalWS;
                    output.tangentWS = normalInputs.tangentWS;
                    output.bitangentWS = normalInputs.bitangentWS;
                #else
                    output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                #endif

                return output;
            }

            half4 Frag(Varyings input) : SV_Target
            {
                half3 geometricNormalWS = normalize(input.normalWS);
                half3 normalWS = geometricNormalWS;

                #if defined(_NORMALMAP_ON)
                    half3 tangentWS = normalize(input.tangentWS);
                    half3 bitangentWS = normalize(input.bitangentWS);

                    half4 normalSample = SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, input.uv);
                    half3 normalTS = UnpackNormalScale(normalSample, _NormalStrength);

                    normalWS = normalize(tangentWS * normalTS.x + bitangentWS * normalTS.y + geometricNormalWS * normalTS.z);
                #endif

                half4 albedo = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv) * _BaseColor;
                half3 viewDirectionWS = GetWorldSpaceNormalizeViewDir(input.positionWS);

                float4 shadowCoord = TransformWorldToShadowCoord(input.positionWS);
                Light mainLight = GetMainLight(shadowCoord);

                half NdotL = saturate(dot(normalWS, mainLight.direction));
                half surfaceBand = CalculateLightBand(NdotL);
                half realtimeShadowBand = CalculateRealtimeShadowBand(mainLight.shadowAttenuation);
                half mainLightBand = surfaceBand * realtimeShadowBand;

                half3 ambientLighting = SampleSH(normalWS) * _IndirectStrength;
                half3 diffuseLighting = lerp(_ShadowColor.rgb, mainLight.color, mainLightBand);
                half3 finalColor = albedo.rgb * (diffuseLighting + ambientLighting);

                half mainSpecular = CalculateSpecularBand(normalWS, mainLight.direction, viewDirectionWS);
                mainSpecular *= mainLight.distanceAttenuation * realtimeShadowBand * step(0.0001h, NdotL);

                finalColor += _SpecularColor.rgb * mainLight.color * mainSpecular;

                #if defined(_ADDITIONAL_LIGHTS)
                    uint lightsCount = GetAdditionalLightsCount();

                    for (uint lightIndex = 0; lightIndex < lightsCount; ++lightIndex)
                    {
                        Light light = GetAdditionalLight(lightIndex, input.positionWS);

                        half additionalNdotL = saturate(dot(normalWS, light.direction));
                        half surfaceBand = CalculateLightBand(additionalNdotL);
                        half realtimeShadowBand = CalculateRealtimeShadowBand(light.shadowAttenuation);
                        half lightBand = surfaceBand * realtimeShadowBand;

                        half diffuseAttenuation = light.distanceAttenuation * _AdditionalLightsStrength;
                        finalColor += albedo.rgb * light.color * lightBand * diffuseAttenuation;

                        half specular = CalculateSpecularBand(normalWS, light.direction, viewDirectionWS);
                        specular *= light.distanceAttenuation * realtimeShadowBand * step(0.0001h, additionalNdotL);

                        finalColor += _SpecularColor.rgb * light.color * specular * _AdditionalLightsStrength;
                    }
                #endif

                #if defined(_EMISSION_ON)
                    half emissionMask = SAMPLE_TEXTURE2D(_EmissionMap, sampler_EmissionMap, input.uv).r;
                    finalColor += _EmissionColor.rgb * emissionMask * input.fogAndPulse.y;
                #endif

                finalColor = MixFog(finalColor, input.fogAndPulse.x);

                return half4(finalColor, albedo.a);
            }

            ENDHLSL
        }

        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }

            ZWrite On
            ZTest LEqual
            ColorMask 0
            Cull Back

            HLSLPROGRAM

            #pragma vertex ShadowVert
            #pragma fragment ShadowFrag
            #pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"

            struct ShadowAttributes
            {
                float3 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct ShadowVaryings
            {
                float4 positionHCS : SV_POSITION;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                half4 _BaseColor;
                half4 _ShadowColor;
                half4 _SpecularColor;
                half4 _EmissionColor;
                half _ShadowThreshold;
                half _ShadowSoftness;
                half _IndirectStrength;
                half _AdditionalLightsStrength;
                half _SpecularSize;
                half _SpecularSoftness;
                half _SpecularIntensity;
                half _NormalStrength;
                half _UseNormalMap;
                half _UseEmission;
                half _BandAntiAliasing;
                half _RealtimeShadowThreshold;
                half _RealtimeShadowSoftness;
                half _EmissionPulseSpeed;
                half _EmissionPulseAmount;
                half _EmissionPulsePhase;
            CBUFFER_END

            float3 _LightDirection;
            float3 _LightPosition;

            ShadowVaryings ShadowVert(ShadowAttributes input)
            {
                ShadowVaryings output;

                float3 positionWS = TransformObjectToWorld(input.positionOS);
                float3 normalWS = TransformObjectToWorldNormal(input.normalOS);

                #if defined(_CASTING_PUNCTUAL_LIGHT_SHADOW)
                    float3 lightDirectionWS = normalize(_LightPosition - positionWS);
                #else
                    float3 lightDirectionWS = _LightDirection;
                #endif

                positionWS = ApplyShadowBias(positionWS, normalWS, lightDirectionWS);
                output.positionHCS = TransformWorldToHClip(positionWS);

                #if UNITY_REVERSED_Z
                    output.positionHCS.z = min(output.positionHCS.z, UNITY_NEAR_CLIP_VALUE);
                #else
                    output.positionHCS.z = max(output.positionHCS.z, UNITY_NEAR_CLIP_VALUE);
                #endif

                return output;
            }

            half4 ShadowFrag(ShadowVaryings input) : SV_Target
            {
                return 0;
            }

            ENDHLSL
        }
    }
}