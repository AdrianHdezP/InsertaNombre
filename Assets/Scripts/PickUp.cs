using UnityEngine;

public class PickUp : MonoBehaviour
{
    [SerializeField] Types type;
    [SerializeField] int amount;

    private AudioSource audioSource;
    enum Types
    {
        ammo,
        heal
    }
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody && other.attachedRigidbody.TryGetComponent(out PlayerMovement move))
        {
            if (type == Types.heal && move.health < move.maxHealth)
            {
                audioSource.PlayOneShot(Audios.instance.healItem);
                move.HealDamage(amount);
                Notifier.instance.AddNotice("You have picked up a heal", Color.red);
                Destroy(gameObject, 0.5f);
            }
        }

        if (other.attachedRigidbody && other.attachedRigidbody.TryGetComponent(out Weapon weapon))
        {
            if (type == Types.ammo)
            {
                audioSource.PlayOneShot(Audios.instance.ammoItem);
                weapon.RestoreAmmo(amount);
                Notifier.instance.AddNotice("You have picked up " + amount + " ammo", Color.blue);
                Destroy(gameObject, 0.5f);
            }
        }

    }
}
