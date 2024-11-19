using Lucky.Framework;
using Lucky.Kits;
using UnityEngine;

namespace Lucky.Kits.Animation
{
    /// <summary>
    /// 自动旋转
    /// </summary>
    public class AutoRotate : ManagedBehaviour
    {
        public Vector3 rotationSpeed = default;
        public bool local = default;

        protected override void ManagedFixedUpdate()
        {
            Vector3 rotateAmount = rotationSpeed * Time.deltaTime;
            if (local)
            {
                transform.Rotate(rotateAmount.x, rotateAmount.y, rotateAmount.z, Space.Self);
            }
            else
            {
                transform.Rotate(rotateAmount.x, rotateAmount.y, rotateAmount.z, Space.World);
            }
        }
    }
}