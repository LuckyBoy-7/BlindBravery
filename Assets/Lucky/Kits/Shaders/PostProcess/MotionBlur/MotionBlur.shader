// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Lucky/PostProcess/MotionBlur"
{
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_BlurAmount ("Blur Amount", Float) = 1.0
	}
	SubShader {
		CGINCLUDE
		
		#include "UnityCG.cginc"
		
		sampler2D _MainTex;
		fixed _BlurAmount;
		
		struct v2f {
			float4 pos : SV_POSITION;
			half2 uv : TEXCOORD0;
		};
		
		v2f vert(appdata_img v) {
			v2f o;
			
			o.pos = UnityObjectToClipPos(v.vertex);
			
			o.uv = v.texcoord;
					 
			return o;
		}
		
		fixed4 fragRGB (v2f i) : SV_Target {
			return fixed4(tex2D(_MainTex, i.uv).rgb, 1 - _BlurAmount);
		}
		
		half4 fragA (v2f i) : SV_Target {
			return tex2D(_MainTex, i.uv);
		}
		
		ENDCG
		
		ZTest Always 
		Cull Off 
		ZWrite Off
		
		Pass {
			Blend SrcAlpha OneMinusSrcAlpha  // 因为默认是覆盖，所以如果把这行注释了就没有模糊了，这里SrcAlpha就是1 - blurAmount
			ColorMask RGB // 这里如果把alpha注释了，并且把cs里一开始src->accumulate的那行注释了，那么图片是先半透明然后正常（因为图片效果叠加了） 
			
			CGPROGRAM
			// 相当于第一次是拿当前屏幕图像*blurAmount和以前的图像混合，也能保证越之前的残影会越淡
			#pragma vertex vert  
			#pragma fragment fragRGB  
			ENDCG
		}
		
//		Pass {    // 这个pass关了好像没什么大问题？因为本案例中alpha一直是1，不过这样的话前面不设置rgb就错了，因为前面也修改了alpha通道, 可能渲染半透明图像还是有点问题的, 不过反正透明度用的是src材质的, 所以好像也没问题
//			Blend One Zero  
//			ColorMask A
//			   	
//			CGPROGRAM  
//
//			// 相当于第二次是正常渲染当前屏幕图像
//			#pragma vertex vert  
//			#pragma fragment fragA
//			  
//			ENDCG
//		}
	}
 	FallBack Off
}
