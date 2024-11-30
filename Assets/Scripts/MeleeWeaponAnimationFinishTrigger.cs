using UnityEngine;

public class MeleeWeaponAnimationFinishTrigger : MonoBehaviour
{
    [SerializeField] private Weapon weaponHolder;

    [HideInInspector] public Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void SwingMopTrigger()
    {
        weaponHolder.MeleeProyectileSpawn();
    }
}
