using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Mathf;


/// <summary>
/// 酷毙啦
/// </summary>
public class GlassRGBTextureGenerator : MonoBehaviour
{
    public int textureSizeExponent = 6;
    public int textureSize => (int)Pow(2, textureSizeExponent);

    void Start()
    {
        Texture2D texture = new Texture2D(textureSize, textureSize, TextureFormat.ARGB32, false);
        //对于.ARGB32
        for (int i = 0; i < textureSize; i++)
        {
            for (int j = 0; j < textureSize; j++)
            {
                var (uvI, uvJ) = (i * 1.0f / textureSize, j * 1.0f / textureSize);
                float r = RandomNoise(new Vector2(uvI, uvJ));
                float g = RandomNoise(new Vector2(uvI + 1, uvJ));
                float b = RandomNoise(new Vector2(uvI, uvJ + 1));
                print(r);
                texture.SetPixel(i, j, new Color(r, g, b, 1));
            }
        }

        // string file = "./Assets/GlassRGBTexture.png"; //路径记得要修改！注意该路径斜线是向左的，和电脑里文件将爱路径相反
        // byte[] bytes = texture.EncodeToPNG();
        // Destroy(texture);
        // System.IO.File.WriteAllBytes(file, bytes);
        // UnityEditor.AssetDatabase.Refresh(); //自动刷新资源，相当于ctrl + r reload
    }

    float RandomNoise(Vector2 seed)
    {
        return Frac(Sin(Vector2.Dot(seed, new Vector2(127.1f, 311.7f))) *
                    43758.5453123f);

        float Frac(float v)
        {
            return v - Floor(v);
        }
    }
}