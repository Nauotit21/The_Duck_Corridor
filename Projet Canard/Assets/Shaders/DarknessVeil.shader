Shader "Custom/DarknessVeil"
{
    Properties
    {
        _InnerRadius  ("Rayon visible (Inner)",  Float) = 1.5
        _OuterRadius  ("Rayon total (Outer)",    Float) = 4.0
        _DarknessColor("Couleur ténèbres",       Color) = (0, 0, 0, 1)
        _NoiseScale   ("Bruit (ondulation)",     Float) = 2.0
        _NoiseStrength("Force du bruit",         Float) = 0.15
    }

    SubShader
    {
        // Rendu APRÈS l'opaque, AVANT le transparent
        Tags { "Queue"="Transparent+100" "RenderType"="Transparent" }

        Pass
        {
            // On rend l'intérieur de la sphère
            Cull Front
            ZWrite Off
            ZTest LEqual
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes { float4 positionOS : POSITION; };
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
            };

            float  _InnerRadius, _OuterRadius, _NoiseScale, _NoiseStrength;
            float4 _DarknessColor;

            // Bruit simple pour rendre le bord organique
            float noise(float3 p)
            {
                return frac(sin(dot(p, float3(127.1, 311.7, 74.2))) * 43758.5);
            }

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.positionCS = TransformWorldToHClip(OUT.positionWS);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // Distance du fragment au centre de la sphère (= position joueur)
                float3 center = UNITY_MATRIX_M._m03_m13_m23; // position world de l'objet
                float dist = distance(IN.positionWS, center);

                // Bruit pour bord irrégulier
                float n = noise(IN.positionWS * _NoiseScale) * _NoiseStrength;
                float innerR = _InnerRadius + n;

                // Gradient : 0 (transparent) au bord intérieur → 1 (opaque) à l'extérieur
                float alpha = saturate((dist - innerR) / (_OuterRadius - innerR));
                alpha = smoothstep(0.0, 1.0, alpha);

                return half4(_DarknessColor.rgb, alpha * _DarknessColor.a);
            }
            ENDHLSL
        }
    }
}