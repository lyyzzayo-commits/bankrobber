using System.Collections;
using TMPro;
using UnityEngine;

public class DeathUI : MonoBehaviour
{
    [SerializeField] private Health playerHealth;
    [SerializeField] private Canvas deathCanvas;
    [SerializeField] private RespawnManager respawnManager;
    [SerializeField] private TextMeshProUGUI countdownText;

    [SerializeField] private float totalSeconds = 3f;

    private Coroutine countdownCo;

   

    private void OnEnable()
    {
        playerHealth.onDied += StartCountdown;
        respawnManager.OnRespawnd += Hide;

    }

    private void OnDisable()
    {
        playerHealth.onDied -= StartCountdown;
        respawnManager.OnRespawnd -= Hide;
    }

    private void StartCountdown(Health _)
    {
        deathCanvas.enabled = true;

        if(countdownCo != null) StopCoroutine(countdownCo);
        countdownCo = StartCoroutine(CountDownRoutine(totalSeconds));
    }

    private IEnumerator CountDownRoutine(float totalSeconds)
    {
        float remaining = totalSeconds;

        while (remaining > 0f)
        {
            int sec = Mathf.CeilToInt(remaining);
            countdownText.text = $"Respawn in {sec}";

            yield return new WaitForSeconds(1f);
            remaining -= 1f;
        }

        countdownText.text = "Respawning...";
        countdownCo = null;
    }

    private void Hide()
    {
        if (countdownCo != null)
        {
            StopCoroutine(countdownCo);
            countdownCo = null;
        }

        deathCanvas.enabled = false;
    }
}
