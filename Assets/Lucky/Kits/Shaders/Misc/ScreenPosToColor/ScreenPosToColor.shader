//// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'
//
//Shader "Lucky/Misc/ScreenPosToColor"
//{
//    SubShader
//    {
//        Pass
//        {
//            CGPROGRAM
//            #include <UnityCG.cginc>
//            
//            #pragma vertex vert
//            #pragma fragment frag
//
//            struct v2f
//            {
//                float4 position: POSITION;
//            };
//
//            v2f vert(appdata_base i)
//            {
//                v2f o;
//                o.position = UnityObjectToClipPos(i.vertex);
//                return o;
//            }
//
//            fixed4 frag(float4 i: VPOS) : SV_Target
//            {
//                return fixed4(i.xy / _ScreenParams.xy, 0, 1);
//            }
//            ENDCG
//        }
//    }
//}
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Lucky/Misc/ScreenPosToColor"
{
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #include <UnityCG.cginc>

            #pragma vertex vert
            #pragma fragment frag

            struct v2f
            {
                float4 position: SV_POSITION;
                float4 screenPos: TEXCOORD0;
            };

            v2f vert(appdata_base i)
            {
                v2f o;
                o.position = UnityObjectToClipPos(i.vertex);
                o.screenPos = ComputeScreenPos(o.position);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return fixed4(i.screenPos.xy / i.screenPos.w, 0, 1);
            }
            ENDCG
        }
    }
}