using UnityEngine;

namespace Lucky.Kits.Utilities
{
    public static class CameraUtils
    {
        public static float HalfHeight => Camera.main.orthographicSize;
        public static float HalfWidth => Camera.main.aspect * HalfHeight;
        public static float Width => HalfWidth * 2;
        public static float Height = HalfHeight * 2;
        public static float Left => Camera.main.transform.position.x - HalfWidth;
        public static float Right => Camera.main.transform.position.x + HalfWidth;
        public static float Top => Camera.main.transform.position.y + HalfHeight;
        public static float Bottom => Camera.main.transform.position.y - HalfHeight;
        public static Vector2 BottomLeft => new Vector2(Left, Bottom);
        public static Vector2 BottomRight => new Vector2(Right, Bottom);
        public static Vector2 TopLeft => new Vector2(Left, Top);
        public static Vector2 TopRight => new Vector2(Right, Top);
        public static bool InBounds(Vector2 pos) => pos.x >= Left && pos.x <= Right && pos.y >= Bottom && pos.y <= Top;

        public static Rect Bounds => new Rect(Left, Bottom, Width, Height);
    }
}