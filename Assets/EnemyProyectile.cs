using UnityEngine;

public class EnemyProyectile : MonoBehaviour
{
    [SerializeField] int damage;
    [SerializeField] float moveSpeed;
    [SerializeField] float destroyTime = 5;

    private void Awake()
    {
        AddForce(transform.forward);
        Destroy(gameObject, 5);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody && other.attachedRigidbody.TryGetComponent(out PlayerMovement move)) move.RecieveDamage(damage);

        Destroy(gameObject);
    }

    public void AddForce(Vector3 direction)
    {
        GetComponent<Rigidbody>().AddForce(direction.normalized * moveSpeed,ForceMode.Impulse);
    }
}
