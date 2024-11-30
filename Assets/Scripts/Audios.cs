using UnityEngine;

public class Audios : MonoBehaviour
{
    private static Audios instance;

    [Header("Enemy Effects")]
    public AudioClip enemyRangeAttack;

    [Header("Weapon Effects")]
    public AudioClip rangeAttack;
    public AudioClip rangeReload;
    public AudioClip meleeAttack;

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

    private void Awake()
    {
        instance = this;
    }
}
