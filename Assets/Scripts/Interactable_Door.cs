using UnityEngine;

public class Interactable_Door : Interactable
{
    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public override void Interact()
    {
        anim.SetBool("Open", true);
    }
}
