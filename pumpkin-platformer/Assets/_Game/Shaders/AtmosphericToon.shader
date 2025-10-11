Shader "Custom/AtmosphericToon"
{
    Properties
    {
        [MainTexture] _BaseMap("Base Map", 2D) = "white" {}
        [MainColor] _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        
        [Space(10)]
        [Header(Toon Shading)]
        _ShadowThreshold("Shadow Threshold", Range(0, 1)) = 0.5
        _ShadowSoftness("Shadow Softness", Range(0, 1)) = 0.15
        _ShadowColor("Shadow Color", Color) = (0.1, 0.1, 0.15, 1)
        
        [Space(10)]
        [Header(Atmospheric)]
        _DarknessMultiplier("Darkness Multiplier", Range(0, 2)) = 1.2
        _EmissiveBoost("Emissive Boost", Range(0, 1)) = 0.3
        [HDR] _EmissiveColor("Emissive Color", Color) = (0.2, 0.2, 0.2, 1)
        
        [Space(10)]
        [Header(Rim Lighting)]
        _RimPower("Rim Power", Range(0, 10)) = 4
        _RimIntensity("Rim Intensity", Range(0, 2)) = 0.5
        [HDR] _RimColor("Rim Color", Color) = (1, 0.8, 0.4, 1)
        
        [Space(10)]
        [Header(Specular)]
        _SpecularSharpness("Specular Sharpness", Range(1, 128)) = 48
        _SpecularIntensity("Specular Intensity", Range(0, 1)) = 0.4
        [HDR] _SpecularColor("Specular Color", Color) = (1, 1, 1, 1)
        
        [Space(10)]
        [Header(Outline)]
        _OutlineWidth("Outline Width", Range(0, 0.1)) = 0.008
        _OutlineColor("Outline Color", Color) = (0, 0, 0, 1)
        
        [Space(10)]
        [Header(Surface Options)]
        [Enum(UnityEngine.Rendering.CullMode)] _Cull("Cull Mode", Float) = 2
        [Toggle] _ZWrite("Z Write", Float) = 1
    }

    SubShader
    {
        Tags 
        { 
            "RenderType" = "Opaque" 
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Geometry"
        }
        
        Pass
        {
            Name "AtmosphericToonForward"
            Tags { "LightMode" = "UniversalForward" }
            
            Cull [_Cull]
            ZWrite [_ZWrite]
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            
            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);
            
            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                half4 _BaseColor;
                half4 _ShadowColor;
                half4 _RimColor;
                half4 _SpecularColor;
                half4 _EmissiveColor;
                half _ShadowThreshold;
                half _ShadowSoftness;
                half _RimPower;
                half _RimIntensity;
                half _SpecularSharpness;
                half _SpecularIntensity;
                half _DarknessMultiplier;
                half _EmissiveBoost;
            CBUFFER_END
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float2 uv : TEXCOORD2;
                float fogCoord : TEXCOORD3;
            };
            
            Varyings vert(Attributes input)
            {
                UNITY_SETUP_INSTANCE_ID(input);
                
                Varyings output = (Varyings)0;
                
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS);
                
                output.positionHCS = vertexInput.positionCS;
                output.positionWS = vertexInput.positionWS;
                output.normalWS = normalInput.normalWS;
                output.uv = TRANSFORM_TEX(input.uv, _BaseMap);
                output.fogCoord = ComputeFogFactor(vertexInput.positionCS.z);
                
                return output;
            }
            
            half4 frag(Varyings input) : SV_Target
            {
                half4 baseMap = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv);
                half4 baseColor = baseMap * _BaseColor;
                
                float3 normalWS = normalize(input.normalWS);
                float3 lightDir = normalize(_MainLightPosition.xyz);
                float3 viewDir = normalize(_WorldSpaceCameraPos - input.positionWS);
                float3 halfDir = normalize(lightDir + viewDir);
                
                float NdotL = dot(normalWS, lightDir);
                float NdotV = dot(normalWS, viewDir);
                float NdotH = dot(normalWS, halfDir);
                
                float4 shadowCoord = TransformWorldToShadowCoord(input.positionWS);
                float shadow = MainLightRealtimeShadow(shadowCoord);
                
                float lightIntensity = NdotL * 0.5 + 0.5;
                float litShadow = lightIntensity * shadow;
                
                // Toon shading step with darker falloff
                float shadowStep = smoothstep(_ShadowThreshold - _ShadowSoftness, 
                                            _ShadowThreshold + _ShadowSoftness, litShadow);
                
                // Darker shadow areas for atmospheric feel
                half3 shadowArea = lerp(_ShadowColor * _DarknessMultiplier, baseColor.rgb, shadowStep);
                
                // Specular calculation
                float specularStep = smoothstep(1.0 - _SpecularSharpness * 0.01, 
                                              1.0 - (_SpecularSharpness * 0.01 - 0.05), NdotH);
                
                // Rim lighting with warm glow (key for the "signal" aesthetic)
                float rimMask = 1.0 - abs(NdotV);
                float rimStep = smoothstep(1.0 - _RimPower * 0.1, 
                                         1.0 - (_RimPower * 0.1 - _RimIntensity * 0.1), rimMask);
                
                // Combine lighting
                half3 diffuse = _MainLightColor.rgb * shadowArea * shadowStep;
                half3 specular = _SpecularColor.rgb * _SpecularIntensity * shadow * shadowStep * specularStep;
                half3 rim = _RimColor.rgb * rimStep * (1.0 - shadow * 0.5);
                
                // Minimal ambient with emissive boost for spooky feel
                half3 ambient = SampleSH(normalWS) * baseColor.rgb * 0.2;
                half3 emissive = _EmissiveColor.rgb * _EmissiveBoost * baseColor.rgb;
                
                half3 finalColor = diffuse + specular + rim + ambient + emissive;
                
                finalColor = MixFog(finalColor, input.fogCoord);
                
                return half4(finalColor, baseColor.a);
            }
            ENDHLSL
        }
        
        Pass
        {
            Name "SilhouetteOutline"
            Tags { "LightMode" = "SRPDefaultUnlit" }
            
            Cull Front
            ZWrite On
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            CBUFFER_START(UnityPerMaterial)
                float _OutlineWidth;
                half4 _OutlineColor;
            CBUFFER_END
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };
            
            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
            };
            
            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;
                
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS);
                
                float3 positionWS = vertexInput.positionWS + normalInput.normalWS * _OutlineWidth;
                output.positionHCS = TransformWorldToHClip(positionWS);
                
                return output;
            }
            
            half4 frag(Varyings input) : SV_Target
            {
                return _OutlineColor;
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
            Cull [_Cull]

            HLSLPROGRAM
            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment
            
            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"
            ENDHLSL
        }
        
        Pass
        {
            Name "DepthOnly"
            Tags { "LightMode" = "DepthOnly" }

            ZWrite On
            ColorMask 0
            Cull [_Cull]

            HLSLPROGRAM
            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment
            
            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/DepthOnlyPass.hlsl"
            ENDHLSL
        }
    }
    
    Fallback "Hidden/Universal Render Pipeline/FallbackError"
}