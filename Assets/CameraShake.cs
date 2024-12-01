using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake instance;

    [HideInInspector] public Coroutine cr;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public IEnumerator Shake(float duration, float magnitude)
    {
        if(cr != null)
        {
            Vector3 originalPosition = transform.localPosition; // Posición inicial
            float elapsed = 0.0f;

            while (elapsed < duration)
            {
                float x = Random.Range(-1f, 1f) * magnitude;
                float y = Random.Range(-1f, 1f) * magnitude;

                transform.localPosition = new Vector3(originalPosition.x + x, originalPosition.y + y, originalPosition.z);

                elapsed += Time.deltaTime;

                yield return null;
            }
            cr = null;
            transform.localPosition = originalPosition; // Volver a la posición original

        }
    }
}
