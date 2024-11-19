using System;
using System.Collections;
using System.Collections.Generic;
using Lucky.Framework;
using Lucky.Kits.Interactive;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Lucky.Kits.UI.Interactive
{
    public class Toggle : InteractableScreenUI
    {
        public bool on;

        public bool On
        {
            get => on;
            set
            {
                on = value;
                checkmark.enabled = on;
                onValueChange?.Invoke(on);
                if (on)
                    OnCheck?.Invoke();
            }
        }

        public Image checkmark;
        private TMP_Text text;
        public List<Toggle> exclusiveToggles = new();

        public string Content
        {
            get => text.text;
            set => text.text = value;
        }

        public UnityEvent<bool> onValueChange;
        public Action OnCheck;

        private void Awake()
        {
            text = GetComponentInChildren<TMP_Text>();
            On = on;
        }

        protected override void OnCursorReleaseInBounds()
        {
            base.OnCursorReleaseInBounds();
            // 说明只是个普通开关
            if (exclusiveToggles.Count == 0)
            {
                On = !On;
            }
            else // 说明是Select
            {
                if (On == true)
                    return;
                foreach (var exclusiveToggle in exclusiveToggles)
                {
                    exclusiveToggle.On = false;
                }

                On = true;
            }
        }
    }
}