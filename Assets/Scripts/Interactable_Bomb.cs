using UnityEngine;

public class Interactable_Bomb : Interactable
{
    [SerializeField] private GameObject bomb;
    [SerializeField] Light prevLight;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public override void Interact()
    {
        bomb.SetActive(true);
        audioSource.Play();
        triggered = true;

        prevLight.enabled = false;
    }
}
