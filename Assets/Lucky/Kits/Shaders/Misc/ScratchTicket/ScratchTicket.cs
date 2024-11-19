using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ScratchTicket : MonoBehaviour
{
    public Material material;
    public Camera lineCamera;
    public float radius;
    private Transform ball;

    private void Start()
    {
        // 这里的屏幕坐标是基于lineCamera的，如果用shader的compute screenPos，则基于当前相机
        // 用这种方法可以固定刮彩票区域，使得移动主相机不影响mask
        material.SetMatrix("_paintCameraVP", lineCamera.nonJitteredProjectionMatrix * lineCamera.worldToCameraMatrix);
        ball = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
        ball.gameObject.layer = LayerMask.NameToLayer("Brush");
    }


    private void Update()
    {
        ball.localScale = Vector3.one * radius;
        ball.position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetMouseButton(0))
        {
            lineCamera.Render();
            material.SetTexture("_LineTex", lineCamera.targetTexture);
        }
    }
}