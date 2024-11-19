Shader "Lucky/Lighting1/SpecularVertexLevel"
{

    Properties
    {
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

            fixed3 _DiffuseColor;
            fixed _HalfLambertFactor;

            fixed3 _SpecularColor;
            fixed _Gloss;

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

                // half lambert光照模型, 可以使物体背面没被光照到的地方也有染上类似的光照颜色颜色, 模拟漫反射
                float3 lightDir = normalize(ObjSpaceLightDir(i.vertex));
                float3 normal = UnityObjectToWorldNormal(i.normal);
                fixed3 diffuseColor = _LightColor0.xyz * _DiffuseColor * max(0, dot(lightDir, normal) * _HalfLambertFactor + _HalfLambertFactor);

                // specular color
                float3 lightToCameraDir = normalize(reflect(-lightDir, normal));
                float3 vertexToCameraDir = normalize(WorldSpaceViewDir(i.vertex));
                // Gloss本意是光泽度, 可以理解为反射光的细腻程度, 因为gloss越大, 那就能把值映射到更加靠0的位置(指数函数嘛), 所以值离得更近, 也就更细腻
                // 如果max里写0则, 则当两向量夹角>90度时, pow(0, 0)会直接出错(因为0不能pow), 从而渲染出纯黑面片, 也不知道它pow怎么实现的难道是自己除以自己?
                // pow的实现应该是exp(y * log(x)), 所以log(0)会崩
                fixed3 specularColor = _LightColor0.xyz * _SpecularColor * pow(max(0.0001, dot(lightToCameraDir, vertexToCameraDir)), _Gloss);

                o.color = ambient + diffuseColor + specularColor;
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