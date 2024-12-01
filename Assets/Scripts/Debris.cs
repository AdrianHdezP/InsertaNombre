using UnityEngine;

public class Debris : MonoBehaviour
{
    [SerializeField] private int damageAmount = 10; // Daño que causa

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out PlayerMovement playerHealth))
        {
            playerHealth.RecieveDamage(damageAmount);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
