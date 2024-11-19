using System;
using Lucky.Kits.Interactive;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Lucky.Kits.UI.Interactive
{
    public class Button : InteractableScreenUI
    {
        public Image outlineImage;
        public Color origOutlineColor = Color.black;
        public Color hoverOutlineColor = Color.white;
        public UnityEvent onClick;

        private void Awake()
        {
            outlineImage.color = origOutlineColor;
        }

        protected override void OnCursorHover()
        {
            base.OnCursorHover();
            outlineImage.color = hoverOutlineColor;
        }

        protected override void OnCursorExit()
        {
            base.OnCursorExit();
            outlineImage.color = origOutlineColor;
        }

        protected override void OnCursorPress()
        {
            base.OnCursorPress();
            onClick?.Invoke();
        }
    }
}