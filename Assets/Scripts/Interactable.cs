using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public abstract void Interact();
    public string actionText;
    public bool triggered;
}
