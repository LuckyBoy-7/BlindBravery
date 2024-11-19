using System.Collections.Generic;
using Lucky.Framework;
using Lucky.Kits.Interactive;
using Lucky.Kits.Utilities;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;


namespace Lucky.Kits.UI
{
    /// <summary>
    /// 感觉能用上的地方比较局限, 主要用在静态点的地方吧(也就是无对话文字直接显示, 当然逐字显示也可以兼容, 但先这么放着吧)
    /// </summary>
    [RequireComponent(typeof(TMP_Text))]
    public class HyperLink : ManagedBehaviour
    {
        public DialogueText dialogueText;
        public Color linkColor = Color.red; // 未访问状态的color
        public Color pressedColor = Color.green; // 点击时的color
        public Color visitedColor = Color.blue; // 点击过后的color
        private TMP_Text Text;
        public Texture2D HandTexture;
        private Dictionary<string, bool> linkVisited = new();
        private string rawContent = "";
        private string pressedId = "";
        private string hoverId = "";

        public bool CanInteract => dialogueText == null || !dialogueText.isTalking;

        private void Awake()
        {
            Text = GetComponent<TMP_Text>();
            Text.ForceMeshUpdate();
        }

        private void Start()
        {
            ResetLink();
        }

        public void ResetLink()
        {
            linkVisited.Clear();
            foreach (var info in Text.textInfo.linkInfo)
            {
                linkVisited[info.GetLinkID()] = false;
            }

            rawContent = Text.text;
        }

        protected override void ManagedFixedUpdate()
        {
            base.ManagedFixedUpdate();

            if (!CanInteract)
                return;
            // 这里mouse pos视情况而定，如果纯ui就用game cursor的，如果是overlay的canvas直接传屏幕坐标即可
            // int linkIndex = TMP_TextUtilities.FindIntersectingLink(tmpText, GameCursor.MouseWorldPos, null);
            int hoveringLinkIndex = TMP_TextUtilities.FindIntersectingLink(Text, GameCursor.MouseScreenPos, null);
            if (hoveringLinkIndex != -1) // 如果点击在超链接上
            {
                if (Input.GetMouseButtonDown(0))
                    pressedId = Text.textInfo.linkInfo[hoveringLinkIndex].GetLinkID();
                hoverId = Text.textInfo.linkInfo[hoveringLinkIndex].GetLinkID();
            }
            else
            {
                hoverId = "";
            }

            // 第一次点击发送事件
            if (Input.GetMouseButtonUp(0) && pressedId != "")
            {
                if (!linkVisited[pressedId])
                {
                    // EventManager.instance.Broadcast($"Link{pressedId}Pressed");
                    linkVisited[pressedId] = true;
                }

                pressedId = "";
            }
        }

        public override void Render()
        {
            base.Render();

            // 设置鼠标样式
            if ((pressedId != "" || hoverId != "") && HandTexture != null)
                Cursor.SetCursor(HandTexture, new Vector2(10, 0), CursorMode.Auto);
            else
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

            string tmpContent = rawContent;
            foreach (var linkInfo in Text.textInfo.linkInfo)
            {
                string linkContent = linkInfo.GetLinkText();
                string linkId = linkInfo.GetLinkID();
                string pattern = HtmlUtils.WrapTag(linkContent, "link", linkId);

                Color color;
                if (linkId == pressedId)
                    color = pressedColor;
                else if (linkVisited[linkId])
                    color = visitedColor;
                else
                    color = linkColor;
                string colorString = "#" + ColorUtils.ToHexString(color);

                tmpContent = tmpContent.Replace(pattern, HtmlUtils.WrapTag(pattern, "color", colorString));
                // 因为下划线的颜色也会被影响所以放最后
                if (CanInteract && (linkId == hoverId || linkId == pressedId))
                    tmpContent = tmpContent.Replace(pattern, HtmlUtils.WrapTag(pattern, "u"));
            }

            Text.text = tmpContent;
        }
    }
}