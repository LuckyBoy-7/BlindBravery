using Lucky.Kits.Extensions;
using Lucky.Kits.Interactive;
using Lucky.Kits.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Lucky.Kits.UI
{
    public class TooltipUI : Singleton<TooltipUI>
    {
        public float diagonalDelta;
        public RectTransform panelRectTransform;
        public UIWrapping wrapping;
        [SerializeField] private bool isShowing;

        protected override void ManagedFixedUpdate()
        {
            base.ManagedFixedUpdate();
            if (isShowing)
                UpdatePos();
        }


        // 设置锚点为左上角（然后加上偏移），如果超出屏幕范围
        // 则把锚点设置到右上角，并施加偏移
        private void UpdatePos()
        {
            Vector2 mousePos = GameCursor.MouseWorldPos;
            // 默认pivot为左上角
            panelRectTransform.pivot = new Vector2(0, 1);
            if (mousePos.x + panelRectTransform.sizeDelta.x + diagonalDelta > (float)Screen.width / 2)
                // pivot变为右上角
                panelRectTransform.pivot += new Vector2(1, 0);
            if (mousePos.y - panelRectTransform.sizeDelta.y - diagonalDelta < -(float)Screen.height / 2)
                panelRectTransform.pivot -= new Vector2(0, 1);

            // 0 1 -> 1 -1
            // 1 1 -> -1 -1
            // 0 0 -> 1 1
            // 1 0 -> -1 1
            // x -> -(x * 2 - 1)
            panelRectTransform.position = mousePos + new Vector2(diagonalDelta, diagonalDelta) * -(panelRectTransform.pivot * 2 - Vector2.one);
        }

        /// <summary>
        /// 到时候show里写参数给对应UI赋值就行
        /// </summary>
        public void Show()
        {
            isShowing = true;
            panelRectTransform.gameObject.SetActive(true);

            this.WaitForOneFrameToExecution(wrapping.UpdateUI);
            UpdatePos();
        }

        public void Hide()
        {
            isShowing = false;
            panelRectTransform.gameObject.SetActive(false);
        }
    }
}