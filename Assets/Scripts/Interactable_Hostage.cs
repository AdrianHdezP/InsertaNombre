using UnityEngine;

public class Interactable_Hostage : Interactable
{
    public SpriteRenderer s_renderer;
    public override void Interact()
    {
        GameManager.Instance.AddHostage();
        triggered = true;
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
