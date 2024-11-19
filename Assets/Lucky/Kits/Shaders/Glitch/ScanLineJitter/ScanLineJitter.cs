using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Lucky.Shaders.PostProcess
{

    public class ScanLineJitter : PostEffectsBase
    {
        public float _Intensity;
        public float _Frequency;
        public int _SegmentHeight = 10;

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            MainMaterial.SetFloat("_Intensity", _Intensity);
            MainMaterial.SetFloat("_Frequency", _Frequency);
            MainMaterial.SetInt("_SegmentHeight", _SegmentHeight);

            Graphics.Blit(source, destination, MainMaterial);
        }
    }
}