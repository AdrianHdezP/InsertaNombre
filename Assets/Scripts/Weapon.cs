using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Weapon : MonoBehaviour
{
    private InputSystem_Actions inputSystemActions;

    [SerializeField] private GameObject meleeWeapon;
    [SerializeField] private GameObject rangeWeapon;

    public float fireRate = 1;
    public float meleeRate = 1;
    public int magazine = 15;
    public int totalBullets = 60;


    [HideInInspector] public bool canReload;
    [HideInInspector] public bool canShoot;
    [HideInInspector] public bool pressed;
    [HideInInspector] public bool isRange;

    public Proyectile bulletsPrefab;
    public Proyectile meleePrefab;

    private void Awake()
    {
        inputSystemActions = new InputSystem_Actions();
        inputSystemActions.Player.Enable();
    }


    private void Start()
    {
        inputSystemActions.Player.Shoot.performed += ShootingPerformed;
        inputSystemActions.Player.Shoot.canceled += ShootingCanceled;

        canShoot = true;
        canReload = false;
        isRange = true;
    }

    private void Update()
    {
        if (magazine < 15 && totalBullets > 0)
            canReload = true;

        if (magazine <= 0)
            canShoot = false;

        if (pressed)
            ShootAction();
    }

    public void ChangeWeaponAction()
    {
        if (rangeWeapon.activeSelf == true)
        {
            isRange = false;
            rangeWeapon.SetActive(false);
            meleeWeapon.SetActive(true);
        }
        else
        {
            isRange = true;
            rangeWeapon.SetActive(true);
            meleeWeapon.SetActive(false);   
        }
    }

    public void ReloadAction()
    {
        if (CanReload())
        {
            canShoot = false;
            

            int bullets = 15 - magazine;

            if (totalBullets + magazine >= 15)
            {
                totalBullets -= bullets;
                magazine += bullets;
            }
            else
            {
                magazine += totalBullets;
                totalBullets = 0;
            }

            rangeWeapon.GetComponent<RangeWeaponAnimationFinishTrigger>().anim.SetBool("Reload", true);

            canReload = false;  
        }
    }

    private void ShootingPerformed(InputAction.CallbackContext context)
    {
        pressed = true;
    }

    private void ShootingCanceled(InputAction.CallbackContext context)
    {
        pressed = false;
    }

    public void ShootAction()
    {
        if (isRange)
        {
            if (magazine == 0 && totalBullets > 0)
            {
                ReloadAction();
                return;
            }

            if (CanShoot())
                Shoot();
        }
        else
        {
            if (CanShoot())
            {
                Melee();
            }
        }
    }

    private bool CanReload() => canReload;

    private bool CanShoot() => canShoot;

    private void Shoot()
    {
        canShoot = false;
        magazine--;
        rangeWeapon.GetComponent<RangeWeaponAnimationFinishTrigger>().anim.SetBool("Shoot", true);
        Proyectile proyectile = Instantiate(bulletsPrefab, Camera.main.transform.position , Quaternion.LookRotation(Camera.main.transform.forward));
        StartCoroutine(FireRate());
    }

    private void Melee()
    {
        canShoot = false;
        meleeWeapon.GetComponent<MeleeWeaponAnimationFinishTrigger>().anim.SetTrigger("Swing");
        StartCoroutine(MeleeRate());
    }
    public void MeleeProyectileSpawn() //ON ANIM
    {
        Proyectile proyectile = Instantiate(meleePrefab, Camera.main.transform.position, Quaternion.LookRotation(Camera.main.transform.forward));
    }

    private IEnumerator FireRate()
    {
        float t = 0;

        while (t < fireRate)
        {
            t += Time.deltaTime;
            yield return null;
        }

        canShoot = true;
    }
    private IEnumerator MeleeRate()
    {
        float t = 0;

        while (t < meleeRate)
        {
            t += Time.deltaTime;
            yield return null;
        }

        canShoot = true;
    }

    public void RestoreAmmo(int amount)
    {
        totalBullets += amount;
    }
}
