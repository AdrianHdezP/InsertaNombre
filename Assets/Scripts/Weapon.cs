using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Weapon : MonoBehaviour
{
    [SerializeField] private AudioSource weaponEffects;
    [SerializeField] private Audios audios;
    private PlayerMovement playerMovement;
    private InputSystem_Actions inputSystemActions;

    [SerializeField] private GameObject meleeWeapon;
    [SerializeField] private GameObject rangeWeapon;

    public float fireRate = 1;
    public float meleeRate = 1;
    public int magazine = 15;
    public int totalBullets = 60;


    [HideInInspector] public bool canReload;
    [HideInInspector] public bool canSwaap;
    [HideInInspector] public bool canShoot;
    [HideInInspector] public bool pressed;
    [HideInInspector] public bool isRange;
    [HideInInspector] public bool click;

    public Proyectile bulletsPrefab;
    public Proyectile meleePrefab;

    private void Awake()
    {
        inputSystemActions = new InputSystem_Actions();
        inputSystemActions.Player.Enable();
    }


    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        inputSystemActions.Player.Shoot.performed += ShootingPerformed;
        inputSystemActions.Player.Shoot.canceled += ShootingCanceled;

        canShoot = true;
        canReload = false;
        isRange = true;
        canSwaap = true;
        click = false;
        
    }

    private void Update()
    {
        if (magazine < 15 && totalBullets > 0 && isRange)
            canReload = true;

        if (magazine <= 0)
            canShoot = false;

        if (pressed)
            ShootAction();

        if (playerMovement.input != Vector2.zero)
        {
            if (isRange)
            {
                rangeWeapon.GetComponent<RangeWeaponAnimationFinishTrigger>().anim.SetBool("Move", true);
            }
            else
            {
                meleeWeapon.GetComponent<MeleeWeaponAnimationFinishTrigger>().anim.SetBool("Move", true);
            }
        }
        else
        {
            if (isRange)
            {
                rangeWeapon.GetComponent<RangeWeaponAnimationFinishTrigger>().anim.SetBool("Move", false);
            }
            else
            {
                meleeWeapon.GetComponent<MeleeWeaponAnimationFinishTrigger>().anim.SetBool("Move", false);
            }
        }
    }

    public void ChangeWeaponAction()
    {
        if (!canSwaap)
            return;

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

        canSwaap = true;
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
            canSwaap = false;
            weaponEffects.volume = 1;
            weaponEffects.PlayOneShot(audios.rangeReload);

            canReload = false;  
        }
    }

    private void ShootingPerformed(InputAction.CallbackContext context) => pressed = true;

    private void ShootingCanceled(InputAction.CallbackContext context) => pressed = false;

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
                ShootAnimation();

            if (magazine == 0 && totalBullets == 0 && !click)
            {        
                StartCoroutine(ClickNoise());
            }
        }
        else
        {
            if (canSwaap)
            {
                Melee();
            }
        }
    }

    IEnumerator ClickNoise()
    {
        click = true;
        weaponEffects.volume = 0.6f;
        weaponEffects.PlayOneShot(audios.emptyGun);
        yield return new WaitForSeconds(1);
        click = false;
    }

    private bool CanReload() => canReload;

    private bool CanShoot() => canShoot;

    private void ShootAnimation()
    {
        rangeWeapon.GetComponent<RangeWeaponAnimationFinishTrigger>().anim.SetBool("Shoot", true);
        canSwaap = false;
    }

    public void Shoot()
    {
        canShoot = false;
        magazine--;
        weaponEffects.volume = 1;
        weaponEffects.PlayOneShot(audios.rangeAttack);
        Proyectile proyectile = Instantiate(bulletsPrefab, Camera.main.transform.position, Quaternion.LookRotation(Camera.main.transform.forward));
        StartCoroutine(FireRate());
    }

    private void Melee()
    {
        canShoot = false;
        weaponEffects.volume = 0.25f;
        meleeWeapon.GetComponent<MeleeWeaponAnimationFinishTrigger>().anim.SetTrigger("Swing");
        canSwaap = false;
        weaponEffects.PlayOneShot(audios.meleeAttack);
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
