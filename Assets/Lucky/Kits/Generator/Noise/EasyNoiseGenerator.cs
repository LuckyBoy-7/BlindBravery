using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Lucky.Kits.Extensions;
using Lucky.Kits.Utilities;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Random = System.Random;

namespace Lucky.Kits.Generator.Noise
{
    public class EasyNoiseGenerator : NoiseGenerator
    {
        protected override string FileName => "EasyNoise.png";

        protected override Color GetPixelColor(int x, int y) => Color.white * RandomUtils.NextFloat(1);
    }
}