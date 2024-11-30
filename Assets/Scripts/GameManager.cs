using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timeTextMesh;
    [SerializeField] TextMeshProUGUI hpTextMesh;
    [SerializeField] TextMeshProUGUI weaponTextMesh;
    [SerializeField] TextMeshProUGUI actionTextMesh;

    [HideInInspector] public PlayerMovement playerSC;
    [HideInInspector] public Weapon playerWeaponSC;
    [HideInInspector] public int hostageCount;
    [HideInInspector] public int bombsLeft;

    Interactable_Bomb[] totalBombs;

    private static GameManager instance;

    public int timeInSeconds;
    float secT;
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
        playerSC = FindFirstObjectByType<PlayerMovement>();
        playerWeaponSC = FindFirstObjectByType<Weapon>();

        totalBombs = FindObjectsByType<Interactable_Bomb>(FindObjectsSortMode.None);
        bombsLeft = totalBombs.Length;
    }

    private void Update()
    {
        UIControl();

    }
    public void AddHostage() => hostageCount++;

    void UIControl()
    {
        if (secT < 1) secT += Time.deltaTime;
        else
        {
            secT = 0;
            timeInSeconds++;

            timeTextMesh.text = (timeInSeconds / 60).ToString("00") + ":" + (timeInSeconds % 60).ToString("00");
        }

        hpTextMesh.text = playerSC.health.ToString("0") + " HP";

        if (playerWeaponSC.isRange) weaponTextMesh.text = "Cleaner 3000 <br>" + playerWeaponSC.magazine + "/" + playerWeaponSC.totalBullets;
        else weaponTextMesh.text = "Power M.O.P. <br> (Melee)";

        if (playerSC.closestInteractable != null)
        {
            actionTextMesh.gameObject.SetActive(true);
            actionTextMesh.text = "Press \"y\" To " + playerSC.closestInteractable.actionText;
        }
        else actionTextMesh.gameObject.SetActive(false);
    }

    void TrackBombsPlanted()
    {
        int count = 0;
        foreach (Interactable_Bomb bomb in totalBombs) 
        { 
            if (!bomb.triggered) count++;
        }

        bombsLeft = count;
    }
}
