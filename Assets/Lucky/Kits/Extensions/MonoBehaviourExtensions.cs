using System;
using System.Collections;
using Lucky.Framework;
using Lucky.Kits;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Lucky.Kits.Extensions
{
    public static class ManagedBehaviourExtensions
    {
        /// <summary>
        /// 动态的计时，并且会随着gameObject关闭或是StopAllCoroutines而停止，而invokeRepeating好像会一直执行，且不动态
        /// </summary>
        /// <param name="orig">调用对象</param>
        /// <param name="callback">回调函数</param>
        /// <param name="interval">调用时间间隔</param>
        /// <param name="isScaledTime">是否受时间缩放影响</param>
        /// <param name="isOneShot">是否就执行一次</param>
        public static void CreateFuncTimer(
            this ManagedBehaviour orig,
            Action callback,
            Func<float> interval,
            bool isScaledTime = true,
            bool isOneShot = false,
            bool isStartImmediate = false
        )
        {
            float elapse = isStartImmediate ? interval() : 0;
            orig.StartCoroutine(Tick());

            IEnumerator Tick()
            {
                while (true)
                {
                    elapse += isScaledTime ? Time.deltaTime : Time.unscaledDeltaTime;
                    if (elapse >= interval())
                    {
                        elapse = 0;
                        callback();
                        if (isOneShot)
                            yield break;
                    }

                    yield return null;
                }
            }
        }

        public static void WaitForOneFrameToExecution(this ManagedBehaviour orig, Action callback)
        {
            orig.StartCoroutine(Tick());

            IEnumerator Tick()
            {
                yield return null;
                callback?.Invoke();
            }
        }

        public static void WaitForTwoFrameToExecution(this ManagedBehaviour orig, Action callback)
        {
            orig.StartCoroutine(Tick());

            IEnumerator Tick()
            {
                yield return null;
                yield return null;
                callback?.Invoke();
            }
        }

        public static void DoWaitUntil(this ManagedBehaviour orig, Func<bool> conditionCallback, Action callback)
        {
            orig.StartCoroutine(Tick());

            IEnumerator Tick()
            {
                while (!conditionCallback())
                {
                    yield return null;
                }

                callback?.Invoke();
            }
        }

        public static void DoWaitUntilEndOfFrame(this ManagedBehaviour orig, Action callback)
        {
            orig.StartCoroutine(Tick());

            IEnumerator Tick()
            {
                yield return new WaitForEndOfFrame();
                callback?.Invoke();
            }
        }

        public static float Dist(this ManagedBehaviour orig, ManagedBehaviour other) => (orig.transform.position - other.transform.position).magnitude;
        public static float Dist(this ManagedBehaviour orig, Vector3 pos) => (orig.transform.position - pos).magnitude;
        public static Vector3 Dir(this ManagedBehaviour orig, ManagedBehaviour other) => (other.transform.position - orig.transform.position).normalized;
        public static Vector3 Dir(this ManagedBehaviour orig, Vector3 pos) => (pos - orig.transform.position).normalized;

        public static T NewSonWithComponent<T>(this ManagedBehaviour orig) where T : Component
        {
            T component = new GameObject(typeof(T).Name).AddComponent<T>();
            component.transform.SetParent(orig.transform);
            return component;
        }

        public static T NewUISonWithComponent<T>(this ManagedBehaviour orig) where T : Component
        {
            T component = new GameObject(typeof(T).Name).AddComponent<RectTransform>().gameObject.AddComponent<T>();
            component.transform.SetParent(orig.transform);
            return component;
        }
        
        public static T NewUIRootWithComponent<T>(this ManagedBehaviour orig) where T : Component
        {
            Transform t = orig.transform;
            while (t.parent)
                t = t.parent;
            T component = new GameObject(typeof(T).Name).AddComponent<RectTransform>().gameObject.AddComponent<T>();
            component.transform.SetParent(t);
            t.SetAsLastSibling();
            return component;
        }

        public static T NewUIBrotherWithComponent<T>(this ManagedBehaviour orig) where T : Component
        {
            T component = new GameObject(typeof(T).Name).AddComponent<RectTransform>().gameObject.AddComponent<T>();
            component.transform.SetParent(orig.transform.parent);
            return component;
        }

        /// <summary>
        /// 用Resources加载对应资源, 因为unity好像不允许写在声明部分
        /// </summary>
        public static T LoadAndInstantiate<T>(this ManagedBehaviour orig, string path, bool isChild) where T : Component
        {
            T component = Object.Instantiate(Resources.Load<T>(path));
            if (isChild)
                component.transform.SetParent(orig.transform);
            return component;
        }
    }
}