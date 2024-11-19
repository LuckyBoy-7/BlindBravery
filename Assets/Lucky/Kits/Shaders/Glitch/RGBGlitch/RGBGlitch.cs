using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lucky.Shaders.PostProcess
{

    public class RGBGlitch : PostEffectsBase
    {
        public float _Intensity;
        public float _Frequency;
        public float _Speed;
        public float _Exponent;

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            MainMaterial.SetFloat("_Intensity", _Intensity);
            MainMaterial.SetFloat("_Frequency", _Frequency);
            MainMaterial.SetFloat("_Speed", _Speed);
            MainMaterial.SetFloat("_Exponent", _Exponent);

            Graphics.Blit(source, destination, MainMaterial);
        }
    }
}