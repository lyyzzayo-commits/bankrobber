using System;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    [SerializeField] private Health playerhealth;
    [SerializeField] private Transform respawnPoint;

    public event Action OnRespawnd;

    private void OnEnable()
    {
        playerhealth.onDied += HandlePlayerDied;
    }

    private void OnDisable()
    {
        playerhealth.onDied -= HandlePlayerDied;
    }

    private void HandlePlayerDied(Health deadHealth)
    {
        StartCoroutine(RespawnRoutine(deadHealth));
    }

    private System.Collections.IEnumerator RespawnRoutine(Health health)
    {
        health.gameObject.SetActive(false);

        yield return new WaitForSeconds(2f);

        health.transform.position = respawnPoint.position;

        health.ResetHp();

        health.gameObject.SetActive(true);

        OnRespawnd?.Invoke();
    }
} 
