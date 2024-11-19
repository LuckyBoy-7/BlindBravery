using Lucky.Framework;
using UnityEngine;

public class Test : ManagedBehaviour
{
    public AudioSource PreSource;
    public AudioClip Clip;


    private void Awake()
    {
    }

    protected override void ManagedFixedUpdate()
    {
        base.ManagedFixedUpdate();
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (PreSource)
                PreSource.Pause();
            AudioSource source;
            string name = "Audio_" + Clip.name;
            GameObject go = new GameObject(name);
            source = go.AddComponent<AudioSource>();

            source.clip = Clip;
            source.loop = false;
            source.volume = 1;
            source.playOnAwake = false;
            source.Play();
            PreSource = source;
        }
    }
}