using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Lucky.Kits.Extensions;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Ease = Lucky.Kits.Utilities.Ease;
using Random = UnityEngine.Random;

namespace Lucky.Kits.UI
{
    public class ButtonStyle : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField] private List<Image> frontImages, bgImages;
        [SerializeField] private List<TMP_Text> texts;

        [SerializeField] private bool doScale;
        [SerializeField, ShowIf("doScale")] private float scaleAmount;

        [SerializeField] private bool doRotation;
        [SerializeField, ShowIf("doRotation")] private float rotationAmount;

        [SerializeField] private bool doColors;
        [SerializeField, ShowIf("doColors")] private List<Color> backColors = new() { Color.black };
        [SerializeField, ShowIf("doColors")] private List<Color> frontColors = new() { Color.white };

        private Vector3 originalScale;
        private Color originalBackColor, originalFrontColor;

        public bool IsHovered { get; set; }

        public Action onClick;
        public Action onEnter;
        public Action onExit;

        private void Start()
        {
            originalScale = transform.localScale;
            SaveOriginalColors();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            IsHovered = true;
            ApplyScaling(scaleAmount);
            ApplyRotation(Random.Range(-rotationAmount, rotationAmount));
            ApplyColors(backColors.Choice(), frontColors.Choice());
            onEnter?.Invoke();
        }

        private void ApplyScaling(float amount)
        {
            if (doScale)
                transform.DOScale(originalScale * (1f + amount), 0.2f).SetEase(DG.Tweening.Ease.OutSine);
        }

        private void ApplyRotation(float amount)
        {
            if (doRotation)
                transform.DORotate(new Vector3(0, 0, amount), 0.2f).SetEase(DG.Tweening.Ease.OutSine);
        }

        private void ApplyColors(Color back, Color front)
        {
            if (!doColors) return;
            bgImages.ForEach(i => i.color = back);
            frontImages.ForEach(i => i.color = front);
            texts.ForEach(t => t.color = back);
        }

        private void SaveOriginalColors()
        {
            if (!doColors) return;

            bgImages.ForEach(i => { originalBackColor = i.color; });

            frontImages.ForEach(i => { originalFrontColor = i.color; });
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Reset();
            onExit?.Invoke();
        }

        public void Reset()
        {
            IsHovered = false;
            ApplyScaling(0);
            ApplyRotation(0);
            ApplyColors(originalBackColor, originalFrontColor);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            onClick?.Invoke();
        }
    }
}