// 混合四个角的颜色
Shader "Mine/BlurEffectConeTap"
{
    Properties
    {
        _MainTex ("", 2D) = "white" {}
    }
    CGINCLUDE
    #include "UnityCG.cginc"

    struct v2f
    {
        float4 pos : SV_POSITION;
        half2 uv : TEXCOORD0;
        half2 taps[4] : TEXCOORD1;
    };

    sampler2D _MainTex;
    half4 _MainTex_TexelSize;

    // _BlueOffsets应该是由Graphics.BlitMultiTap里的offset参数传进来的
    half4 _BlurOffsets;

    v2f vert(appdata_img v)
    {
        v2f o;
        o.pos = UnityObjectToClipPos(v.vertex);
        o.uv = v.texcoord - _BlurOffsets.xy * _MainTex_TexelSize.xy;
        // hack, see BlurEffect.cs for the reason for this. let's make a new blur effect soon
        o.taps[0] = o.uv + _MainTex_TexelSize.xy * _BlurOffsets.xy;  // 右上
        o.taps[1] = o.uv - _MainTex_TexelSize.xy * _BlurOffsets.xy;  // 左下
        o.taps[2] = o.uv + _MainTex_TexelSize.xy * _BlurOffsets.xy * half2(1, -1);  // 右下
        o.taps[3] = o.uv - _MainTex_TexelSize.xy * _BlurOffsets.xy * half2(1, -1);  // 左上
        return o;
    }

    half4 frag(v2f i) : SV_Target
    {
        half4 color = tex2D(_MainTex, i.taps[0]);
        color += tex2D(_MainTex, i.taps[1]);
        color += tex2D(_MainTex, i.taps[2]);
        color += tex2D(_MainTex, i.taps[3]);
        return color * 0.25;
    }
    ENDCG

    SubShader
    {
        Pass
        {
            ZTest Always Cull Off ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            ENDCG
        }
    }
    Fallback off
}