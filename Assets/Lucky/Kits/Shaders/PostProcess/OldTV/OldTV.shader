Shader "Lucky/PostProcess/OldTV"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
    }

    CGINCLUDE
    #include "UnityCG.cginc"

    sampler2D _MainTex;

    float _Expand;
    float _NoiseIntensity;
    int _StripeIntensity;

    float simpleNoise(float2 uv)
    {
        return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
    }


    half4 frag(v2f_img i) : SV_Target
    {
        // 中间到当前uv的向量
        half2 vec = i.uv - half2(0.5, 0.5);
        // 当前uv到中间的距离的平方
        float d2 = dot(vec, vec);
        // expand是指展开程度，越小，展开程度越小，画面越凸，越像老电视
        // (_Expand + d2 * (1 - _Expand))如果这玩意儿为1，那么很显然取的就是原来的uv坐标
        // 当1 > _Expand > 0, 很显然d2 <= 1，这个式子只可能<=1
        // uv都往内部采样了, 意味着整个画面相当于被放大，但是离中心越远的点影响越小，离中心越近的点影响越大，因为是平方
        // 当d2不变时，_Expand越大，d2的影响越小，所以看起来越正常
        half2 coord = vec * (_Expand + d2 * (1 - _Expand)) + half2(0.5, 0.5);
        half4 color = tex2D(_MainTex, coord);

        // 随机一个0~1的小数噪声，然后和原颜色混合一下
        float n = simpleNoise(coord.xy * _Time.x);
        half3 result = color.rgb * (1 - _NoiseIntensity) + _NoiseIntensity * n;

        // 中间的黑线
        // 根据坐标y和sin cos随机两个0~1的小数
        half2 sc = half2((sin(coord.y * _StripeIntensity) + 1) / 2, (cos(coord.y * _StripeIntensity) + 1) / 2);
        result += color.rgb * sc.xyx;  // 正片叠底？或者说只是单纯原纹理的某些通道在图片你上的叠加

        return half4(result, color.a);
    }
    ENDCG

    SubShader
    {
        ZTest Always Cull Off ZWrite Off
        Fog
        {
            Mode off
        }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            ENDCG
        }
    }

    FallBack off
}