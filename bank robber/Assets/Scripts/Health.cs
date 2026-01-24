using System;
using UnityEngine;
using UnityEngine.Rendering;
public class Health : MonoBehaviour, IDamageable
{
    [SerializeField] public int maxhp = 100;
    [SerializeField] public int currenthp;
    public event Action<Health> onDied;

    public void TakeDamage(int damage)
    {
        currenthp -= damage;
        Debug.Log(currenthp);
        if (currenthp <= 0)
            Die();
    }

    private void Die()
    {
        onDied?.Invoke(this);
    }

    public void ResetHp()
    {
        currenthp = maxhp;
    }
}
