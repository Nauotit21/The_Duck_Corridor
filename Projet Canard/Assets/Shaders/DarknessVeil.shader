Shader "Custom/DarknessVeil"
{
    Properties
    {
        _InnerRadius   ("Rayon visible (Inner)",    Float) = 1.5
        _OuterRadius   ("Rayon total (Outer)",      Float) = 4.0
        _DarknessColor ("Couleur ténèbres",         Color) = (0, 0, 0, 1)
        _NoiseScale    ("Bruit (ondulation)",        Float) = 2.0
        _NoiseStrength ("Force du bruit",           Float) = 0.15
    
        _LookDirection ("Direction du regard",      Vector) = (0, 0, 1, 0)
        _LookBonus     ("Bonus rayon (regard)",     Float) = 2.0
        _LookSharpness ("Netteté du cône",          Float) = 4.0
    }

    SubShader
    {
        Tags { "Queue"="Transparent+100" "RenderType"="Transparent" }

        Pass
        {
            Cull Front
            ZWrite Off
            ZTest LEqual
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #pragma instancing_options renderinglayer

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            UNITY_INSTANCING_BUFFER_START(Props)
            UNITY_INSTANCING_BUFFER_END(Props)

            struct Attributes
            {
                float4 positionOS : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            float  _InnerRadius, _OuterRadius, _NoiseScale, _NoiseStrength;
            float  _LookBonus, _LookSharpness;
            float4 _DarknessColor;
            float4 _LookDirection;

            float noise(float3 p)
            {
                return frac(sin(dot(p, float3(127.1, 311.7, 74.2))) * 43758.5);
            }

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.positionCS = TransformWorldToHClip(OUT.positionWS);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);

                float3 center = UNITY_MATRIX_M._m03_m13_m23;
                float3 toFrag = IN.positionWS - center;
                float dist    = length(toFrag);
                float3 dir    = toFrag / (dist + 0.0001);

      
                float3 lookDir  = normalize(_LookDirection.xyz);
                float  dotLook  = dot(dir, lookDir);

              
                float lookFactor = saturate(pow(max(dotLook, 0.0), _LookSharpness));

                
                float n       = noise(IN.positionWS * _NoiseScale) * _NoiseStrength;
                float innerR  = _InnerRadius + n + (_LookBonus * lookFactor);

                float alpha = saturate((dist - innerR) / (_OuterRadius - innerR));
                alpha = smoothstep(0.0, 1.0, alpha);

                return half4(_DarknessColor.rgb, alpha * _DarknessColor.a);
            }
            ENDHLSL
        }
    }
}