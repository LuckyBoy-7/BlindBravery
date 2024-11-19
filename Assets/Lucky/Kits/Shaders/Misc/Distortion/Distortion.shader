Shader "Mine/Distortion"
{
    Properties
    {
        _MainTex("MainTexture", 2D) = "white"{}
        _NoiseScale("NoiseScale", Range(0, 0.05)) = 0.01
        _NoiseSnap("NoiseSnap", Range(0.005, 0.02)) = 0.01
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
        }
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            #include <UnityCG.cginc>
            #pragma vertex vert
            #pragma fragment frag

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _NoiseScale;
            float _NoiseSnap;

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float rand(float2 seed)
            {
                return frac(sin(dot(seed.xy, float2(12.9898, 78.233))) * 43758.5453);
            }

            inline float snap(float x, float snap)
            {
                return snap * round(x / snap);
            }

            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                // 扭曲一个个格子
                o.uv += rand(o.uv * snap(_Time.x, _NoiseSnap)) * _NoiseScale;

                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                return tex2D(_MainTex, i.uv);
            }
            ENDCG
        }
    }
    Fallback "Diffuse"
}