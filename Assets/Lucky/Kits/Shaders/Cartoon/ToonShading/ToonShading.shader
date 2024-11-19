// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Lucky/Cartoon/ToonShading"
{
    Properties
    {
        _Color ("Color Tint", Color) = (1, 1, 1, 1)
        _MainTex ("Main Tex", 2D) = "white" {}
        _Ramp ("Ramp Texture", 2D) = "white" {}
        _Outline ("Outline", Range(0, 1)) = 0.1
        _OutlineColor ("Outline Color", Color) = (0, 0, 0, 1)
        _Specular ("Specular", Color) = (1, 1, 1, 1)
        //        _SpecularScale ("Specular Scale", Range(0, 0.1)) = 0.01
        _SpecularScale ("Specular Scale", Range(0, 10)) = 0.01
    }
    SubShader
    {
        // 大概原理就是先渲染背面面片的轮廓，然后把轮廓向后移动到正面
        // 正面就是正常渲染就行
        Tags
        {
            "RenderType"="Opaque" "Queue"="Geometry"
        }

        Pass
        {
            NAME "OUTLINE"

            Cull Front

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            float _Outline;
            fixed4 _OutlineColor;

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            v2f vert(appdata_base v)
            {
                v2f o;

                float4 pos = mul(UNITY_MATRIX_MV, v.vertex);
                float3 normal = mul((float3x3)UNITY_MATRIX_IT_MV, v.normal); // 逆转置矩阵
                // 过程有点像是把整个模型放大一点，然后涂黑，弄成半透明最后再向摄像机移动一点的感觉
                normal.z = -0.5; // 朝我们人眼看的反方向缩放到固定常数，减少轮廓线遮挡正面面片的可能性
                pos = pos + float4(normalize(normal), 0) * _Outline;
                o.pos = mul(UNITY_MATRIX_P, pos);

                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                return float4(_OutlineColor.rgb, 1);
            }
            ENDCG
        }

        Pass
        {
            Tags
            {
                "LightMode"="ForwardBase"
            }

            Cull Back

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile_fwdbase

            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"
            #include "UnityShaderVariables.cginc"

            fixed4 _Color;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _Ramp;
            fixed4 _Specular;
            fixed _SpecularScale;

            struct v2f
            {
                float4 pos : POSITION;
                float2 uv : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
                SHADOW_COORDS(3)
            };

            v2f vert(appdata_tan v)
            {
                v2f o;

                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

                TRANSFER_SHADOW(o);

                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                fixed3 worldNormal = normalize(i.worldNormal);
                fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
                fixed3 worldViewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
                fixed3 worldHalfDir = normalize(worldLightDir + worldViewDir);

                fixed4 c = tex2D(_MainTex, i.uv);
                fixed3 albedo = c.rgb * _Color.rgb;

                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;

                UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);

                fixed diff = dot(worldNormal, worldLightDir);
                diff = (diff * 0.5 + 0.5) * atten;

                fixed3 diffuse = _LightColor0.rgb * albedo * tex2D(_Ramp, diff.rr).rgb;

                fixed spec = dot(worldNormal, worldHalfDir);
                // GPU在光栅化的时候一般以2x2的像素块为单位并行执行的, 所以它是能访问到相邻var数据的(应该吧)
                // 所以ddx（var），拿的是水平方向上var的差值
                // 所以ddy（var），拿的是垂直方向上var的差值
                // fwidth = abs(ddx(a) + ddy(a))
                // fixed w = fwidth(spec) * 2.0;  // spec -> [-1, 1], ddx(spec) -> [-2, 2], fwidth -> [0, 4]
                //
                // < -w -> 0, > w -> 1, 之间的插值
                // 在高光区域内，w -> 0, spec -> 1
                // 在高光边界处(大色块棱角处)，w较大 -> [0, 4]，此时插值
                // 在高光外, w -> 0，spec < 0 或 < w(总之就是很小)，lerp后值接近为0
                float w = fwidth(spec) * 2;  // *2我也不知道为什么，可能只是单纯的缩放吧
                // fixed3 specular = _Specular.rgb * lerp(0, 1, smoothstep(-w, w, spec + _SpecularScale - 1)) * step(0.0001, _SpecularScale);
                fixed3 specular = _Specular.rgb * smoothstep(-w, w, spec + _SpecularScale - 1) * step(0.0001, _SpecularScale);

                // 用0和0.5表现出来就类似普通的高光反射，少了卡通化的风格
                // 用fwidth就能让高光区域内纯色，边缘处过渡，外面无色
                // fixed3 specular = _Specular.rgb * lerp(0, 1, smoothstep(0, 0.5, spec + _SpecularScale - 1)) * step(0.0001, _SpecularScale);


                return fixed4(ambient + diffuse + specular, 1.0);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}