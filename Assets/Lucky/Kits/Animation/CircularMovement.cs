using System.Collections;
using System.Collections.Generic;
using Lucky.Framework;
using Lucky.Kits;
using UnityEngine;

public class CircularMovement : ManagedBehaviour
{
    /// <summary>
    /// x^2 / a^2 + y^2 / b^2 = 1
    /// x = a * cos(t)  // a为半长轴距离
    /// y = b * sin(t)  // b为半短轴距离
    /// 其实这里cos，sin反一下也没事（只是初相变了），重点是a必须对的x，b必须对的y
    /// </summary>
    [SerializeField] // radii是半径的复数
    private Vector2 radii = Vector2.one;

    [SerializeField] private float speed = 1f;

    [SerializeField, Range(0, 1)] private float normalizedPositionOffset = 0f;

    private float timer = 0f;

    private float startPhase => normalizedPositionOffset * Mathf.PI * 2f;

    protected override void ManagedFixedUpdate()
    {
        timer += Time.deltaTime;
        float scaledTime = startPhase + timer * speed;
        // 因为这里已经用了localPosition了，所以如果要pivot那就自己挂一个爸爸
        transform.localPosition = new Vector3(Mathf.Sin(scaledTime) * radii.x, Mathf.Cos(scaledTime) * radii.y, transform.localPosition.z);
    }
}