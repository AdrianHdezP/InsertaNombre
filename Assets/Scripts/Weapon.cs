using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private GameObject meleeWeapon;
    [SerializeField] private GameObject rangeWeapon;

    public float fireRate = 1;
    public int magazine = 15;
    public int totalBullets = 60;
    private int bullets = 0;

    [HideInInspector] public bool canReload;
    [HideInInspector] public bool canShoot;

    public GameObject bulletsPrefab;

    private void Start()
    {
        canShoot = true;
        canReload = false;
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
            rangeWeapon.SetActive(false);
            meleeWeapon.SetActive(true);
        }
        else
        {
            rangeWeapon.SetActive(true);
            meleeWeapon.SetActive(false);   
        }
    }

    public void ReloadAction()
    {
        if (CanReload())
        {
            canShoot = false;

            bullets = 15 - magazine;

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
        if (magazine == 0 && totalBullets > 0)
        {
            ReloadAction();
            return;
        }

        if (CanShoot())
            Shoot();
    }

    private bool CanReload() => canReload;

    private bool CanShoot() => canShoot;

    private void Shoot()
    {
        canShoot = false;
        magazine--;
        rangeWeapon.GetComponent<RangeWeaponAnimationFinishTrigger>().anim.SetBool("Shoot", true);
        StartCoroutine(FireRate());
    }

    private IEnumerator FireRate()
    {
        yield return new WaitForSeconds(fireRate);
        canShoot = true;
    }
}
