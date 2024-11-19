using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lucky.Shaders.PostProcess
{

    public class TileJitter : PostEffectsBase
    {
        public float _Count;
        public float _Intensity;
        public float _Speed;
        public float _Frequency;
        public int isJitterDirectionHorizontal;

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            MainMaterial.SetFloat("_Count", _Count);
            MainMaterial.SetFloat("_Intensity", _Intensity);
            MainMaterial.SetFloat("_Speed", _Speed);
            MainMaterial.SetFloat("_Frequency", _Frequency);
            MainMaterial.SetInt("isJitterDirectionHorizontal", isJitterDirectionHorizontal);

            Graphics.Blit(source, destination, MainMaterial);
        }
    }
}