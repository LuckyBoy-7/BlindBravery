using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireCircle : MonoBehaviour
{
    public Material material;
    private float rate;
    private float origWidth;

    void Start()
    {
        // 这里默认是纯圆
        // 也就是说lineWidth对应世界坐标的宽度
        origWidth = material.GetFloat("_LineWidth");
        rate = origWidth * transform.localScale.x;
    }

    private void Update()
    {
        material.SetFloat("_LineWidth", rate / transform.localScale.x);
    }

    private void OnDestroy()
    {
        material.SetFloat("_LineWidth", origWidth);
    }
}