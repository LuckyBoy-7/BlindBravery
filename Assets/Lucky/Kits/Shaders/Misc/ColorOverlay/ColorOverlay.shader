Shader "Mine/ColorOverlay"
{
    Properties
    {
        _MainTex("MainTexture", 2D) = "white"{}
        _Color ("Color", Color) = (1,1,1,1)
        _Alpha ("Alpha", Float) = 0.5
    }

    SubShader
    {
        // blend后才能把透明像素搞掉
        //        Tags
        //        {
        //            "Queue" = "Transparent"
        //        }

        Blend One One // 线性减淡(变白了就淡了吧   )
        //        Blend OneMinusDstColor One // 柔和相加

        //        BlendOp Max // 变亮
        //        Blend One One

        //             Blend OneMinusDstColor One

        //        Blend DstColor Zero  // 正片叠底， 相乘
        //        Blend SrcAlpha OneMinusSrcAlpha  // 透明度混合
        //        Blend DstColor SrcColor  // 两倍相乘

        //        BlendOp Min  // 变暗
        //        Blend One One




        Pass
        {
            CGPROGRAM
            #include <UnityCG.cginc>
            #pragma vertex vert
            #pragma fragment frag

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _Alpha;

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
                float alpha = tex2D(_MainTex, i.uv).a;
                if (alpha == 0)
                    discard;
                return float4(_Color.rgb, _Alpha);
            }
            ENDCG
        }
    }
    Fallback "Diffuse"
}