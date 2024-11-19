Shader"Lucky/PostProcess/ScrollingWave"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
    }

    CGINCLUDE
    #include "UnityCG.cginc"

    sampler2D _MainTex;

    float _ScrollingSpeed; // 波纹向上移动的速度
    float _Amplitude; // 振幅
    float _WaveLength; // 波长


    float4 frag(v2f_img i) : SV_Target
    {
        // centerY -> [-waveLength / 4, 1 + waveLength / 4], [0, 1 + waveLength / 2]
        float centerY = frac(_Time.y * _ScrollingSpeed) * (1 + _WaveLength / 2) - _WaveLength / 4; // 波的最高点对应的y
        float bottomY = centerY - _WaveLength / 4;
        float topY = centerY + _WaveLength / 4;

        if (bottomY <= i.uv.y && i.uv.y <= topY)
        {
            // int flag =
            // if (i.uv.y < _WaveLength / 4) // 保证是个循环，而不是到边界就断掉
            // i.uv.y += 1;
            float dy = i.uv.y - bottomY;
            float delta = _Amplitude * sin(dy / _WaveLength * radians(360));
            return tex2D(_MainTex, float2(i.uv.x - delta, i.uv.y));
        }
        return tex2D(_MainTex, i.uv);
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