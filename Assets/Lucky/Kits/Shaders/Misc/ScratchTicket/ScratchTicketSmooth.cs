using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScratchTicketSmooth : MonoBehaviour
{
    public Material material;
    public Camera lineCamera;
    public float radius;
    private Vector3 prePos = Vector3.one * 10000; // 不知道为什么用inf就爆了
    public LineRenderer brush;

    // 不知道为什么笔刷改成自己的材质才能正常工作
    private void Start()
    {
        material.SetTexture("_LineTex", lineCamera.targetTexture);
        material.SetMatrix("_paintCameraVP", lineCamera.nonJitteredProjectionMatrix * lineCamera.worldToCameraMatrix);
        brush.startColor = brush.endColor = Color.white;
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            brush.enabled = true;
            brush.endWidth = brush.startWidth = radius;
            Vector3 curPos = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (prePos == Vector3.one * 10000)
                prePos = curPos;
            brush.SetPosition(0, prePos);
            brush.SetPosition(1, curPos);

            lineCamera.Render();

            prePos = curPos;
        }
        else
        {
            brush.enabled = false;
            prePos = Vector3.one * 10000;
        }
    }
}