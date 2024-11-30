using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timeTextMesh;
    [SerializeField] TextMeshProUGUI hpTextMesh;
    [SerializeField] TextMeshProUGUI weaponTextMesh;
    [SerializeField] TextMeshProUGUI actionTextMesh;

    [SerializeField] Image faceImage;
    [SerializeField] Sprite[] faceSprites;

    [HideInInspector] public PlayerMovement playerSC;
    [HideInInspector] public Weapon playerWeaponSC;
    [HideInInspector] public int hostageCount;
    public int bombsLeft;
    private bool ending;

    Interactable_Bomb[] totalBombs;

    private static GameManager instance;

    [SerializeField] float secondsLeft;
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
        TrackBombsPlanted();
        LoseCondition();

        if (timeInSeconds <= 0 || secondsLeft <= 0)
            timeTextMesh.text = "00:00";
    }

    public void AddHostage() => hostageCount++;

    void UIControl()
    {
        if (ending == false)
        {
            if (secT < 1)
            {
                secT += Time.deltaTime;
            }
            else
            {
                secT = 0;
                timeInSeconds++;

                timeTextMesh.text = Mathf.FloorToInt(timeInSeconds / 60f).ToString("00") + ":" + (timeInSeconds % 60).ToString("00");
            }
        }

        hpTextMesh.text = playerSC.health.ToString("0") + " HP";
        faceImage.sprite = faceSprites[Mathf.RoundToInt((faceSprites.Length - 1) * ((float)playerSC.health / playerSC.maxHealth))];

        if (playerWeaponSC.isRange) weaponTextMesh.text = "Cleaner 3000 <br>" + playerWeaponSC.magazine + "/" + playerWeaponSC.totalBullets;
        else weaponTextMesh.text = "Power M.O.P. <br> (Melee)";

        if (playerSC.closestInteractable != null)
        {
            actionTextMesh.gameObject.SetActive(true);
            actionTextMesh.text = "Press \"Y\" To " + playerSC.closestInteractable.actionText;
        }
        else actionTextMesh.gameObject.SetActive(false);
    }

     private void TrackBombsPlanted()
    {
        int count = 0;
        foreach (Interactable_Bomb bomb in totalBombs) 
        { 
            if (!bomb.triggered) count++;
        }

        bombsLeft = count;

        if (bombsLeft <= 0 && ending == false)
        {
            ending = true;
            StartCoroutine(PizzaTowerTime());
        }
    }

    IEnumerator PizzaTowerTime()
    {
        Debug.Log("Llamada");
        yield return new WaitForSeconds(2);
        Debug.Log("Acaba llamada");

        timeTextMesh.color = Color.red;
        timeTextMesh.text = Mathf.FloorToInt(secondsLeft / 60f).ToString("00") + ":" + (secondsLeft % 60).ToString("00");

        float secT = 0;

        while (secondsLeft > 0)
        {
            if (secT < 1) secT += Time.deltaTime;
            else
            {
                secT = 0;
                secondsLeft--;

                timeTextMesh.text = Mathf.FloorToInt(secondsLeft / 60f).ToString("00") + ":" + (secondsLeft % 60).ToString("00");
            }

            yield return null;
        }

        playerSC.RecieveDamage(999);
    }

    private void LoseCondition()
    {
        if (playerSC.health <= 0)
            Lose();
    }

    private void Lose()
    {
        Debug.Log("Has perdido pringado");
        //Pantalla titulo
    }
}
