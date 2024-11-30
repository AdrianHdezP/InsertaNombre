using UnityEngine;

public class RangeWeaponAnimationFinishTrigger : MonoBehaviour
{
    [SerializeField] private Weapon weaponHolder;

    [HideInInspector] public Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void FinishReload()
    {
        anim.SetBool("Reload", false);
        weaponHolder.canShoot = true;
    }

    public void FinishShoot()
    {
        anim.SetBool("Shoot", false);
    }
}
