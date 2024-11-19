Shader "Lucky/Misc/Shine"
{
    Properties
    {
        _MainTex("MainTexture", 2D) = "white"{}
        _ShineTex("ShineTex", 2D) = "white"{}
        _ShineIntensity("ShineIntensity", Range(1, 5)) = 2
        _ShineRadians("ShineRadians", Range(0, 3.14)) = 1
        _ShineScrollingSpeed("ShineScrollingSpeed", Range(1, 3)) = 2
        _ShineThickness("ShineThickness", Range(0.1, 2)) = 1
        _ShineNumber("ShineNumber", Range(0.1, 3)) = 1
        _Color("Color", Color) = (1,1,1,1)
        _ReplacementColor("ReplacementColor", Color) =(1,1,1,1)
        _ReplacementK("ReplacementK", Range(0, 1)) = 0
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
            sampler2D _ShineTex;
            float4 _MainTex_ST;
            float4 _ShineTex_ST;

            float4 _Color;
            float4 _ReplacementColor;
            float _ReplacementK;
            float _ShineIntensity;
            float _ShineRadians;
            float _ShineScrollingSpeed;
            float _ShineThickness;
            float _ShineNumber;

            struct v2f
            {
                float4 clipPos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 screenPos : TEXCOORD1;
            };

            v2f vert(appdata_base v)
            {
                v2f o;
                o.clipPos = UnityObjectToClipPos(v.vertex);
                o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.screenPos = ComputeScreenPos(o.clipPos);
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float4 color = tex2D(_MainTex, i.uv.xy);
                // 因为实际旋转方向与采样方向相反, 所以这里矩阵直接反着写了
                float2x2 rotation = float2x2(
                    cos(_ShineRadians), sin(_ShineRadians),
                    -sin(_ShineRadians), cos(_ShineRadians)
                );

                // 我们的目标是让shien在世界坐标中, 所以我们要拿到uv相对视口坐标的位置
                i.screenPos /= i.screenPos.w;
                i.screenPos.y += _Time.w * _ShineScrollingSpeed;
                float2 uv = mul(rotation, i.screenPos);
                // 只跑一趟
                // uv = uv;

                // 循环
                // uv = frac(uv);

                // 变细
                // uv = frac(uv * 2);

                // 减高光条数, > 1的部分可以理解为无效被clamp了
                float x = 1 / _ShineThickness;
                float y = 1 / _ShineNumber;
                uv.y = uv.y * x % y;

                if (tex2D(_ShineTex, uv).r == 1)
                    color.rgb *= _ShineIntensity;


                color *= _Color;
                // 这里不要写a, 因为这样会把透明区域的部分也lerp出一部分颜色
                color.rgb = lerp(color.rgb, _ReplacementColor.rgb, _ReplacementK);
                return color;
            }
            ENDCG
        }
    }
    Fallback "Diffuse"
}