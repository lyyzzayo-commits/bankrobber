using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerFireInput : MonoBehaviour
{
    [SerializeField] private WeaponController weapon;

    public void OnAttack(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
            weapon.Fire();
    }
}
