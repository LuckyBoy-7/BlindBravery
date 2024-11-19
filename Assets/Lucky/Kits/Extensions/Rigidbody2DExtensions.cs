using System;
using Lucky.Kits.Utilities;
using UnityEngine;

namespace Lucky.Kits.Extensions
{
    public static class Rigidbody2DExtensions
    {
        public static void SetSpeedX(this Rigidbody2D orig, float x)
        {
            orig.velocity = orig.velocity.WithX(x);
        }

        public static void SetSpeedY(this Rigidbody2D orig, float y)
        {
            orig.velocity = orig.velocity.WithY(y);
        }

        public static void AddSpeedX(this Rigidbody2D orig, float x)
        {
            orig.velocity = orig.velocity.WithX(orig.velocity.x + x);
        }

        public static void AddSpeedY(this Rigidbody2D orig, float y)
        {
            orig.velocity = orig.velocity.WithY(orig.velocity.y + y);
        }
        
        public static void MulSpeedX(this Rigidbody2D orig, float k)
        {
            orig.velocity = orig.velocity.WithX(orig.velocity.x * k);
        }
        
        public static void MulSpeedY(this Rigidbody2D orig, float k)
        {
            orig.velocity = orig.velocity.WithY(orig.velocity.y * k);
        }
    }
}