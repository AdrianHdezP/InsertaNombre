using UnityEngine;

public class Proyectile : MonoBehaviour
{
    [SerializeField] int damage;
    [SerializeField] float moveSpeed;
    [SerializeField] float destroyTime = 5;

    private void Awake()
    {
        AddForce(transform.forward);
        Destroy(gameObject, destroyTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody && other.attachedRigidbody.TryGetComponent(out PlayerMovement move)) move.RecieveDamage(damage);
        else if (other.transform.TryGetComponent(out EnemyController enemy)) enemy.RecieveDamage(damage);

        Destroy(gameObject);
    }

    public void AddForce(Vector3 direction)
    {
        GetComponent<Rigidbody>().AddForce(direction.normalized * moveSpeed,ForceMode.Impulse);
    }
}
