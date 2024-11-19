Shader "Lucky/Misc/WaterMirrorWithScreen"  // 根据屏幕的话不够灵活, 而且和屏幕边缘重合的时候效果不好, 还会突然消失(因为被剔除了)
{
    Properties
    {
        _MainTex("MainTexture", 2D) = "white"{}
        _Offset ("Offset", Vector) = (0,0,0,0)
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
        // 感觉grabPass好像抓取的是整个屏幕，然后采样的时候是取框选的部分，总之就是抓取了整个屏幕
        // 但为什么采样后不是像以前那样随着对象随访而缩放呢，那是因为采样的坐标我们也是在屏幕上转化为视口坐标再取的。
        // 同时要注意这种调试一定要在game窗口进行，scene窗口会因为窗口位置和大小变化而引发不一样的效果
        GrabPass
        {
            "_GrabTex"
        }
        Pass
        {
            CGPROGRAM
            #include <UnityCG.cginc>
            #pragma vertex vert
            #pragma fragment frag

            sampler2D _MainTex;
            sampler2D _GrabTex;
            float4 _MainTex_ST;
            float4 _GrabTex_ST;
            float4 _GrabTex_TexelSize;
            float4 _Offset;
            float _DistortionScale;
            float _WaveOffset;

            struct v2f
            {
                float4 pos : SV_POSITION;
                float4 grabPos : TEXCOORD0;
                float2 uv : TEXCOORD1;
            };

            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex); // 模型到裁剪
                o.grabPos = ComputeGrabScreenPos(o.pos); // 裁剪到屏幕，到时候记得/w
                // o.grabPos.xy  = -o.grabPos.xy;
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);

                v.vertex += _Offset;
                v.vertex.y *= -1;
                o.pos = UnityObjectToClipPos(v.vertex); // 模型到裁剪


                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float2 uv = i.grabPos / i.grabPos.w;
                uv.x += _DistortionScale * sin(_Time.y + i.grabPos.y * _WaveOffset) * 0.01;
                // 这里反色一下，清楚点
                return 1 - tex2D(_GrabTex, float2(uv.x, uv.y));


                // // 因为正交投影完后w就是1，所以这里其实直接采样也行
                // float2 uv = i.grabPos / i.grabPos.w;
                // return 1 - tex2D(_GrabTex, uv);
                // // 或
                // return 1 - tex2Dproj(_GrabTex, i.grabPos);
            }
            ENDCG
        }
    }
    Fallback "Diffuse"
}