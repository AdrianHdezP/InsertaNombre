using UnityEngine;

public class Interactable_MrChedar : Interactable
{
    private AudioSource audio;

    private void Start()
    {
        audio = GetComponent<AudioSource>();
    }

    public override void Interact()
    {
        audio.Play();   
    }
}
