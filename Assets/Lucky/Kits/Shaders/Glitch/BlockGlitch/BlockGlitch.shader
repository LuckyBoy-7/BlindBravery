Shader "Lucky/Glitch/BlockGlitch"
{
    Properties
    {
        _MainTex ("MainTex", 2D) = "white" { }
    }
    SubShader
    {
        // 哦，因为是基于屏幕纹理渲染的，所以不需要搞blend
        ZTest Always Cull Off ZWrite Off
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            sampler2D _MainTex;
            float _Intensity;
            float2 _BlockUV;
            float _BlockIntensity;
            float _RGBSplitIntensity;


            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float randomNoise(float2 seed)
            {
                return frac(sin(dot(seed * floor(_Time.y * 30.0), float2(127.1, 311.7))) * 43758.5453123);
            }

            float randomNoise(float seed)
            {
                return randomNoise(float2(seed, 1.0));
            }

            float4 ImageBlock(float2 uv, float intensity, float2 blockUV, float blockIntensity,
                              float _RGBSplitIntensity)
            {
                float2 blockLayer1 = floor(uv * blockUV);  // 1 / blockUV为一个单位块的大小, blockLayer表示对应哪个块, 我说为什么intensity没用呢, 原来是blockUV太小了, 结果一直是0
                float lineNoise = pow(randomNoise(blockLayer1), blockIntensity);  // 根据块的位置拿到noise, 并根据1 / blockIntensity调整noise
                float RGBSplitNoise = pow(randomNoise(5.1379), 7.1) * _RGBSplitIntensity;  // 偏移程度 
                lineNoise = lineNoise * intensity - RGBSplitNoise;  // 最后的偏移噪声
                float4 colorR = tex2D(_MainTex, uv);
                float4 colorG = tex2D(_MainTex, uv + float2(lineNoise * 0.05 * randomNoise(5), 0));
                float4 colorB = tex2D(_MainTex, uv - float2(lineNoise * 0.05 * randomNoise(31), 0));
                float4 result = float4(float3(colorR.r, colorG.g, colorB.b), colorR.a + colorG.a + colorB.a);
                return result;
            }

            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float4 color = ImageBlock(i.uv, _Intensity, _BlockUV, _BlockIntensity, _RGBSplitIntensity);
                return color;
            }
            ENDCG
        }
    }
}