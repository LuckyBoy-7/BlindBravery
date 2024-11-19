Shader "Lucky/PostProcess/ScreenMask"
{

    Properties
    {
        _MainTex("", Any) = "white"{}
    }

    SubShader
    {
        ZTest Always Cull Off ZWrite Off
        Pass
        {
            CGPROGRAM
            #include "UnityCG.cginc"
            #pragma vertex vert
            #pragma fragment frag


            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Radius;
            float _BlurAmount;
            float2 _Center;

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
                // 中间到当前uv的向量
                // 正圆, 相当于把1:1的纹理映射到16:9的纹理上, 因为这里保持宽为1, 所以半径0.5正好对应水平方向占满, 竖直方向溢出
                float2 vec = (i.uv - _Center) * float2(1, 9.0 / 16);
                // 等比圆
                // float2 vec = i.uv - _Center;
                // 当前uv到中间的距离的平方
                float d2 = dot(vec, vec);
                float alpha = smoothstep(pow(_Radius + _BlurAmount, 2), pow(_Radius, 2), d2);
                return tex2D(_MainTex, i.uv) * alpha;
            }
            ENDCG


        }
    }

    FallBack off
}