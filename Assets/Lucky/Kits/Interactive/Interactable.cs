using System;
using UnityEngine;

namespace Lucky.Kits.Interactive
{
    public class Interactable : InteractableBase
    {
        [SerializeField]
        private Collider2D _collider;

        public Collider2D Collider
        {
            get
            {
                if (_collider == null)
                    _collider = GetComponent<Collider2D>();
                return _collider;
            }
        }

        [SerializeField]
        private SpriteRenderer _renderer;

        public SpriteRenderer Renderer
        {
            get
            {
                if (_renderer == null)
                    _renderer = GetComponent<SpriteRenderer>();
                return _renderer;
            }
        }

        protected override SortingLayerType sortingLayerType => SortingLayerType.Default;

        // SortingLayer.GetLayerValueFromID，通过id返回对应的层序号，越高的层序号越大从0开始
        protected override long SortingOrder =>
            (SortingLayer.GetLayerValueFromID(Renderer.sortingLayerID) + OffsetSortingLayer) * 10000 + Renderer.sortingOrder;

        public override bool IsPositionInBounds(RectTransform trans = null) => Collider.OverlapPoint(BoundsCheckPos);
        public override Vector2 BoundsCheckPos => GameCursor.MouseWorldPos;
    }
}