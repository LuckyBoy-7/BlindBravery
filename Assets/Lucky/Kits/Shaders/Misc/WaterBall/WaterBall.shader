Shader "Lucky/Misc/WaterBall"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex("MainTex", 2D) = "white"{}
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

            float4 _Color;
            sampler2D _MainTex;
            float4 _MainTex_ST;

            struct v2f
            {
                float4 clipPos : SV_POSITION;
                float2 uv: TEXCOORD0;
            };

            v2f vert(appdata_base i)
            {
                v2f o;
                o.clipPos = UnityObjectToClipPos(i.vertex);
                o.uv = TRANSFORM_TEX(i.texcoord, _MainTex);

                float2 vec = o.uv - float2(0.5, 0.5);
                float2 normalized = normalize(vec);
                // 从右端点开始逆时针旋转, x就从角度上取
                float x = atan2(normalized.y, normalized.x) + _Time.y * 1.5;
                float y = 0;
                // sin1, sin2, sin3, sin4
                y += 0.5 * sin(5 * x);
                y += 0.4 * sin(1.2 * x);
                y += 0.23 * sin(2 * x - 3.41);
                y += 0.13 * sin(2.1 * x + 5.79);

                vec += normalized * y / 18;
                o.uv = vec + float2(0.5, 0.5);
                o.uv = saturate(o.uv);

                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float4 color = tex2D(_MainTex, i.uv);
                return color * _Color;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}