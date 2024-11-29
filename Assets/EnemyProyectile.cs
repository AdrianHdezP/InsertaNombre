using UnityEngine;

public class EnemyProyectile : MonoBehaviour
{
    [SerializeField] int damage;
    [SerializeField] float moveSpeed;


    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody.TryGetComponent(out PlayerMovement move)) move.RecieveDamage(damage);

        Destroy(gameObject);
    }

    public void AddForce(Vector3 direction)
    {
        GetComponent<Rigidbody>().AddForce(direction.normalized * moveSpeed);
    }
}
