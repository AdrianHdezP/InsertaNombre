using UnityEngine;

public class MeleeWeaponAnimationFinishTrigger : MonoBehaviour
{
    [SerializeField] private Weapon weaponHolder;

    [HideInInspector] public Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void SwingMopTrigger()
    {
        weaponHolder.MeleeProyectileSpawn();
    }
}
