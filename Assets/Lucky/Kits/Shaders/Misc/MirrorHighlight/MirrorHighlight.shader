Shader "Mine/MirrorHighlight"
{
    Properties
    {
        _MainTex("MainTexture", 2D) = "white"{}
        _Color ("Color", Color) = (1,1,1,1)
        _K ("K", float) = 1
        _StrokeWidth ("StrokeWidth", float) = 1
        _Speed ("Speed", float) = 0.8
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
        }
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            #include <UnityCG.cginc>
            #pragma vertex vert
            #pragma fragment frag

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _K;
            float _StrokeWidth;
            float _Speed;

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
                float4 color = tex2D(_MainTex, i.uv);
                if (color.a == 0)
                    discard;
                /*
                 y = k*(x - uv.x) + uv.y
                 当y=0
                 x = -uv.y / k + uv.x
                 */
                float leftBottom;
                if (_K >= 0)
                    // [-(lineWith + 1 / _K), 1]
                    leftBottom = fmod(_Time.y * _Speed, 1 + 1 / _K + _StrokeWidth) - (_StrokeWidth + 1 / _K);
                else
                    // [-lineWith, 1 - 1 / _K]
                    leftBottom = fmod(_Time.y * _Speed, 1 + _StrokeWidth - 1 / _K) - _StrokeWidth;
                float rightBottom = leftBottom + _StrokeWidth;
                // 当前uv位置根据k算出与底部相交时x的值
                float x = -i.uv.y / _K + i.uv.x;
                if (leftBottom <= x && x <= rightBottom) // 这里逻辑判断一不小心又写成python那样的了
                    return 1;
                return color;
            }
            ENDCG
        }
    }
    Fallback "Diffuse"
}