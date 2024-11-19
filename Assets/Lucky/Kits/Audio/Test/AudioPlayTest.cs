using Lucky.Framework;
using Lucky.Framework.Inputs_;
using Lucky.Kits.Utilities;
using TMPro;
using UnityEngine;

namespace Lucky.Kits.Audio.Test
{
    public class AudioPlayTest : ManagedBehaviour
    {
        private AudioClip[] audios;
        private int i = 0;
        public TMP_Text Text;
        private AudioSource source;

        private void Awake()
        {
            audios = ResourcesUtils.LoadAll<AudioClip>("Audio/Test");
        }

        protected override void ManagedFixedUpdate()
        {
            base.ManagedFixedUpdate();

            if (Input.GetKeyDown(KeyCode.P))
            {
                AudioController.Instance.PlayLoop("KissingDisease", 0);
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                StartCoroutine(AudioController.Instance.CrossFade("KissingDisease", 1, 2, 0));
            }

            if (audios.Length == 0)
                return;

            if (Inputs.MenuDown.Pressed)
                i = MathUtils.Mod(i + 1, audios.Length);
            else if (Inputs.MenuUp.Pressed)
                i = MathUtils.Mod(i - 1, audios.Length);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                AudioController.Instance.SetAllSfxPausedState(true);
                source = AudioController.Instance.PlaySound2D(audios[i]);
                // source = AudioController.Instance.PlaySound2D("Retro Alarm 02");
            }

            if (Input.GetKeyDown(KeyCode.B))
            {
                AudioController.Instance.SetAllSfxPausedState(false);
            }

            // if (source)
            // print(source.time);

            if (Text)
                Text.text = audios[i].name;
        }
    }
}