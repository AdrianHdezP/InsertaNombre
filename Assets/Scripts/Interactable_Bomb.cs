using UnityEngine;

public class Interactable_Bomb : Interactable
{
    [SerializeField] private GameObject bomb;
    [SerializeField] Light prevLight;

    public override void Interact()
    {
        bomb.SetActive(true);
        triggered = true;

        prevLight.enabled = false;
    }
}
