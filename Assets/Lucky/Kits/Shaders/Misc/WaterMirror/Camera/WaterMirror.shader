Shader "Lucky/Misc/WaterMirrorWithCamera"  // 要做镜子效果也差不多, 但是相机要选图层(但直觉上还是没自己控制渲染逻辑方便)
{
    Properties
    {
        _ViewTex("MainTexture", 2D) = "white"{}
        _DistortionScale ("DistortionScale", float) = 0.1
        _WaveOffset ("WaveOffset", float) = 1
    }

    SubShader
    {
        Cull Back // 因为我们反转了顶点, 又反转了transform, 所以看到的还是前面的部分
        Tags
        {
            "Queue" = "Transparent"
        }
        Pass
        {
            CGPROGRAM
            #include <UnityCG.cginc>
            #pragma vertex vert
            #pragma fragment frag

            sampler2D _ViewTex;
            float4 _ViewTex_ST;
            float _DistortionScale;
            float _WaveOffset;

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata_base v)
            {
                v2f o;
                o.uv = TRANSFORM_TEX(v.texcoord, _ViewTex);
                o.pos = UnityObjectToClipPos(v.vertex); // 模型到裁剪


                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                i.uv.x += _DistortionScale * sin(_Time.y + i.uv.y * _WaveOffset) * 0.01;
                // 把相机渲染模式改成只渲染深度, 然后clip一下就行, 因为clip是小于0才clip, 所以还要减一个很小的值
                i.uv.y = 1 - i.uv.y;
                clip(tex2D(_ViewTex, i.uv).a - 0.001);
                return tex2D(_ViewTex, float2(i.uv.x, i.uv.y));
            }
            ENDCG
        }
    }
    Fallback "Diffuse"
}