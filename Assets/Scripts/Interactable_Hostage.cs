using UnityEngine;

public class Interactable_Hostage : Interactable
{
    public override void Interact()
    {
        GameManager.Instance.AddHostage();
    }
}
