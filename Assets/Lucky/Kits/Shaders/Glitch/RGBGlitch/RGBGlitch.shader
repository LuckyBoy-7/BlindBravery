Shader "Lucky/Glitch/RGBGlitch"
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
            float _Intensity;
            float _Frequency;
            float _Speed;
            float _Exponent;

            struct a2v
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float4 Horizontal(float2 uv, float intensity, float frequency, float speed)
            {
                float time = _Time.y * speed;
                half splitAmount = 0.5 + 0.5 * cos(_Time.y * frequency);
                splitAmount = splitAmount * intensity;
                splitAmount *= 0.001;
                float4 colorR = tex2D(_MainTex, uv + float2(sin(time * 0.2) * splitAmount, 0));
                float4 colorG = tex2D(_MainTex, uv + float2(sin(time * 0.1) * splitAmount, 0));
                float4 colorB = tex2D(_MainTex, uv);
                return float4(colorR.r, colorG.g, colorB.b, 1);
            }

            float4 Vertical(float2 uv, float intensity, float frequency, float speed)
            {
                float time = _Time.y * speed;
                half splitAmount = 0.5 + 0.5 * cos(_Time.y * frequency);
                splitAmount = splitAmount * intensity;
                splitAmount *= 0.001;
                float4 colorR = tex2D(_MainTex, uv + float2(0, sin(time * 0.2) * splitAmount));
                float4 colorG = tex2D(_MainTex, uv + float2(0, sin(time * 0.1) * splitAmount));
                float4 colorB = tex2D(_MainTex, uv);
                return float4(colorR.r, colorG.g, colorB.b, 1);
            }

            float4 Horizontal_Vertical(float2 uv, float intensity, float frequency, float speed)
            {
                float t = _Time.y * speed; // 模拟速度
                float val = (1 + sin(t * 6.0)) * 0.5f;
                val *= 1 + sin(t * 16) * 0.5f;
                val *= 1 + sin(t * 19) * 0.5f;
                val *= 1 + sin(t * 27) * 0.5f;
                val = pow(val, _Exponent);


                half splitAmount = 0.5 + 0.5 * cos(_Time.y * frequency);
                splitAmount = splitAmount * intensity; // 分离程度
                splitAmount *= 0.001;
                float4 colorR = tex2D(
                    _MainTex, uv + float2(sin(val * 0.2) * splitAmount, sin(val * 0.2) * splitAmount * 0.5));
                float4 colorG = tex2D(
                    _MainTex, uv + float2(sin(val * 0.1) * splitAmount, sin(val * 0.1) * splitAmount * 0.5));
                float4 colorB = tex2D(_MainTex, uv);
                return float4(colorR.r, colorG.g, colorB.b, 1);
            }

            v2f vert(a2v v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                //float4 color = Horizontal(i.uv,_Intensity,_Frequency,_Speed);
                //float4 color = Vertical(i.uv,_Intensity,_Frequency,_Speed);
                float4 color = Horizontal_Vertical(i.uv, _Intensity, _Frequency, _Speed);
                return color;
            }
            ENDCG
        }
    }
}