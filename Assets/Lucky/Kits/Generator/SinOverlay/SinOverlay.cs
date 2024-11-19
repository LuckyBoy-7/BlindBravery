using System;
using System.Collections.Generic;
using Lucky.Framework;
using Lucky.Kits.Extensions;
using Lucky.Kits.Utilities;
using UnityEngine;
using UnityEngine.Serialization;

namespace Lucky.Kits.Generator.SinOverlay
{
    [Serializable]
    public struct SinData
    {
        [Range(0, 5)] public float a;
        [Range(0, 5)] public float w;
        public float startPhase;
    }

    public class SinOverlay : ManagedBehaviour
    {
        public float scale;
        public float lineWidth;
        public Color lineColor = Color.green;
        public int segments = 100;
        public List<SinData> datas = new();
        private List<LineRenderer> lines = new();
        private LineRenderer combinedLine;

        private LineRenderer NewLineRenderer()
        {
            LineRenderer line = this.NewSonWithComponent<LineRenderer>();
            line.transform.localPosition = Vector3.zero;
            line.useWorldSpace = false;
            line.material = new Material(Shader.Find("Sprites/Default"));
            return line;
        }

        private void Awake()
        {
            combinedLine = NewLineRenderer();
        }

        protected override void ManagedUpdate()
        {
            base.ManagedUpdate();
            // 数量不够就补充
            while (lines.Count < datas.Count)
            {
                LineRenderer line = NewLineRenderer();
                lines.Add(line);
            }

            // 数量多了就关闭
            for (var i = 0; i < lines.Count; i++)
            {
                lines[i].enabled = i < datas.Count;
            }

            float[] values = new float[segments + 1];
            foreach (var (data, line) in Itertools.Zip(datas, lines))
            {
                line.positionCount = segments + 1;
                for (int i = 0; i < segments + 1; i++)
                {
                    float x = (float)i / segments * MathUtils.PI(2);
                    float y = data.a * MathUtils.Sin(data.w * x + data.startPhase);
                    line.SetPosition(i, new Vector3(x, y) * scale);
                    values[i] += y;
                }

                line.startWidth = line.endWidth = lineWidth;
                line.startColor = line.endColor = lineColor;
            }

            // 叠加后的波
            combinedLine.positionCount = segments + 1;
            for (int i = 0; i < segments + 1; i++)
            {
                float x = (float)i / segments * MathUtils.PI(2);
                combinedLine.SetPosition(i, new Vector3(x, values[i]) * scale);
            }

            combinedLine.startWidth = combinedLine.endWidth = lineWidth;
            combinedLine.startColor = combinedLine.endColor = Color.red;
        }
    }
}