using System.Collections.Generic;
using Lucky.Framework;
using Lucky.Kits.Utilities;
using UnityEngine;
using UnityEngine.U2D;

namespace Misc.Replica.SoftSlime
{
    public class SpriteShape : ManagedBehaviour
    {
        public SpriteShapeController spriteShape;
        public Transform center;
        public List<Transform> points;
        public float mul;

        protected override void ManagedUpdate()
        {
            base.ManagedUpdate();
            for (var i = 0; i < points.Count; i++)
            {
                // 这里倒着算会影响描边的方向(草, 找了半天bug
                int j = points.Count - 1 - i;
                spriteShape.spline.SetPosition(i, points[j].position);
                spriteShape.spline.SetTangentMode(i, ShapeTangentMode.Continuous);
                spriteShape.spline.SetRightTangent(i, MathUtils.TurnRight((center.position - points[j].position).normalized * mul));
                spriteShape.spline.SetLeftTangent(i, -MathUtils.TurnRight((center.position - points[j].position).normalized) * mul);
            }
        }
    }
}