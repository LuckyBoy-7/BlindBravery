Shader "Mine/IntersectionColor"
{
    Properties
    {
        _MainTex("MainTexture", 2D) = "white"{}
        _Color ("Color", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent+1"
        }
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            // 如果像素被discard了，那么模板信息将不会被写入
            Stencil
            {
                Ref 2
                Comp Always  // 这个comp是拿ref跟buffer比，不是buffer跟ref比
                Pass Replace
            }
            CGPROGRAM
            #include <UnityCG.cginc>
            #pragma vertex vert
            #pragma fragment frag

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float4 _IntersectionColor;

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
                float4 color = tex2D(_MainTex, i.uv);
                if (color.a == 0)
                    discard;
                return color;
            }
            ENDCG
        }
    }
    Fallback "Diffuse"
}