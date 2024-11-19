Shader "Lucky/Misc/SimpleOutline"
{
    Properties
    {
        _MainTex("MainTexture", 2D) = "white"{}
        _Color ("Color", Color) = (1,1,1,1)
        _LineColor ("LineColor", Color) = (1,1,1,1)
        _LineWidth ("LineWidth", Float) = 1
        _IsOuline ("IsOuline", Int) = 1
    }

    SubShader
    {
        // blend后才能把透明像素搞掉
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
            float4 _MainTex_TexelSize;
            float4 _Color;
            float4 _LineColor;
            float _LineWidth;
            int _IsOuline;

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);

                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float up = tex2D(_MainTex, i.uv + _LineWidth * _MainTex_TexelSize.xy * float2(0, 1)).a;
                float down = tex2D(_MainTex, i.uv + _LineWidth * _MainTex_TexelSize.xy * float2(0, -1)).a;
                float left = tex2D(_MainTex, i.uv + _LineWidth * _MainTex_TexelSize.xy * float2(-1, 0)).a;
                float right = tex2D(_MainTex, i.uv + _LineWidth * _MainTex_TexelSize.xy * float2(1, 0)).a;
                float4 alpha_around = float4(up, down, left, right);
                float4 color = tex2D(_MainTex, i.uv);
                // 内描边
                if (!_IsOuline)
                {
                    if (!all(alpha_around))
                        color.rgb = _LineColor.rgb;
                    return color;
                }


                // 外描边
                if (color.a == 0 && any(alpha_around))
                    color = _LineColor;
                return color;
            }
            ENDCG
        }
    }
    Fallback "Diffuse"
}