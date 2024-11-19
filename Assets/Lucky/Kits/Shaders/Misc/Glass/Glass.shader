Shader "Lucky/Misc/Glass"
{
    Properties
    {
        _MainTex("MainTexture", 2D) = "white"{}
        _GlassTex("GlassTexture", 2D) = "white"{}
        _DistortionScaler("DistortionScaler", Range(0,0.1)) = 0.1
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
            sampler2D _GlassTex;
            float4 _MainTex_ST;
            float4 _GlassTex_ST;
            float _DistortionScaler;

            struct v2f
            {
                float4 pos : SV_POSITION;
                float4 uv : TEXCOORD0;
            };

            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);

                // 偏移好像还有点问题，回来再改
                o.uv.zw = TRANSFORM_TEX(v.texcoord, _GlassTex);

                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                // 因为是2d平面，有没有光的方向所以做玻璃效果只能采取一些trick
                // 以前是光是平行光，玻璃上凹凸不平以实现扭曲效果
                // 但现在玻璃是平坦的，就只能用纹理扭曲平行光了
                // 我们实现时可以更草率点，r*2 - 1通道偏移x, g通道*2 - 1偏移y
                float4 offsetColor = tex2D(_GlassTex, i.uv.zw);
                offsetColor = (offsetColor * 2 - 1) * _DistortionScaler;

                return tex2D(_MainTex, float2(i.uv.x + offsetColor.x, i.uv.y + offsetColor.y));
            }
            ENDCG
        }
    }
    Fallback "Diffuse"
}