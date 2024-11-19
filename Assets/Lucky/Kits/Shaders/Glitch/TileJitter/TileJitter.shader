Shader "Lucky/Glitch/TileJitter"
{
    Properties
    {
        _MainTex ("MainTex", 2D) = "white" { }
    }
    SubShader
    {
        ZTest Always
        Cull Off
        ZWrite Off
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            sampler2D _MainTex;
            float _Count;
            float _Intensity;
            float _Speed;
            float _Frequency;
            int isJitterDirectionHorizontal;

            struct a2v
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };


            // 这个tile是一整条, 不是block
            float4 Horizontal(float2 uv, float count, float intensity, float speed, float frequency)
            {
                float strength = 0.5 + 0.5 * cos(_Time.y * frequency); // 拉伸程度
                float pixelSizeX = 1.0 / _ScreenParams.x; // 单位水平纹素
                if (fmod(uv.y * count, 2) < 1.0) // count本质上控制了tile的数量, < 1控制了错位(也就是隔一段有隔一段没有)
                {
                    // 本来这里是用JITTER_DIRECTION_HORIZONTAL 的if def来判断的，但好像没有效果，可能跟版本什么有关吧
                    // 上面本来定义的是  #pragma shader_feature JITTER_DIRECTION_HORIZONTAL
                    uv.x += pixelSizeX * cos(_Time.y * speed) * intensity * strength;
                }
                float4 sceneColor = tex2D(_MainTex, uv);
                return sceneColor;
            }

            float4 Vertical(float2 uv, float count, float intensity, float speed, float frequency)
            {
                float strength = 0.5 + 0.5 * cos(_Time.y * frequency);
                float pixelSizeY = 1.0 / _ScreenParams.y;
                if (fmod(uv.x * count, 2) < 1.0) // 让奇数位的图块错位
                {
                    uv.y += pixelSizeY * cos(_Time.y * speed) * intensity * strength;
                }
                float4 sceneColor = tex2D(_MainTex, uv);
                return sceneColor;
            }

            v2f vert(a2v v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float4 color;
                if (isJitterDirectionHorizontal)
                    color = Horizontal(i.uv, _Count, _Intensity, _Speed, _Frequency);
                else
                    color = Vertical(i.uv, _Count, _Intensity, _Speed, _Frequency);
                return color;
            }
            ENDCG
        }
    }
}