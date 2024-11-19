Shader "Lucky/BasicTexture/SingleTexture"
{

    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)
        _MainTex("MainTex", 2D) = "white" {}
        _DiffuseColor("DiffuseColor", Color) = (1, 1, 1, 1)
        _HalfLambertFactor("HalfLambertFactor", Range(0, 1)) = 0.5

        _SpecularColor("SpecularColor", Color) = (1, 1, 1, 1)
        _Gloss("Gloss", Range(0, 10)) = 0.1
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
            sampler2D _MainTex;
            float4 _MainTex_ST;

            fixed3 _DiffuseColor;
            fixed _HalfLambertFactor;

            fixed3 _SpecularColor;
            fixed _Gloss;

            struct v2f
            {
                float4 vertex: SV_POSITION;
                fixed3 worldPosition: TEXCOORD0;
                fixed3 worldNormal: TEXCOORD1;
                fixed2 uv: TEXCOORD2;
            };

            v2f vert(appdata_base i)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(i.vertex);
                o.worldPosition = mul(UNITY_MATRIX_M, i.vertex);
                o.worldNormal = UnityObjectToWorldNormal(i.normal);
                o.uv = TRANSFORM_TEX(i.texcoord, _MainTex);

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // albedo
                fixed3 albedo = tex2D(_MainTex, i.uv);
                // 环境光
                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo * _Color;

                // half lambert光照模型, 可以使物体背面没被光照到的地方也有染上类似的光照颜色颜色, 模拟漫反射
                float3 lightDir = normalize(UnityWorldSpaceLightDir(i.worldPosition));
                float3 normal = i.worldNormal;
                fixed3 diffuseColor = _LightColor0.xyz * _DiffuseColor * max(0, dot(lightDir, normal) * _HalfLambertFactor + _HalfLambertFactor);

                // specular color
                // float3 reflectedLightToCameraDir = normalize(reflect(-lightDir, normal));
                float3 vertexToCameraDir = normalize(UnityWorldSpaceViewDir(i.worldPosition));
                // 之前是拿反射光和眼睛方向的夹角比, 现在是先把这入射光和眼睛方向中和一下再和法线比
                float3 combinedDir = normalize(lightDir + vertexToCameraDir);
                fixed3 specularColor = _LightColor0.xyz * _SpecularColor * pow(max(0, dot(combinedDir, i.worldNormal)), _Gloss);

                fixed3 color = ambient + diffuseColor + specularColor;

                return fixed4(color, 1);
            }
            ENDCG
        }
    }
    Fallback "Specular"
}