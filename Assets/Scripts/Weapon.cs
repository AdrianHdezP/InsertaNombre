using System.Collections;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Weapon : MonoBehaviour
{
    [SerializeField] private GameObject meleeWeapon;
    [SerializeField] private GameObject rangeWeapon;

    public float fireRate = 1;
    public float meleeRate = 1;
    public int magazine = 15;
    public int totalBullets = 60;


    [HideInInspector] public bool canReload;
    [HideInInspector] public bool canShoot;

    public Proyectile bulletsPrefab;
    public Proyectile meleePrefab;

    public bool isRange;

    private void Start()
    {
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
