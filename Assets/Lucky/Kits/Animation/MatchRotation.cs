using System.Collections;
using System.Collections.Generic;
using Lucky.Framework;
using Lucky.Kits;
using UnityEngine;

namespace Lucky.Kits.Animation
{
    public class MatchRotation : ManagedBehaviour
    {
        [SerializeField] private Transform transformToMatch = default;

        [SerializeField] private bool matchX = default;

        [SerializeField] private bool matchY = default;

        [SerializeField] private bool matchZ = default;

        [SerializeField] private bool inverse = default;

        [SerializeField] private Vector3 offset = default;

        protected override void ManagedFixedUpdate()
        {
            if (transformToMatch == null)
                return;
            // float multiplier = inverse ? -1f : 1f;
            // float x = matchX ? transformToMatch.localEulerAngles.x * multiplier : transform.localEulerAngles.x;
            // float y = matchY ? transformToMatch.localEulerAngles.y * multiplier : transform.localEulerAngles.y;
            // float z = matchZ ? transformToMatch.localEulerAngles.z * multiplier : transform.localEulerAngles.z;
            //
            // transform.localEulerAngles = new Vector3(x, y, z) + offset;
            float multiplier = inverse ? -1f : 1f;
            float x = matchX ? transformToMatch.localEulerAngles.x * multiplier + offset.x : transform.localEulerAngles.x;
            float y = matchY ? transformToMatch.localEulerAngles.y * multiplier + offset.y : transform.localEulerAngles.y;
            float z = matchZ ? transformToMatch.localEulerAngles.z * multiplier + offset.z : transform.localEulerAngles.z;

            transform.localEulerAngles = new Vector3(x, y, z);
        }
    }
}