using UnityEngine;

public class Audios : MonoBehaviour
{
    private static Audios instance;

    [SerializeField] AudioClip audioaa;

    public Audios Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.LogWarning("El audio peero");
            }

            return instance;
        }
    }
}
