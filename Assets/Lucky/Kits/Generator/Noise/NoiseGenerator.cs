using Lucky.Kits.Utilities;
using UnityEngine;

namespace Lucky.Kits.Generator.Noise
{
    public abstract class NoiseGenerator : MonoBehaviour
    {
        public bool isDebug; 
        [Range(1, 2048)] public int width = 64;
        [Range(1, 2048)] public int height = 64;
        private const string RootPath = "Assets/Lucky/Kits/Generator/Noise/Textures/";
        protected abstract string FileName { get; }
        protected abstract Color GetPixelColor(int x, int y);

        protected virtual void Awake()
        {
            Texture2D texture = new Texture2D(width, height, TextureFormat.RGB24, false);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    texture.SetPixel(x, y, GetPixelColor(x, y));
                }
            }

            texture.Apply();

            ResourcesUtils.SaveTextureToAssets(RootPath, FileName, texture);
        }
    }
}