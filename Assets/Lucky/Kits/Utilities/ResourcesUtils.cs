using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Lucky.Kits.Utilities
{
    public class ResourcesUtils
    {
        private static void print(object o) => Debug.Log(o);
        private const string SpriteRootPath = "Art/Sprites/";
        public static T Load<T>(string path) where T : Object => Resources.Load<T>(path);
        public static T[] LoadAll<T>(string path) where T : Object => Resources.LoadAll<T>(path);

        public static Sprite GetAtlasSubtexturesAt(string path, int idx)
        {
            return LoadSubtextures(path)[idx];
        }

        public static List<Sprite> LoadSubtextures(string path, int maxLength = 2)
        {
            path = Path.Combine(SpriteRootPath, path);
            return LoadAll<Sprite>(path).ToList();
        }

        public static void SaveTextureToAssets(string path, string name, Texture2D texture)
        {
            // 检查文件夹
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            
            // 写入纹理
            File.WriteAllBytes(path + name, texture.EncodeToPNG());
            // 刷新Asset
            // AssetDatabase.Refresh();
        }
    }
}