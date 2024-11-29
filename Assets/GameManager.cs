using UnityEngine;

public class GameManager : MonoBehaviour
{
    [HideInInspector] public int hostageCount;
    [HideInInspector] public int bombCount;

    private static GameManager instance;

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.LogWarning("El manager peero");
            }

            return instance;
        }
    }

    private void Awake()
    {
        instance = this;
    }

    public void AddHostage() => hostageCount++;

    public void AddBomb() => bombCount++;
}
