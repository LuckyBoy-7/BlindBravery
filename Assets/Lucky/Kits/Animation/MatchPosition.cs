using Lucky.Framework;
using Lucky.Kits;
using UnityEngine;

namespace Lucky.Kits.Animation
{
    /// <summary>
    /// 相当于把物体固定在某个对象上的相对位置（以某些维度），然后可以做一定偏移
    /// </summary>
    public class MatchPosition : ManagedBehaviour
    {
        public Transform target;
        public Vector3 offset;
        public bool x = true;
        public bool y = true;
        public bool z = true;
        public bool destroyIfTargetNull;

        protected override void ManagedFixedUpdate()
        {
            base.ManagedFixedUpdate();
            if (target != null)
            {
                // 我感觉这么写的话不match的时候物体就跑了
                // Vector3 pos = new Vector3 (
                // 	              x ? target.transform.position.x : transform.position.x,
                // 	              y ? target.transform.position.y : transform.position.y,
                // 	              z ? target.transform.position.z : transform.position.z
                //               );
                //          transform.position = pos + offset;
                // 感觉得这么写，如果不match就不变
                transform.position = new Vector3(
                    x ? target.transform.position.x + offset.x : transform.position.x,
                    y ? target.transform.position.y + offset.y : transform.position.y,
                    z ? target.transform.position.z + offset.z : transform.position.z
                );
            }
            else if (destroyIfTargetNull)
            {
                Destroy(gameObject);
            }
        }
    }
}