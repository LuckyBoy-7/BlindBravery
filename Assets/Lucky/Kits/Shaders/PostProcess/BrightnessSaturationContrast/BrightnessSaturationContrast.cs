using UnityEngine;
using System.Collections;

namespace Lucky.Shaders.PostProcess
{
    public class BrightnessSaturationContrast : PostEffectsBase
    {

        [Range(0.0f, 3.0f)] public float brightness = 1.0f;

        [Range(0.0f, 3.0f)] public float saturation = 1.0f;

        [Range(0.0f, 3.0f)] public float contrast = 1.0f;

        void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            if (MainMaterial != null)
            {
                MainMaterial.SetFloat("_Brightness", brightness);
                MainMaterial.SetFloat("_Saturation", saturation);
                MainMaterial.SetFloat("_Contrast", contrast);

                Graphics.Blit(src, dest, MainMaterial);
            }
            else
            {
                Graphics.Blit(src, dest);
            }
        }
    }
}