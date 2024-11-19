using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Lucky.Shaders.PostProcess
{

    public class ScrollingWave : PostEffectsBase
    {
        public float _ScrollingSpeed; // 波纹向上移动的速度
        public float _Amplitude; // 振幅
        public float _WaveLength; // 波长

        protected void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            MainMaterial.SetFloat("_ScrollingSpeed", _ScrollingSpeed);
            MainMaterial.SetFloat("_Amplitude", _Amplitude);
            MainMaterial.SetFloat("_WaveLength", _WaveLength);
            Graphics.Blit(source, destination, MainMaterial);
        }
    }
}