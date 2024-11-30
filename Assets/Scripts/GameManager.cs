using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timeTextMesh;
    [SerializeField] TextMeshProUGUI hpTextMesh;
    [SerializeField] TextMeshProUGUI weaponTextMesh;

    [HideInInspector] public PlayerMovement playerSC;
    [HideInInspector] public Weapon playerWeaponSC;
    [HideInInspector] public int hostageCount;
    [HideInInspector] public int bombCount;

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
    }

    private void Update()
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
    }
    public void AddHostage() => hostageCount++;

    public void AddBomb() => bombCount++;
}
