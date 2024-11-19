using System;
using System.Collections;
using System.Collections.Generic;
using Lucky.Framework;
using Lucky.Kits.Extensions;
using UnityEngine;
using UnityEngine.Serialization;

public class Spectrum : ManagedBehaviour
{
    public AudioSource audioSource;
    private Transform[] transforms;
    public float gap = 0.1f;
    public float multiplier = 50;
    public int samplePower = 7;
    public float barSize = 0.1f;
    public bool isPivotMiddle = true;
    private int sampleNum => (int)Mathf.Pow(2, samplePower);

    private void OnValidate()
    {
        samplePower = Mathf.Clamp(samplePower, 6, 13);
    }

    private void Start()
    {
        transforms = new Transform[sampleNum];
        for (int i = 0; i < sampleNum; i++)
        {
            var go = new GameObject("GO");
            go.AddComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Art/Primatives/Square");
            go.transform.parent = transform;
            transforms[i] = go.transform;
            transforms[i].localPosition = i * gap * Vector3.right;
            transforms[i].localScale = Vector3.one * barSize;
        }
    }

    protected override void ManagedUpdate()
    {
        base.ManagedUpdate();
        // Number of values (the length of the samples array provided) must be a power of 2. (ie 128/256/512 etc). Min = 64. Max = 8192
        // 原来如此，那他的采样是直接曲取那个点还是说取平均值呢
        float[] samples = new float[sampleNum];
        audioSource.GetSpectrumData(samples, 0, FFTWindow.Blackman);
        for (int i = 0; i < sampleNum; i++)
        {
            Vector3 scale = transforms[i].localScale;
            transforms[i].localScale = new Vector3(scale.x, samples[i] * multiplier, scale.z);
            transforms[i].localPosition = i * gap * Vector3.right;
            if (!isPivotMiddle)
                transforms[i].SetLocalPositionY(samples[i] * multiplier / 2);
        }
    }
}