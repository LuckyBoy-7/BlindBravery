using UnityEngine;

namespace Lucky.Kits.Extensions
{
    public static class RectTransformExtensions
    {
        public static void SetAnchoredPositionX(this RectTransform orig, float x) => orig.anchoredPosition = orig.anchoredPosition.WithX(x);
        public static void SetAnchoredPositionY(this RectTransform orig, float y) => orig.anchoredPosition = orig.anchoredPosition.WithY(y);
        public static void SetAnchoredPositionZ(this RectTransform orig, float z) => orig.anchoredPosition = orig.anchoredPosition.WithZ(z);

        public static void AddAnchoredPositionX(this RectTransform orig, float x) =>
            orig.anchoredPosition = orig.anchoredPosition.WithX(orig.anchoredPosition.x + x);

        public static void AddAnchoredPositionY(this RectTransform orig, float y) =>
            orig.anchoredPosition = orig.anchoredPosition.WithY(orig.anchoredPosition.y + y);

        public static void SetAnchor(this RectTransform orig, Vector2 pos) => orig.anchorMin = orig.anchorMax = pos;
        public static void SetPivot(this RectTransform orig, Vector2 pos) => orig.pivot = pos;
    }
}