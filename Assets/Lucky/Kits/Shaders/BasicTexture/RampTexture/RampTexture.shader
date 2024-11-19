Shader "Lucky/BasicTexture/RampTexture"
{

    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)
        _RampTex("RampTex", 2D) = "white" {}
    }
    SubShader
    {
        Pass
        {
            Tags
            {
                // 只有定义了正确的 LightMode,我们才能得到 Unity 的内置光照变量，的_LightColorO
                "LightMode" = "ForwardBase"
            }
            CGPROGRAM
            #include <UnityCG.cginc>
            #include <Lighting.cginc>

            #pragma vertex vert
            #pragma fragment frag

            fixed3 _Color;
            sampler2D _RampTex;
            float4 _RampTex_ST;

            struct v2f
            {
                float4 vertex: SV_POSITION;
                float4 worldPosition: TEXCOORD0;
                float3 worldNormal: TEXCOORD1;
                // fixed2 uv: TEXCOORD2;
            };

            v2f vert(appdata_base i)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(i.vertex);
                o.worldPosition = mul(UNITY_MATRIX_M, i.vertex);
                o.worldNormal = UnityObjectToWorldNormal(i.normal);
                // o.uv = TRANSFORM_TEX(i.texcoord, _RampTex);

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float3 lightDir = normalize(UnityWorldSpaceLightDir(i.worldPosition));
                float3 normal = normalize(i.worldNormal);
                fixed zeroToOne = dot(lightDir, normal) * 0.5f + 0.5f;
                // albedo
                fixed3 albedo = tex2D(_RampTex, float2(zeroToOne, 0));
                // 环境光
                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo * _Color;

                fixed3 diffuseColor = _LightColor0.xyz * albedo;


                fixed3 color = ambient + diffuseColor;

                return fixed4(color, 1);
            }
            ENDCG
        }
    }
    Fallback "Specular"
}