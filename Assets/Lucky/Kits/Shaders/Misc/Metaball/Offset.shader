// 混合四个角的颜色
Shader "Lucky/Misc/Offset"
{
    Properties
    {
        _MainTex ("", 2D) = "white" {}
        _OverlayTex("", 2D) = "white" {}
    }
    CGINCLUDE
    #include "UnityCG.cginc"

    struct v2f
    {
        float4 pos : SV_POSITION;
        half2 uv : TEXCOORD0;
    };

    sampler2D _MainTex;
    half4 _MainTex_TexelSize;
    sampler2D _OverlayTex;

    half4 _Offset;

    v2f vert(appdata_img v)
    {
        v2f o;
        o.pos = UnityObjectToClipPos(v.vertex);
        o.uv = v.texcoord;
        return o;
    }

    half4 frag(v2f i) : SV_Target
    {
        float4 color = tex2D(_MainTex, i.uv);
        color += tex2D(_OverlayTex, i.uv - _Offset);
        return color;
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