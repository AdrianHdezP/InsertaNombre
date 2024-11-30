using UnityEngine;

public class PickUp : MonoBehaviour
{
    [SerializeField] Types type;
    [SerializeField] int amount;
    enum Types
    {
        ammo,
        heal
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody && other.attachedRigidbody.TryGetComponent(out PlayerMovement move))
        {
            if (type == Types.heal && move.health < move.maxHealth)
            {
                move.HealDamage(amount);
                Destroy(gameObject);
            }
        }

        if (other.attachedRigidbody && other.attachedRigidbody.TryGetComponent(out Weapon weapon))
        {
            if (type == Types.ammo)
            {
                weapon.RestoreAmmo(amount);
                Destroy(gameObject);
            }
        }

    }
}
