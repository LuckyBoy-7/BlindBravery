using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lucky.Shaders.PostProcess
{

    [ExecuteInEditMode]
    public class ScreenMask : PostEffectsBase
    {
        [Range(0, 1)] public float _Radius = 0.7f;
        [Range(0, 1)] public float _BlurAmount = 0.2f;

        protected void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            // 不知道为什么获取鼠标位置好像不能再editMode下执行
            Vector3 mousePos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            MainMaterial.SetFloat("_Radius", _Radius);
            MainMaterial.SetFloat("_BlurAmount", _BlurAmount);
            MainMaterial.SetVector("_Center", mousePos);
            Graphics.Blit(source, destination, MainMaterial);
        }
    }
}