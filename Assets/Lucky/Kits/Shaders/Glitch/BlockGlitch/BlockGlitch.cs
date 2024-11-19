using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lucky.Shaders.PostProcess
{

    public class BlockGlitch : PostEffectsBase
    {
        public float _Intensity;
        public Vector2 _BlockUV;
        public float _BlockIntensity;
        public float _RGBSplitIntensity;

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            MainMaterial.SetFloat("_Intensity", _Intensity);
            MainMaterial.SetFloat("_BlockIntensity", _BlockIntensity);
            MainMaterial.SetFloat("_RGBSplitIntensity", _RGBSplitIntensity);
            MainMaterial.SetVector("_BlockUV", _BlockUV);

            Graphics.Blit(source, destination, MainMaterial);
        }
    }
}