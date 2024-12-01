using TMPro;
using UnityEngine;

public class Notice : MonoBehaviour
{
    TextMeshProUGUI textMesh;
    public string text;
    public float timeToDespawn;
    public Color color;


    private void Start()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        textMesh.text = text;
        textMesh.color = color;
    }

    private void Update()
    {
        if (timeToDespawn > 0)
        {
            timeToDespawn -= Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
