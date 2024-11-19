using System;
using Lucky.Framework;
using Lucky.Kits;
using Unity.VisualScripting;
using UnityEngine;

namespace Lucky.Kits.Managers
{
    public class Singleton<T> : ManagedBehaviour where T : Singleton<T>
    {
        private static T instance;

        public static T Instance => instance;

        protected virtual void Awake()
        {
            if (instance == null)
            {
                instance = (T)this;
                DontDestroyOnLoad(instance);
            }
            else
                Destroy(gameObject);
        }
    }
}