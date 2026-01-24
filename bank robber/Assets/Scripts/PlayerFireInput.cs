using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerFireInput : MonoBehaviour
{
    [SerializeField] private WeaponController weapon;

    public void OnAttack(InputValue value)
    {
        if (value.isPressed)
            weapon.Fire();
    }
}
