using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lucky.Shaders.PostProcess;

[ExecuteInEditMode]
public class Offset : PostEffectsBase
{
    [Range(0, 0.05f)] public float[] offsetDistance;
    [Range(0, 3)] public float[] speed;
    public float startPhaseOffset;


    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        int rtW = source.width / 4;
        int rtH = source.height / 4;
        RenderTexture buffer0 = RenderTexture.GetTemporary(rtW, rtH, 0);
        RenderTexture buffer1 = RenderTexture.GetTemporary(rtW, rtH, 0);
        MainMaterial.SetTexture("_OverlayTex", source);

        Graphics.Blit(source, buffer0);
        //其实换一个角度, 可以让metaball有很多子球, 然后随机子球的运动, 这样就不用偏移整个quad了
        for (var i = 0; i < offsetDistance.Length; i++)
        {
            // 混合多个正余弦函数
            var (x, y) = (Mathf.Cos(startPhaseOffset * i + Time.time * speed[i]), Mathf.Sin(startPhaseOffset * i + Time.time * speed[i]));
            Vector2 offset = new Vector2(x, y) * offsetDistance[i];

            MainMaterial.SetVector("_Offset", offset);

            Graphics.Blit(buffer0, buffer1, MainMaterial);
            (buffer0, buffer1) = (buffer1, buffer0);
        }

        Graphics.Blit(buffer0, destination);

        RenderTexture.ReleaseTemporary(buffer0);
        RenderTexture.ReleaseTemporary(buffer1);
    }
}