using UnityEngine;

public class RangeWeaponAnimationFinishTrigger : MonoBehaviour
{
    [SerializeField] private Weapon weaponHolder;

    [HideInInspector] public Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void Shoot()
    {
        weaponHolder.Shoot();
    }

    public void FinishReload()
    {
        anim.SetBool("Reload", false);
        weaponHolder.canShoot = true;
        weaponHolder.canSwaap = true;
    }

    public void FinishShoot()
    {
        anim.SetBool("Shoot", false);
        weaponHolder.canSwaap = true;
    }
}
