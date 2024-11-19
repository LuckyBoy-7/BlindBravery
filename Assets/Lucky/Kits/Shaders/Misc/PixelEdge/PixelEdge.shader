Shader "Mine/PixelEdge"
{
    Properties
    {
        [Header(General)]
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Progress ("Progress", Range(0.0,1.0)) = 0
        _Transparent ("Transparent",Range(0.0,1.0)) = 0.2
        [Header(Rotation)]
        _RotationDegree ("RotationDegree", Range(0,360)) = 0
        _RotationPivotX ("RotationPivotX",Range(0,1)) = 0.5
        _RotationPivotY ("RotationPivotY",Range(0,1)) = 0.5
        [Header(Edge)]
        [Toggle]_EnableOutline ("EnableOutline", Int) = 0
        _EdgeColor ("EdgeColor",Color) = (0,0,0,1)
        _EdgeSize ("EdgeSize",Range(0.01, 10)) = 0.1

        [Header(Stentil)]
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15

    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 uv : TEXCOORD0;
            };


            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float4 _MainTex_ST;
            fixed4 _Color;

            v2f vert(appdata_img v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }


            float _RotationDegree;
            float _RotationPivotX;
            float _RotationPivotY;
            float _EdgeSize;
            float4 _EdgeColor;
            int _EnableOutline;
            float _Progress;
            float _Transparent;
            fixed4 frag(v2f i) : SV_Target
            {
                float deg = _RotationDegree / 180 * 3.1415926;
                float2x2 rotationMatrix = float2x2( // 行优先
                    cos(deg), -sin(deg),
                    sin(deg), cos(deg)
                );
                float2 pivotPos = float2(_RotationPivotX, _RotationPivotY);
                // 为什么这里要左乘才能对应度数增大逆时针转呢, 因为uv是反着来的, 它需要顺时针采样, 渲染出来才是逆时针
                float2 nuv = mul(i.uv - pivotPos, rotationMatrix) + pivotPos;

                float delta = _EdgeSize / _MainTex_TexelSize.z;  // 这样应该会导致竖直方向采样不是很均衡
                float4 colSam1 = tex2D(_MainTex, nuv + float2(-delta, -delta));
                float4 colSam2 = tex2D(_MainTex, nuv + float2(-delta, 0));
                float4 colSam3 = tex2D(_MainTex, nuv + float2(-delta, delta));
                float4 colSam4 = tex2D(_MainTex, nuv + float2(0, -delta));
                float4 colSam5 = tex2D(_MainTex, nuv + float2(0, 0));
                float4 colSam6 = tex2D(_MainTex, nuv + float2(0, delta));
                float4 colSam7 = tex2D(_MainTex, nuv + float2(delta, -delta));
                float4 colSam8 = tex2D(_MainTex, nuv + float2(delta, 0));
                float4 colSam9 = tex2D(_MainTex, nuv + float2(delta, delta));

                fixed4 color = tex2D(_MainTex, nuv);
                float4 averageColor = (colSam1 + colSam2 + colSam3 + colSam4 + colSam5 + colSam6 + colSam7 + colSam8 +
                    colSam9 + color) / 10.0;

                float4 ans = color * _Color;
                if (_EnableOutline)
                {
                    // 图片是从内到外不断变透明的, 也就是边缘有blur
                    ans.a = step(0.1, averageColor.a);

                    // 是否这个点的透明度很小且周围有透明度较大的点
                    float isOutline = step(color.a, averageColor.a) * step(color.a, 0.1);
                    ans.rgb = isOutline * _EdgeColor.rgb + (1 - isOutline) * color.rgb;
                }

                // 超过progress的部分要降低一定深度（透明度）
                ans.a = max(0, ans.a - step(_Progress, i.uv.x) * _Transparent);
                return ans;
            }
            ENDCG
        }
    }
}