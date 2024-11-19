using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Lucky.Kits.Utilities;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.U2D.Animation;

namespace Misc.Replica.SoftSlime
{
    /// <summary>
    /// 拙劣的模仿 https://jimmyc5.itch.io/little-sl, 但确实不知道怎么动态缩放而不影响骨骼(我一缩放骨骼都飞起来了, 以后再说)
    /// https://www.youtube.com/watch?v=F82BlnW5z6g
    /// todo: 动态缩放骨骼, 现在的方案是在中间放个球把整个边撑大
    /// </summary>
    public class Move : MonoBehaviour
    {
        public float duration = 0.5f;
        [Title("Move")]
        public float speedX;
        public List<Rigidbody2D> rbs;
        
        [Title("Distance")]
        // 前后distance joint
        public float distanceJointDist;
        public float bigDistanceJointDist;
        public List<DistanceJoint2D> distJoints;
        private float curDistanceJointDist;
        
        // 前后spring joint
        public float springJointDist;
        public float bigSpringJointDist;
        public List<SpringJoint2D> springJoints;
        private float curSpringJointDist;

        private void Awake()
        {
            SetDistanceJointDist(distanceJointDist);
            SetSpringJointDist(springJointDist);
            curDistanceJointDist = distanceJointDist;
            curSpringJointDist = springJointDist;
        }

        void Update()
        {
            float moveX = Input.GetAxisRaw("Horizontal");
            foreach (var rb in rbs)
            {
                rb.AddForce(Vector2.right * (moveX * speedX * Timer.DeltaTime()));
            }

            float speed, target;
            speed = (bigDistanceJointDist - distanceJointDist) / duration;
            target = Input.GetKey(KeyCode.Space) ? bigDistanceJointDist : distanceJointDist;
            curDistanceJointDist = MathUtils.Approach(curDistanceJointDist, target,speed * Timer.DeltaTime());
            SetDistanceJointDist(curDistanceJointDist);
            
            speed = (bigSpringJointDist - springJointDist) / duration;
            target = Input.GetKey(KeyCode.Space) ? bigSpringJointDist : springJointDist;
            curSpringJointDist = MathUtils.Approach(curSpringJointDist, target,speed * Timer.DeltaTime());
            SetSpringJointDist(curSpringJointDist);

        }

        private void SetDistanceJointDist(float x)
        {
            foreach (var distJoint in distJoints)
            {
                distJoint.distance = x;
            }
        }
        
        private void SetSpringJointDist(float x)
        {
            foreach (var springJoint in springJoints)
            {
                springJoint.distance = x;
            }
        }
    }
}