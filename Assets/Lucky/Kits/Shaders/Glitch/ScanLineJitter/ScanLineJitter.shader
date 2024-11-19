Shader "Lucky/Glitch/ScanLineJitter"
{
    Properties
    {
        _MainTex ("MainTex", 2D) = "white" { }
    }
    SubShader
    {
        ZTest Always Cull Off ZWrite Off
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            sampler2D _MainTex;
            float _Frequency;
            float _Intensity;
            int _SegmentHeight;

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float randomNoise(float x, float y)
            {
                return frac(sin(dot(float2(x, y), float2(12.9898, 78.233))) * 43758.5453);
            }

            float4 Horizontal(float2 uv, float intensity, float frequency)
            {
                float amount = 0.005f + pow(intensity, 3) * 0.1;
                float threshold = saturate(1 - intensity * 1.2);
                float strength = 0.5 + 0.5 * cos(_Time.y * frequency);
                float jitter = randomNoise(trunc(uv.y * _SegmentHeight), _Time.x) * 2 - 1;
                // 水平偏移量
                jitter *= step(threshold, abs(jitter)) * amount * strength;
                float4 sceneColor = tex2D(_MainTex, frac(uv + float2(jitter, 0)));
                return sceneColor;
            }

            float4 Vertical(float2 uv, float intensity, float frequency)
            {
                float amount = 0.005f + pow(intensity, 3) * 0.1;
                float threshold = saturate(1 - intensity * 1.2);
                float strength = 0.5 + 0.5 * cos(_Time.y * frequency);
                float jitter = randomNoise(uv.x, _Time.x) * 2 - 1;
                jitter *= step(threshold, abs(jitter)) * amount * strength;
                float4 sceneColor = tex2D(_MainTex, frac(uv + float2(0, jitter)));
                return sceneColor;
            }

            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float4 color = Horizontal(i.uv, _Intensity, _Frequency);
                // float4 color = Vertical(i.uv, _Intensity, _Frequency);
                return color;
            }
            ENDCG
        }
    }
}