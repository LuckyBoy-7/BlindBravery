using System;
using UnityEngine;

namespace Lucky.Framework
{
    [RequireComponent(typeof(RectTransform))]
    public class ManagedUIBehaviour : ManagedBehaviour
    {
        [HideInInspector] public RectTransform RectTransform;

        protected virtual void Awake()
        {
            RectTransform = GetComponent<RectTransform>();
        }
    }
}