Shader "Lucky/Lighting1/DiffuseVertexLevel"
{

    Properties
    {
        _DiffuseColor("DiffuseColor", Color) = (1, 1, 1, 1)
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

            fixed3 _DiffuseColor;

            struct v2f
            {
                float4 vertex: SV_POSITION;
                fixed3 color: TEXCOORD0;
            };

            v2f vert(appdata_base i)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(i.vertex);

                // 环境光
                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;

                // lambert光照模型
                float3 lightDir = normalize(ObjSpaceLightDir(i.vertex));
                float3 normal = UnityObjectToWorldNormal(i.normal);
                fixed3 diffuseColor = _LightColor0.xyz * _DiffuseColor * max(0, dot(lightDir, normal));

                o.color = ambient + diffuseColor;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return fixed4(i.color, 1);
            }
            ENDCG
        }
    }
    Fallback "Diffuse"
}