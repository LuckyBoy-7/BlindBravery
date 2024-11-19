Shader "Mine/ScratchTicket"
{
    Properties
    {
        _MainTex("MainTexture", 2D) = "white"{}
        _LineTex("LineTex", 2D) = "white"{}
        _MaskColor("MaskColor", Color) = (0.5,0.5,0.5,1)
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
            sampler2D _LineTex;
            float4 _MaskColor;
            float4x4 _paintCameraVP;

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 worldScreenPos : TEXCOORD1;
            };

            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.worldScreenPos = mul(_paintCameraVP, mul(unity_ObjectToWorld, v.vertex));
                o.worldScreenPos /= o.worldScreenPos.w;
                o.worldScreenPos.xy = o.worldScreenPos.xy * 0.5 + 0.5;

                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float4 color = tex2D(_MainTex, i.uv);
                if (color.a == 0)
                    discard;

                if (tex2D(_LineTex, i.worldScreenPos.xy).a == 1)
                    return color;
                return _MaskColor;
            }
            ENDCG
        }
    }
    Fallback "Diffuse"
}