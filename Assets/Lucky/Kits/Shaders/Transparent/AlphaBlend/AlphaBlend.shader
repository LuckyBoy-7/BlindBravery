Shader "Lucky/Transparent/AlphaBlend"
{

    Properties
    {
        _MainTex("MainTex", 2D) = "white" {}
        _AlphaScale("AlphaScale", Range(0, 1)) = 0.5
    }
    SubShader
    {
        Tags
        {
            // 只有定义了正确的 LightMode,我们才能得到 Unity 的内置光照变量，的_LightColorO
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
        }
        
        Pass
        {

            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #include <UnityCG.cginc>

            #pragma vertex vert
            #pragma fragment frag

            sampler2D _MainTex;
            float4 _MainTex_ST;

            fixed _AlphaScale;

            struct v2f
            {
                float4 vertex: SV_POSITION;
                float2 uv: TEXCOORD0;
            };

            v2f vert(appdata_base i)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(i.vertex);
                o.uv = TRANSFORM_TEX(i.texcoord, _MainTex);

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 albedo = tex2D(_MainTex, i.uv);

                return fixed4(albedo.xyz, albedo.a * _AlphaScale);
            }
            ENDCG
        }
    }
    Fallback "Transparent/VertexLit"
}