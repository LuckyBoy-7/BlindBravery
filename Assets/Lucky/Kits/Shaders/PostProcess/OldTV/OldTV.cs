using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Lucky.Shaders.PostProcess
{

    public class OldTV : PostEffectsBase
    {
        [Range(0, 1)] public float Expand = 0.7f;
        [Range(0, 1)] public float NoiseIntensity = 0.3f;
        public int StripeIntensity = 500;

        protected void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            MainMaterial.SetFloat("_Expand", Expand);
            MainMaterial.SetFloat("_NoiseIntensity", NoiseIntensity);
            MainMaterial.SetInt("_StripeIntensity", StripeIntensity);
            Graphics.Blit(source, destination, MainMaterial, 0);
        }
    }
}