using UnityEngine;

public class FallingDebris : MonoBehaviour
{
    [SerializeField] private GameObject debrisPrefab; // Prefab del escombro
    [SerializeField] private Transform player; // Referencia al jugador
    [SerializeField] private float followHeight = 10f; // Altura del techo invisible
    [SerializeField] private float spawnRadius = 5f; // Radio alrededor del jugador para caer escombros
    [SerializeField] private float spawnInterval = 0.5f; // Intervalo entre caídas

    private bool isSpawning = false;

    private void Start()
    {
        if (player == null)
        {
            Debug.LogError("Player transform is not assigned.");
        }
    }

    private void Update()
    {
        if (player != null)
        {
            // Mantén el techo a cierta altura sobre el jugador
            transform.position = new Vector3(player.position.x, player.position.y + followHeight, player.position.z);
        }
        StartSpawning();
    }

    public void StartSpawning()
    {
        if (!isSpawning)
        {
            isSpawning = true;
            InvokeRepeating(nameof(SpawnDebris), 0f, spawnInterval);
        }
    }

    public void StopSpawning()
    {
        isSpawning = false;
        CancelInvoke(nameof(SpawnDebris));
    }

    private void SpawnDebris()
    {
        // Genera una posición aleatoria alrededor del jugador dentro del radio
        Vector3 spawnPosition = player.position + new Vector3(
            Random.Range(-spawnRadius, spawnRadius),
            0f,
            Random.Range(-spawnRadius, spawnRadius)
        );

        // Ajusta la posición a la altura del techo invisible
        spawnPosition.y = transform.position.y;

        // Instancia el escombro
        CameraShake.instance.cr = StartCoroutine(CameraShake.instance.Shake(1, 0.5f));
        GameObject debris = Instantiate(debrisPrefab, spawnPosition, Quaternion.identity);
        Rigidbody rb = debris.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse); // Simula caída
        }

        Destroy(debris, 5f); // Destruye el escombro tras un tiempo
    }
}
