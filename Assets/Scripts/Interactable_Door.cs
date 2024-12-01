using UnityEngine;

public class Interactable_Door : Interactable
{
    private Animator anim;
    private AudioSource audiosource;

    private void Start()
    {
        anim = GetComponent<Animator>();
        audiosource = GetComponent<AudioSource>();
    }

    public override void Interact()
    {
        audiosource.Play(); 
        GameManager.Instance.playerSC.anim.SetTrigger("Kick");
        anim.SetBool("Open", true);
        Notifier.instance.AddNotice("You have opened a door", Color.yellow);
        triggered = true;
    }
}
