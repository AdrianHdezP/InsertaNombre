using UnityEngine;

public class Interactable_Bomb : Interactable
{
    [SerializeField] private GameObject bomb;

    public override void Interact()
    {
        bomb.SetActive(true);

        GameManager.Instance.AddBomb();
    }
}
