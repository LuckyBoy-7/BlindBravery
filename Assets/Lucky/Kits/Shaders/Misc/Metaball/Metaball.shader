/*
所以以我的理解metaball的原理就是两个朦胧的球相交处不是那么朦胧，于是就联通了。
过程大概是这样的：
先把圆用相机渲染到quad上，然后后处理把圆blur一下
然后圆本身渲染的时候又是在quad（metaball shader在quad上）上面采样的，同时内部alpha值大，可做填充，边缘处alpha值较小可以做描边。

这样的话圆的逻辑可以不变，只是显示方式改了
由于这俩是分开的，所以如果渲染要改图层的话，quad的z轴要改一下
*/

Shader "Mine/Metaballs"
{
    Properties
    {
        _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
        _Color ("Main color", Color) = (1,1,1,1)

        // 感觉这里的cutoff主要还是把quad空白地方舍弃掉
        _Cutoff ("Alpha cutoff", Range(0,1)) = 0.5

        _StrokeAlpha ("Stroke alpha", Range(0,1)) = 0.1
        _StrokeColor ("Stroke color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags
        {
            "Queue"="AlphaTest"
            "IgnoreProjector"="True"
            "RenderType"="TransparentCutout"
        }
        LOD 100
        Lighting Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct v2f
            {
                float4 vertex : SV_POSITION;
                half2 texcoord : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed _Cutoff;

            half4 _Color;

            fixed _StrokeAlpha;
            half4 _StrokeColor;

            v2f vert(appdata_base v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.texcoord);
                clip(col.a - _Cutoff);
                col = col.a < _StrokeAlpha ? _StrokeColor : _Color;

                return col;
            }
            ENDCG
        }
    }

}