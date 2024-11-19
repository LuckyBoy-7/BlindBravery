using System;
using System.Collections;
using Lucky.Framework;
using Lucky.Kits.UI.TextEffect;
using Lucky.UI._TMP_Text.TextEffect;
using Sirenix.OdinInspector;
using UnityEngine;


namespace Lucky.Kits.UI
{
    [RequireComponent(typeof(TextEffectController))]
    public class DialogueText : ManagedBehaviour
    {
        public bool IsDebug;
        public DialogueData DebugDialogueData;

        private TextEffectController textEffectController;

        public bool UseMouseButtonToInteract = false;
        [ShowIf("@ CanSkip && !UseMouseButtonToInteract")]
        public KeyCode nextKey = KeyCode.Return;
        
        public bool CanSkip = true;

        public bool InteractTrigger => UseMouseButtonToInteract && Input.GetMouseButtonDown(0) || !UseMouseButtonToInteract && Input.GetKeyDown(nextKey);
        public bool SkipTrigger => CanSkip && InteractTrigger;

        public float dialogueSpeed = 10;
        [HideInInspector]public bool isTalking;
        private bool isSkipping;
        public Action onDialogueOver;

        private void Awake()
        {
            textEffectController = GetComponent<TextEffectController>();
        }

        private void Start()
        {
            if (IsDebug)
                ShowDialogues(DebugDialogueData);
        }

        protected override void ManagedFixedUpdate()
        {
            base.ManagedFixedUpdate();
            
            // 跳过
            if (SkipTrigger)
                isSkipping = true;
        }

        public void ShowDialogues(params DialogueData[] dialogueDataList)
        {
            StartCoroutine(_ShowDialogues(dialogueDataList));
        }

        private IEnumerator _ShowDialogues(params DialogueData[] dialogueDataList)
        {
            foreach (var dialogue in dialogueDataList)
            {
                yield return StartCoroutine(ShowDialogue(dialogue));
            }

            onDialogueOver?.Invoke();
        }

        private IEnumerator ShowDialogue(DialogueData dialogueData)
        {
            foreach (var content in dialogueData.contents)
            {
                isTalking = true;
                yield return ShowCharOneByOne(content);
                isTalking = false;
                yield return new WaitUntil(() => InteractTrigger);
            }
        }

        private IEnumerator ShowCharOneByOne(string content)
        {
            var dialogueSpeed = this.dialogueSpeed;
            textEffectController.showCharNum = 0;
            textEffectController.RawContent = content;

            isSkipping = false;
            for (int i = 0; i <= textEffectController.ParsedString.Length; i += 1)
            {
                textEffectController.showCharNum = i;
                if (textEffectController.charPosToEventInfo.ContainsKey((int)i))
                {
                    var info = textEffectController.charPosToEventInfo[(int)i];
                    if (info.ContainsKey("speed"))
                    {
                        print(12312);
                        dialogueSpeed = float.Parse(info["speed"]);
                    }

                    if (info.ContainsKey("delay"))
                    {
                        yield return float.Parse(info["delay"]);
                    }
                }

                if (isSkipping)
                    break;

                yield return 1 / dialogueSpeed;
            }

            textEffectController.showCharNum = textEffectController.ParsedString.Length;
        }
    }
}