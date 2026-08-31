Shader "IncrementalMining/Learning/25_MaskMap"
{
    Properties
    {
        [MainTexture] _BaseMap("Texture", 2D) = "white" {}
        [MainColor] _BaseColor("Color", Color) = (1, 1, 1, 1)
        _Smoothness("Smoothness", Range(0, 1)) = 0.5
        _Metallic("Metallic", Range(0, 1)) = 0

        [Toggle(_MASKMAP_ON)] _UseMaskMap("Use Mask Map", Float) = 0
        [NoScaleOffset] _MaskMap("Mask Map (R: Metallic, G: AO, A: Smoothness)", 2D) = "white" {}
        _OcclusionStrength("Occlusion Strength", Range(0, 1)) = 1

        [Toggle(_EMISSION_ON)] _UseEmission("Use Emission", Float) = 0
        [NoScaleOffset] _EmissionMap("Emission Mask", 2D) = "white" {}
        [HDR] _EmissionColor("Emission Color", Color) = (0, 0, 0, 1)

        [NoScaleOffset] _DissolveMap("Dissolve Map", 2D) = "white" {}
        _Dissolve("Dissolve", Range(0, 1)) = 0
        _DissolveEdgeWidth("Dissolve Edge Width", Range(0.001, 0.25)) = 0.05
        [HDR] _DissolveEdgeColor("Dissolve Edge Color", Color) = (0, 1, 1, 1)

        [Toggle(_NORMALMAP_ON)] _UseNormalMap("Use Normal Map", Float) = 1
        [Normal][NoScaleOffset] _NormalMap("Normal Map", 2D) = "bump" {}
        _NormalStrength("Normal Strength", Range(0, 2)) = 1

        _F0("F0 Reflectance", Range(0, 1)) = 0.04
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "TransparentCutout"
            "Queue" = "AlphaTest"
        }

        Pass
        {
            Name "ForwardLit"

            Tags
            {
                "LightMode" = "UniversalForward"
            }

            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment Frag

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #pragma multi_compile _ _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _REFLECTION_PROBE_BLENDING
            #pragma multi_compile_fragment _ _REFLECTION_PROBE_BOX_PROJECTION

            #pragma shader_feature_local _NORMALMAP_ON
            #pragma shader_feature_local_fragment _MASKMAP_ON
            #pragma shader_feature_local_fragment _EMISSION_ON

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/GlobalIllumination.hlsl"

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

                #if defined(_NORMALMAP_ON)
                    half3 tangentWS : TEXCOORD3;
                    half3 bitangentWS : TEXCOORD4;
                #endif
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            TEXTURE2D(_EmissionMap);
            SAMPLER(sampler_EmissionMap);

            TEXTURE2D(_DissolveMap);
            SAMPLER(sampler_DissolveMap);

            TEXTURE2D(_NormalMap);
            SAMPLER(sampler_NormalMap);

            TEXTURE2D(_MaskMap);
            SAMPLER(sampler_MaskMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                half4 _BaseColor;
                half4 _EmissionColor;
                half4 _DissolveEdgeColor;
                half _Smoothness;
                half _Metallic;
                half _OcclusionStrength;
                half _Dissolve;
                half _DissolveEdgeWidth;
                half _NormalStrength;
                half _F0;
                half _UseNormalMap;
                half _UseMaskMap;
                half _UseEmission;
            CBUFFER_END

            half3 FresnelSchlick(half cosTheta, half3 f0)
            {
                return f0 + (1.0h - f0) * pow(1.0h - cosTheta, 5.0h);
            }

            float DistributionGGX(float NdotH, float smoothness)
            {
                float perceptualRoughness = 1.0 - smoothness;
                float alpha = max(perceptualRoughness * perceptualRoughness, 0.002);
                float alphaSquared = alpha * alpha;

                float denominator = NdotH * NdotH * (alphaSquared - 1.0) + 1.0;
                denominator = PI * denominator * denominator;

                return alphaSquared / (denominator + 0.0000001);
            }

            float GeometrySchlickGGX(float NdotDirection, float roughness)
            {
                float k = (roughness + 1.0) * (roughness + 1.0) / 8.0;
                return NdotDirection / (NdotDirection * (1.0 - k) + k);
            }

            float GeometrySmith(float NdotV, float NdotL, float smoothness)
            {
                float roughness = 1.0 - smoothness;

                float viewGeometry = GeometrySchlickGGX(NdotV, roughness);
                float lightGeometry = GeometrySchlickGGX(NdotL, roughness);

                return viewGeometry * lightGeometry;
            }

            half3 CalculateDirectPBR(Light light, half3 albedo, half3 normalWS, half3 viewDirectionWS, half metallic, half smoothness)
            {
                half3 lightDirectionWS = normalize(light.direction);
                half3 halfDirectionWS = SafeNormalize(lightDirectionWS + viewDirectionWS);

                half NdotV = saturate(dot(normalWS, viewDirectionWS));
                half NdotL = saturate(dot(normalWS, lightDirectionWS));
                half NdotH = saturate(dot(normalWS, halfDirectionWS));
                half VdotH = saturate(dot(viewDirectionWS, halfDirectionWS));

                float distribution = DistributionGGX(NdotH, smoothness);
                float geometry = GeometrySmith(NdotV, NdotL, smoothness);

                half3 dielectricF0 = half3(_F0, _F0, _F0);
                half3 f0 = lerp(dielectricF0, albedo, metallic);
                half3 fresnel = FresnelSchlick(VdotH, f0);

                float denominator = max(4.0 * NdotV * NdotL, 0.0001);
                half3 specularBRDF = distribution * geometry * fresnel / denominator;

                half3 diffuseWeight = (1.0h - fresnel) * (1.0h - metallic);
                half3 diffuseBRDF = diffuseWeight * albedo / PI;

                half attenuation = light.distanceAttenuation * light.shadowAttenuation;

                return (diffuseBRDF + specularBRDF) * light.color * NdotL * attenuation;
            }

            half3 CalculateIndirectPBR(half3 albedo, half3 normalWS, half3 viewDirectionWS, float3 positionWS, float2 screenUV, half metallic, half smoothness, half occlusion)
            {
                half perceptualRoughness = 1.0h - smoothness;
                half roughness = max(perceptualRoughness * perceptualRoughness, 0.002h);
                half roughnessSquared = roughness * roughness;

                half3 reflectionDirectionWS = reflect(-viewDirectionWS, normalWS);
                half3 diffuseEnvironment = SampleSH(normalWS);
                half3 specularEnvironment = GlossyEnvironmentReflection(reflectionDirectionWS, positionWS, perceptualRoughness, 1.0h, screenUV);

                half3 dielectricF0 = half3(_F0, _F0, _F0);
                half3 f0 = lerp(dielectricF0, albedo, metallic);

                half NdotV = saturate(dot(normalWS, viewDirectionWS));
                half oneMinusNdotV = 1.0h - NdotV;
                half fresnelTerm = oneMinusNdotV * oneMinusNdotV;
                fresnelTerm *= fresnelTerm;

                half reflectivity = max(f0.r, max(f0.g, f0.b));
                half grazingTerm = saturate(smoothness + reflectivity);
                half surfaceReduction = 1.0h / (roughnessSquared + 1.0h);

                half3 grazingColor = half3(grazingTerm, grazingTerm, grazingTerm);
                half3 environmentBRDF = surfaceReduction * lerp(f0, grazingColor, fresnelTerm);

                half oneMinusReflectivity = (1.0h - _F0) * (1.0h - metallic);
                half3 indirectDiffuse = diffuseEnvironment * albedo * oneMinusReflectivity;
                half3 indirectSpecular = specularEnvironment * environmentBRDF;

                return (indirectDiffuse + indirectSpecular) * occlusion;
            }

            Varyings Vert(Attributes input)
            {
                Varyings output;

                VertexPositionInputs positionInputs = GetVertexPositionInputs(input.positionOS.xyz);

                #if defined(_NORMALMAP_ON)
                    VertexNormalInputs normalInputs = GetVertexNormalInputs(input.normalOS, input.tangentOS);
                    output.normalWS = normalInputs.normalWS;
                    output.tangentWS = normalInputs.tangentWS;
                    output.bitangentWS = normalInputs.bitangentWS;
                #else
                    output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                #endif

                output.positionHCS = positionInputs.positionCS;
                output.positionWS = positionInputs.positionWS;
                output.uv = TRANSFORM_TEX(input.uv, _BaseMap);

                return output;
            }

            half4 Frag(Varyings input) : SV_Target
            {
                half dissolveValue = SAMPLE_TEXTURE2D(_DissolveMap, sampler_DissolveMap, input.uv).r;
                half dissolveThreshold = lerp(-0.01h, 1.01h, _Dissolve);
                clip(dissolveValue - dissolveThreshold);

                half edgeMask = 1.0h - smoothstep(dissolveThreshold, dissolveThreshold + _DissolveEdgeWidth, dissolveValue);
                half3 dissolveEdge = _DissolveEdgeColor.rgb * edgeMask;

                half3 geometricNormalWS = normalize(input.normalWS);
                half3 normalWS = normalize(input.normalWS);

                #if defined(_NORMALMAP_ON)
                    half3 tangentWS = normalize(input.tangentWS);
                    half3 bitangentWS = normalize(input.bitangentWS);

                    half4 normalSample = SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, input.uv);
                    half3 normalTS = UnpackNormalScale(normalSample, _NormalStrength);

                    normalWS = normalize(tangentWS * normalTS.x + bitangentWS * normalTS.y + normalWS * normalTS.z);
                #endif

                half4 albedo = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv) * _BaseColor;

                half metallic = _Metallic;
                half smoothness = _Smoothness;
                half occlusion = 1.0h;

                #if defined(_MASKMAP_ON)
                    half4 mask = SAMPLE_TEXTURE2D(_MaskMap, sampler_MaskMap, input.uv);

                    metallic *= mask.r;
                    smoothness *= mask.a;
                    occlusion = lerp(1.0h, mask.g, _OcclusionStrength);
                #endif

                half3 viewDirectionWS = GetWorldSpaceNormalizeViewDir(input.positionWS);

                float4 shadowCoord = TransformWorldToShadowCoord(input.positionWS);
                Light mainLight = GetMainLight(shadowCoord);

                half3 finalColor = CalculateDirectPBR(mainLight, albedo.rgb, normalWS, viewDirectionWS, metallic, smoothness);

                #if defined(_ADDITIONAL_LIGHTS)
                    uint additionalLightsCount = GetAdditionalLightsCount();

                    for (uint lightIndex = 0; lightIndex < additionalLightsCount; ++lightIndex)
                    {
                        Light additionalLight = GetAdditionalLight(lightIndex, input.positionWS);
                        finalColor += CalculateDirectPBR(additionalLight, albedo.rgb, normalWS, viewDirectionWS, metallic, smoothness);
                    }
                #endif

                float2 screenUV = GetNormalizedScreenSpaceUV(input.positionHCS);
                finalColor += CalculateIndirectPBR(albedo.rgb, normalWS, viewDirectionWS, input.positionWS, screenUV, metallic, smoothness, occlusion);

                #if defined(_EMISSION_ON)
                    half emissionMask = SAMPLE_TEXTURE2D(_EmissionMap, sampler_EmissionMap, input.uv).r;
                    finalColor += _EmissionColor.rgb * emissionMask;
                #endif

                finalColor += dissolveEdge;

                return half4(finalColor, albedo.a);
            }

            ENDHLSL
        }

        Pass
        {
            Name "ShadowCaster"

            Tags
            {
                "LightMode" = "ShadowCaster"
            }

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

            struct ShadowAttributes
            {
                float3 positionOS : POSITION;
                float3 normalOS : NORMAL;
                half4 tangentOS : TANGENT;
                float2 uv : TEXCOORD0;
            };

            struct ShadowVaryings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_DissolveMap);
            SAMPLER(sampler_DissolveMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                half4 _BaseColor;
                half4 _EmissionColor;
                half4 _DissolveEdgeColor;
                half _Smoothness;
                half _Metallic;
                half _OcclusionStrength;
                half _Dissolve;
                half _DissolveEdgeWidth;
                half _NormalStrength;
                half _F0;
                half _UseNormalMap;
                half _UseMaskMap;
                half _UseEmission;
            CBUFFER_END

            half3 FresnelSchlick(half cosTheta, half3 f0)
            {
                return f0 + (1.0h - f0) * pow(1.0h - cosTheta, 5.0h);
            }

            float3 _LightDirection;
            float3 _LightPosition;

            ShadowVaryings ShadowVert(ShadowAttributes input)
            {
                ShadowVaryings output;
        
                output.uv = TRANSFORM_TEX(input.uv, _BaseMap);

                float3 positionWS = TransformObjectToWorld(input.positionOS);
                float3 normalWS = TransformObjectToWorldNormal(input.normalOS);

                #if _CASTING_PUNCTUAL_LIGHT_SHADOW
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
                half dissolveValue = SAMPLE_TEXTURE2D(_DissolveMap, sampler_DissolveMap, input.uv).r;
                half dissolveThreshold = lerp(-0.01h, 1.01h, _Dissolve);

                clip(dissolveValue - dissolveThreshold);

                return 0;
            }

            ENDHLSL
        }
    }
}