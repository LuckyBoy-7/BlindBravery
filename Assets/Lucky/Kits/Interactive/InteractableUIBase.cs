using Lucky.Kits.Utilities;
using UnityEngine;

namespace Lucky.Kits.Interactive
{
    [RequireComponent(typeof(RectTransform))]
    public abstract class InteractableUIBase : InteractableBase
    {
        public RectTransform rectTransform;

        public RectTransform RectTransform
        {
            get
            {
                if (rectTransform == null)
                    rectTransform = GetComponent<RectTransform>();
                return rectTransform;
            }
        }

        protected override abstract SortingLayerType sortingLayerType { get; }

        protected override abstract long SortingOrder { get; }

        public override abstract Vector2 BoundsCheckPos { get; }

        public override bool IsPositionInBounds(RectTransform trans = null)
        {
            if (trans == null)
                trans = RectTransform;
            return GetScreenRect(trans).Contains(BoundsCheckPos);
            // float scaleX = 1, scaleY = 1;
            // Transform cur = transform;
            // while (cur)
            // {
            //     scaleX *= cur.localScale.x;
            //     scaleY *= cur.localScale.y;
            //     cur = cur.parent;
            // }
            //
            // float width = trans.rect.width * scaleX;
            // float height = trans.rect.height * scaleY;
            // Vector2 pivot = trans.pivot;
            // float x = trans.position.x - pivot.x * width;
            // float y = trans.position.y - pivot.y * height;
            // if (pos.x <= x + width
            //     && pos.x >= x
            //     && pos.y <= y + height
            //     && pos.y >= y)
            //     return true;
            // return false;
        }


        protected virtual void Start()
        {
            GameCursor.Instance?.RegisterInteractableUI(this);
        }

        protected virtual void OnEnable()
        {
            GameCursor.Instance?.RegisterInteractableUI(this);
        }

        protected virtual void OnDisable()
        {
            GameCursor.Instance?.UnregisterInteractableUI(this);
        }
        
        protected Rect GetScreenRect(RectTransform trans)
        {
            Vector3[] corners = new Vector3[4];
            trans.GetWorldCorners(corners);
            float width = MathUtils.Abs(Vector2.Distance(corners[0], corners[3]));
            float height = MathUtils.Abs(Vector2.Distance(corners[0], corners[1]));
            return new Rect(corners[0], new Vector2(width, height));
        }
    }
}