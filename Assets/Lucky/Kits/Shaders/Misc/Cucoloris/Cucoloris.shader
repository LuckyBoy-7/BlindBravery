Shader "Mine/Cucoloris"
{
    Properties
    {
        _MainTex("MainTexture", 2D) = "white"{}
        _Color ("Color", Color) = (1,1,1,1)
        _Amount ("Amount", Range(0, 1)) = 0.5
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
            float4 _Color;
            float _Amount;

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
                color.rgb = _Color.rgb * _Amount + color.rgb * (1 - _Amount);
                return color;
            }
            ENDCG
        }
    }
    Fallback "Diffuse"
}