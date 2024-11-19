Shader "Lucky/Misc/WireCircle"
{
    Properties
    {
        _MainTex("MainTexture", 2D) = "white"{}
        _LineColor ("LineColor", Color) = (1,1,1,1)
        _LineWidth ("LineWidth", Float) = 0.1
        _Radius ("Radius", Float) = 1
    }

    SubShader
    {
        Pass
        {
            CGPROGRAM
            #include <UnityCG.cginc>
            #pragma vertex vert
            #pragma fragment frag

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _LineColor;
            float _LineWidth;
            float _Radius;

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
                float dx = 0.5f - i.uv.x, dy = 0.5f - i.uv.y;
                float sqrDist = pow(dx, 2) + pow(dy, 2);
                // if (sqrDist > 0.25f)
                //     discard;
                clip(0.25 - sqrDist);  // 把环外的切了
                if (sqrDist < pow(0.5 - _LineWidth, 2))
                    discard;
                // clip(sqrDist - pow(0.5 - _LineWidth, 2));  // 把环内的切了
                return _LineColor;
            }
            ENDCG
        }
    }
    Fallback "Diffuse"
}