Shader "Lucky/BasicTexture/NormalMapWorldSpace"
{

    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)
        _MainTex("MainTex", 2D) = "white" {}
        _BumpTex("BumpTex", 2D) = "bump" {}
        _BumpScale("BumpTex", Range(-10, 10)) = 1


        _DiffuseColor("DiffuseColor", Color) = (1, 1, 1, 1)

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
            sampler2D _BumpTex;
            float4 _MainTex_ST;
            float4 _BumpTex_ST;

            float _BumpScale;
            fixed3 _DiffuseColor;

            fixed3 _SpecularColor;
            fixed _Gloss;

            struct v2f
            {
                float4 vertex: SV_POSITION;
                fixed3 worldPosition: TEXCOORD0;
                fixed4 uv: TEXCOORD1;
                float3 W2T0: TEXCOORD2;
                float3 W2T1: TEXCOORD3;
                float3 W2T2: TEXCOORD4;
            };

            v2f vert(appdata_tan i)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(i.vertex);
                o.worldPosition = mul(UNITY_MATRIX_M, i.vertex);
                o.uv.xy = TRANSFORM_TEX(i.texcoord, _MainTex);
                o.uv.zw = TRANSFORM_TEX(i.texcoord, _BumpTex);

                float3 tangent = normalize(mul(UNITY_MATRIX_M, i.tangent.xyz)); // 这里不屑xyz会有问题吗
                float3 normal = normalize(UnityObjectToWorldNormal(i.normal));
                float3 binormal = cross(normal, tangent) * i.tangent.w;
                // 顺序为切线, 副切线, 法线(这应该是约定俗成的规则, 刚好跟凹凸纹理上的对应)
                o.W2T0 = tangent;
                o.W2T1 = binormal;
                o.W2T2 = normal;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // albedo
                fixed3 albedo = tex2D(_MainTex, i.uv.xy);

                // 凹凸法线纹理上的颜色对应的法线
                float3x3 worldToTangentMatrix = float3x3(i.W2T0, i.W2T1, i.W2T2);
                float3 bumpedNormal = UnpackNormal(tex2D(_BumpTex, i.uv.zw));
                bumpedNormal.xy *= _BumpScale;
                bumpedNormal.z = sqrt(1 - saturate(length(bumpedNormal.xy)));
                // 世界空间下了
                bumpedNormal = mul(bumpedNormal, worldToTangentMatrix);

                // 环境光
                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo * _Color;

                // diffuse
                float3 worldViewDir = normalize(UnityWorldSpaceViewDir(i.vertex));
                float3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.vertex));
                fixed3 diffuseColor = _LightColor0.xyz * _DiffuseColor * albedo * (dot(worldLightDir, bumpedNormal) * 0.5f + 0.5f);

                // specular color
                float3 combinedDir = normalize(worldLightDir + worldViewDir);
                fixed3 specularColor = _LightColor0.xyz * _SpecularColor * pow(max(0, dot(combinedDir, bumpedNormal)), _Gloss);

                fixed3 color = ambient + diffuseColor + specularColor;

                return fixed4(color, 1);
            }
            ENDCG
        }
    }
    Fallback "Specular"
}