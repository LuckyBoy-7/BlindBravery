using System;
using System.Collections.Generic;
using Lucky.Kits.Managers;
using UnityEngine;
using UnityEngine.Serialization;


namespace Lucky.Kits.Interactive
{
    /// <summary>
    /// 注意先把相机大小放大100倍
    /// </summary>
    public class GameCursor : Singleton<GameCursor>
    {
        private HashSet<InteractableUIBase> interactableUIs = new();
        public static Vector2 MouseWorldPosDelta;
        public static Vector2 MouseWorldPos => Camera.main.ScreenToWorldPoint(MouseScreenPos);
        public static Vector2 MouseWorldCellPos => new(Mathf.Floor(MouseWorldPos.x + 0.5f), Mathf.Floor(MouseWorldPos.y + 0.5f));
        public static Vector2 MouseScreenPos => Input.mousePosition;
        private Vector2 PreviousMouseWorldPosition { get; set; }
        private InteractableBase PreviousInteractable { get; set; }
        private InteractableBase CurrentInteractable { get; set; }
        public InteractableBase MouseButtonDownInteractable { get; set; } // 点击时对应的第一个对象
        private float MouseButtonDownTimestamp { get; set; } = -1;
        private float RealtimeSinceMouseButtonDown => Time.realtimeSinceStartup - MouseButtonDownTimestamp;
        [Header("Click")] [SerializeField] private float clickTimeThreshold = 0.2f;
        [Header("LongPress")] [SerializeField] private float longPressTimeThreshold = 0.8f;
        [SerializeField] private float longPressOffsetTolerance = 0.1f;
        private bool IsLongPressShake { get; set; } // 鼠标位置是否有偏移（长按必须一开始就长按，不能刚开始按，然后鼠标歪了还能长按（不然有点反直觉））
        [Header("Wipe")] [SerializeField] private float wipeCountDistanceThreshold = 3f;
        private float wipeValidDistanceThreshold = 1;
        public static LayerMask LayerMask; // 减小开销

        protected override void Awake()
        {
            base.Awake();
            LayerMask = LayerMask.NameToLayer("Interactable");
        }

        protected override void ManagedUpdate()
        {
            base.ManagedUpdate();
            UpdateCurrentInteractable();
            // 如果是刚开始的话，就把previous改成当前位置
            if (PreviousMouseWorldPosition == default)
                PreviousMouseWorldPosition = MouseWorldPos;

            if (CurrentInteractable != PreviousInteractable && PreviousInteractable != null)
                PreviousInteractable.CursorExit();
            if (CurrentInteractable != PreviousInteractable && CurrentInteractable != null)
                CurrentInteractable.CursorEnter();
            if (CurrentInteractable != null)
                CurrentInteractable.CursorHover();
            if (Input.GetMouseButtonDown(0))
            {
                MouseButtonDownTimestamp = Time.realtimeSinceStartup;
                MouseButtonDownInteractable = CurrentInteractable;
                if (CurrentInteractable)
                    CurrentInteractable.CursorPress();
            }

            if (Input.GetMouseButton(0))
            {
                Vector2 delta = MouseWorldPos - PreviousMouseWorldPosition;
                MouseWorldPosDelta = delta;
                if (MouseButtonDownInteractable != null)
                    MouseButtonDownInteractable.CursorDrag(delta);

                // longPress
                if (delta.magnitude > longPressOffsetTolerance)
                    IsLongPressShake = true;
                if (RealtimeSinceMouseButtonDown >= longPressTimeThreshold && !IsLongPressShake)
                {
                    IsLongPressShake = true;
                    if (MouseButtonDownInteractable != null)
                        MouseButtonDownInteractable.CursorLongPress();
                }

                // wipe
                if (CurrentInteractable != null)
                {
                    if (delta.magnitude > wipeValidDistanceThreshold)
                        CurrentInteractable.WipeDistanceAccumulator += delta.magnitude;
                    if (CurrentInteractable.WipeDistanceAccumulator >= wipeCountDistanceThreshold)
                    {
                        CurrentInteractable.WipeDistanceAccumulator -= wipeCountDistanceThreshold;
                        CurrentInteractable.CursorWipe();
                    }
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                IsLongPressShake = false;
                if (MouseButtonDownInteractable != null)
                {
                    if (RealtimeSinceMouseButtonDown <= clickTimeThreshold)
                        MouseButtonDownInteractable.CursorClick();
                    MouseButtonDownInteractable.CursorRelease();
                    if (MouseButtonDownInteractable.IsPositionInBounds())
                    {
                        MouseButtonDownInteractable.CursorReleaseInBounds();
                    }
                }

                MouseButtonDownInteractable = null;
            }

            PreviousMouseWorldPosition = MouseWorldPos;
        }

        private void UpdateCurrentInteractable()
        {
            InteractableBase topInteractable = null;
            foreach (var curInteractable in GetSortedHitInteractables())
            {
                if (!curInteractable.canInteract && curInteractable.canBlockRaycast)
                    break;
                if (curInteractable.canInteract)
                {
                    topInteractable = curInteractable;
                    break;
                }
            }

            // 更新
            PreviousInteractable = CurrentInteractable;
            CurrentInteractable = topInteractable;
        }

        public List<InteractableBase> GetSortedHitInteractables()
        {
            // 拿到当前鼠标指向的所有的Interactable
            List<InteractableBase> hitInteractables = new();
            // 正交透视相机都适用
            // foreach (var hitCollider in Physics2D.GetRayIntersectionAll(Camera.main.ScreenPointToRay(Input.mousePosition), 10000, LayerMask))
            foreach (var hitCollider in Physics2D.GetRayIntersectionAll(Camera.main.ScreenPointToRay(Input.mousePosition)))
            {
                // 这里要在父级里找, 因为有时候我们是一个空物体挂个Entity脚本, 子物体挂对应组件的
                var component = hitCollider.collider.GetComponentInParent<InteractableBase>();
                if (component != null)
                    hitInteractables.Add(component);
            }

            foreach (InteractableUIBase ui in interactableUIs)
            {
                if (ui.IsPositionInBounds())
                    hitInteractables.Add(ui);
            }

            // 找个最高的（倒序排序）
            hitInteractables.Sort((a, b) => b.CompareSortingOrder(a));
            return hitInteractables;
        }

        public bool IsPointerOverInteractable() => CurrentInteractable != null;

        public InteractableBase GetKthInteractable(int kth = 1)
        {
            if (kth == 1)
                return CurrentInteractable;
            foreach (var curInteractable in GetSortedHitInteractables())
            {
                if (!curInteractable.canInteract && curInteractable.canBlockRaycast)
                    return null;
                if (curInteractable.canInteract)
                {
                    if (--kth == 0)
                        return curInteractable;
                }
            }

            return null;
        }

        public T GetFirstInteractableWithoutBlock<T>() where T : InteractableBase
        {
            foreach (var curInteractable in GetSortedHitInteractables())
            {
                if (curInteractable is T && curInteractable.canInteract)
                    return (T)curInteractable;
            }

            return null;
        }

        public void RegisterInteractableUI(InteractableUIBase ui) => interactableUIs.Add(ui);

        public void UnregisterInteractableUI(InteractableUIBase ui) => interactableUIs.Remove(ui);
    }
}