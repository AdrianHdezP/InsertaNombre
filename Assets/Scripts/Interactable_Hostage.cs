using UnityEngine;

public class Interactable_Hostage : Interactable
{
    private AudioSource audio;
    public SpriteRenderer s_renderer;

    public ParticleSystem particleS;
    public override void Interact()
    {
        GameManager.Instance.AddHostage();
        Notifier.instance.AddNotice("You have found a lost animal", Color.yellow);
        triggered = true;
        audio.Play();
        particleS.Play();
    }

    private void Start()
    {
        audio = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (triggered) 
        {
            if (!s_renderer.isVisible)
            {
                Destroy(gameObject);
            }
        }
    }
}
