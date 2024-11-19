using UnityEngine;
using System.Collections;
using Lucky.Shaders.PostProcess;

namespace Lucky.Shaders.PostProcess
{
    public class EdgeDetection : PostEffectsBase
    {
        [Range(0.0f, 1.0f)] public float edgesOnly = 0.0f;

        public Color edgeColor = Color.black;

        public Color backgroundColor = Color.white;

        void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            if (MainMaterial != null)
            {
                MainMaterial.SetFloat("_EdgeOnly", edgesOnly);
                MainMaterial.SetColor("_EdgeColor", edgeColor);
                MainMaterial.SetColor("_BackgroundColor", backgroundColor);

                Graphics.Blit(src, dest, MainMaterial);
            }
            else
            {
                Graphics.Blit(src, dest);
            }
        }
    }
}