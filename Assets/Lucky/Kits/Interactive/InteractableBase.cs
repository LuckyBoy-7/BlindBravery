using System;
using Lucky.Framework;
using Lucky.Kits;
using UnityEngine;

namespace Lucky.Kits.Interactive
{
    public abstract class InteractableBase : ManagedBehaviour
    {
        public enum SortingLayerType
        {
            Default,
            WorldUI,
            Screen
        }

        protected abstract SortingLayerType sortingLayerType { get; }

        protected long OffsetSortingLayer
        {
            get
            {
                return sortingLayerType switch
                {
                    SortingLayerType.Default => 0,
                    SortingLayerType.WorldUI => 1000,
                    SortingLayerType.Screen => 2000,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }

        protected abstract long SortingOrder { get; }

        public virtual int CompareSortingOrder(InteractableBase other) => (int)Mathf.Sign(SortingOrder - other.SortingOrder);


        // 由EnemyManager调用以判断当前鼠标位置是否在敌人范围内
        // 这样在Drag Card的时候就知道要不要显示Selection Box了
        public abstract bool IsPositionInBounds(RectTransform trans = null);

        // 判断enter和exit bounds所用的位置
        public abstract Vector2 BoundsCheckPos { get; }


        private bool preInBounds = false;

        protected override void ManagedFixedUpdate()
        {
            base.ManagedFixedUpdate();
            bool curInBounds = IsPositionInBounds();
            if (curInBounds && !preInBounds)
                CursorEnterBounds();
            else if (!curInBounds && preInBounds)
                CursorExitBounds();
            preInBounds = curInBounds;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////

        #region Delegates

        // 这些是可以给外部用的给
        public event Action OnCursorEnterEvent; // 鼠标进入时
        public event Action OnCursorExitEvent; // 鼠标离开时
        public event Action OnCursorEnterBoundsEvent; // 鼠标进入时
        public event Action OnCursorExitBoundsEvent; // 鼠标进入时
        public event Action OnCursorPressEvent; // 鼠标按下时
        public event Action OnCursorReleaseEvent; // 鼠标释放时
        public event Action OnCursorClickEvent; // 鼠标点击时
        public event Action<Vector2> OnCursorDragEvent; // 鼠标拖拽时
        public event Action OnCursorHoverEvent; // 鼠标悬停时（只要在范围内就触发）
        public event Action OnCursorLongPressEvent; // 且在area范围内
        public event Action OnCursorWipeEvent; // drag并抖动
        public event Action OnCursorReleaseInBoundsEvent; // 释放是在box内

        #endregion

        public float WipeDistanceAccumulator { get; set; } // 因为wipe这个状态其实不是很好定义，如果wipe的多个对象都不知道要作用于哪一个，所以还是写在基类里，wipeDistance量到了就调用
        private bool debug = false;
        public bool canInteract = true;
        public bool canBlockRaycast = true;

        #region Methods Called By GameCursor

        public void CursorEnter()
        {
            if (debug)
                print("Enter");

            OnCursorEnterEvent?.Invoke();
            OnCursorEnter();
        }

        public void CursorExit()
        {
            if (debug)
                print("Exit");
            OnCursorExitEvent?.Invoke();
            OnCursorExit();
        }

        public void CursorEnterBounds()
        {
            if (debug)
                print("ExitBounds");

            OnCursorEnterBoundsEvent?.Invoke();
            OnCursorEnterBounds();
        }

        public void CursorExitBounds()
        {
            if (debug)
                print("Exit");
            OnCursorExitBoundsEvent?.Invoke();
            OnCursorExitBounds();
        }

        public void CursorPress()
        {
            if (debug)
                print("Press");
            OnCursorPressEvent?.Invoke();
            OnCursorPress();
        }

        public void CursorRelease()
        {
            if (debug)
                print("Release");
            WipeDistanceAccumulator = 0;
            OnCursorReleaseEvent?.Invoke();
            OnCursorRelease();
        }

        public void CursorClick()
        {
            if (debug)
                print("Click");
            OnCursorClickEvent?.Invoke();
            OnCursorClick();
        }

        public void CursorDrag(Vector2 delta)
        {
            if (debug)
                print("Drag");
            OnCursorDragEvent?.Invoke(delta);
            OnCursorDrag(delta);
        }

        public void CursorHover()
        {
            if (debug)
                print("Hover");
            OnCursorHoverEvent?.Invoke();
            OnCursorHover();
        }

        public void CursorLongPress()
        {
            if (debug)
                print("LongPress");
            OnCursorLongPressEvent?.Invoke();
            OnCursorLongPress();
        }

        public void CursorWipe()
        {
            if (debug)
                print("Wipe");
            OnCursorWipeEvent?.Invoke();
            OnCursorWipe();
        }

        public void CursorReleaseInBounds()
        {
            if (debug)
                print("ReleaseInBounds");
            OnCursorReleaseInBoundsEvent?.Invoke();
            OnCursorReleaseInBounds();
        }

        #endregion

        #region Methods Overridden By Children

        // 这些是可以给子类用的用的给
        protected virtual void OnCursorEnter()
        {
        }

        protected virtual void OnCursorExit()
        {
        }

        protected virtual void OnCursorEnterBounds()
        {
        }

        protected virtual void OnCursorExitBounds()
        {
        }

        protected virtual void OnCursorPress()
        {
        }

        protected virtual void OnCursorRelease()
        {
        }

        protected virtual void OnCursorClick()
        {
        }

        /// <summary>
        /// OnCursor分为两种, 一种是从鼠标按下开始算, 一种是要拖动才开始算
        /// 由于第二种可以由第一种稍微判断一下来实现, 且碰到的需求相对少, 且我也不太知道取什么名所以就只写第一种的实现
        /// </summary>
        /// <param name="delta"></param>
        protected virtual void OnCursorDrag(Vector2 delta)
        {
        }

        protected virtual void OnCursorHover()
        {
        }

        protected virtual void OnCursorLongPress()
        {
        }

        protected virtual void OnCursorWipe()
        {
        }

        protected virtual void OnCursorReleaseInBounds()
        {
        }

        #endregion

    }
}