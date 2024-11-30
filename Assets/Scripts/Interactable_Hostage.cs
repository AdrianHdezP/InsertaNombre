using UnityEngine;

public class Interactable_Hostage : Interactable
{
    private AudioSource audio;
    public SpriteRenderer s_renderer;
    public override void Interact()
    {
        GameManager.Instance.AddHostage();
        triggered = true;
        audio.Play();
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
                Destroy(gameObject, 1);
            }
        }
    }
}
