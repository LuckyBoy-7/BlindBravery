using System;
using System.Collections;
using System.Collections.Generic;
using Lucky.Kits.Extensions;
using UnityEngine;

namespace Lucky.Shaders
{

    [RequireComponent(typeof(SpriteRenderer))]
    public class WaterMirror : MonoBehaviour
    {
        private SpriteRenderer sr;
        private new Camera camera;
        private RenderTexture renderTexture;
        public Material material;
        public int precise = 100;

        private void Awake()
        {
            sr = GetComponent<SpriteRenderer>();
            camera = new GameObject("WaterMirrorCamera").AddComponent<Camera>();
            camera.orthographic = true;
            // 先把湖面的比例搞对了, 这里保持高度, 缩放宽度
            float aspect = camera.aspect; // w / h
            float height = transform.localScale.y;
            float width = aspect * height;
            transform.localScale = transform.localScale.WithX(width);

            // 调整相机大小
            camera.orthographicSize = height / 2;

            // 把相机移到上面
            camera.transform.position = transform.position + Vector3.up * height;

            renderTexture = new RenderTexture((int)width * precise, (int)height * precise, 0);
            renderTexture.filterMode = FilterMode.Point;
            renderTexture.wrapMode = TextureWrapMode.Repeat;
            camera.targetTexture = renderTexture;

            // 可见
            camera.transform.position = camera.transform.position.WithZ(-10);
            camera.clearFlags = CameraClearFlags.Depth;
        }

        private void Update()
        {
            material.SetTexture("_ViewTex", renderTexture);
        }


    }
}