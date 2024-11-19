using System.Collections;
using System.Collections.Generic;
using Lucky.Framework;
using Lucky.Kits;
using Lucky.Kits.Extensions;
using Lucky.Kits.Utilities;
using UnityEngine;


namespace Lucky.Kits.Animation
{
    /// <summary>
    /// 让物体在规定区域内不断反弹
    /// </summary>
    public class BounceWithinBounds : ManagedBehaviour
    {
        [SerializeField] private Transform bottomLeftBounds = default;

        [SerializeField] private Transform topRightBounds = default;

        [SerializeField] private float speed = 1f; // 移动速度

        [SerializeField] private float objectRadius = default; // 我们假定的物体半径，其实把物体想象成正方形也行

        [SerializeField] private Vector2 direction = Vector2.one;
        private Vector2 boundsMin => new(transform.position.x - objectRadius, transform.position.y - objectRadius);
        private Vector2 boundsMax => new(transform.position.x + objectRadius, transform.position.y + objectRadius);

        protected override void ManagedFixedUpdate()
        {
            transform.Translate(direction * speed * Time.deltaTime); // 移动，这里是本地的哦

            if (boundsMin.x <= bottomLeftBounds.position.x || boundsMax.x >= topRightBounds.position.x)
            {
                direction.x *= -RandomUtils.Range(0.95f, 1.05f); // 变向，同时稍微改点速度
                float newX = Mathf.Clamp(transform.position.x, bottomLeftBounds.position.x + objectRadius, topRightBounds.position.x - objectRadius);
                transform.position = transform.position.WithX(newX);
            }

            if (boundsMin.y <= bottomLeftBounds.position.y || boundsMax.y >= topRightBounds.position.y)
            {
                direction.y *= -RandomUtils.Range(0.95f, 1.05f);
                float newY = Mathf.Clamp(transform.position.y, bottomLeftBounds.position.y + objectRadius, topRightBounds.position.y - objectRadius);
                transform.position = transform.position.WithY(newY);
            }
        }
    }
}