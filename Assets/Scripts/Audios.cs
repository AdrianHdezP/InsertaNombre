using UnityEngine;

public class Audios : MonoBehaviour
{
    public static Audios instance;

    [Header("Enemy Effects")]
    public AudioClip enemyRangeAttack;
    public AudioClip enemyRangeDying;

    [Header("Weapon Effects")]
    public AudioClip rangeAttack;
    public AudioClip rangeReload;
    public AudioClip meleeAttack;

    [Header("PickUp Effects")]
    public AudioClip healItem;
    public AudioClip ammoItem;

    [Header("PickUp Effects")]
    public AudioClip davidStart;
    public AudioClip davidEnd;


    private AudioSource audioSource;
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
        audioSource = GetComponent<AudioSource>();
    }
    private void Start()
    {
        audioSource.Play();
    }
}
