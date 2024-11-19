using System;
using System.Collections.Generic;
using Lucky.Framework;
using TMPro;
using UnityEngine;

namespace Lucky.Kits.UI.Interactive
{
    public abstract class Select : ManagedBehaviour
    {
        private Toggle togglePrefab;
        private TMP_Text text;
        public abstract string Content { get; }
        public abstract string StartToggleName { get; }
        private Transform rightArea;
        private List<Toggle> toggles = new();

        private void Awake()
        {
            togglePrefab = Resources.Load<Toggle>("Prefabs/UI/Toggle");
            text = transform.GetChild(0).GetComponent<TMP_Text>();
            rightArea = transform.GetChild(1);
            CreateItems();
            AddExclusiveToggles();
            text.text = Content;
            foreach (var toggle in toggles)
            {
                if (toggle.Content == StartToggleName)
                    toggle.On = true;
            }
        }

        protected abstract void CreateItems();

        protected void CreateItem(string name, Action onCheck)
        {
            Toggle toggle = Instantiate(togglePrefab, rightArea);
            toggle.Content = name;
            toggle.OnCheck = onCheck;
            toggles.Add(toggle);
        }

        protected void AddExclusiveToggles()
        {
            foreach (var t1 in toggles)
            {
                foreach (var t2 in toggles)
                {
                    if (t1 != t2)
                    {
                        t1.exclusiveToggles.Add(t2);
                        t2.exclusiveToggles.Add(t1);
                    }
                }
            }
        }


    }
}