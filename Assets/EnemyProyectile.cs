using UnityEngine;

public class EnemyProyectile : MonoBehaviour
{
    [SerializeField] int damage;

    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody.TryGetComponent(out PlayerMovement move)) move.RecieveDamage(damage);

        Destroy(gameObject);
    }
}
