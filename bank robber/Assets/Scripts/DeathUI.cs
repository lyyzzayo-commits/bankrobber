using UnityEngine;

public class DeathUI : MonoBehaviour
{
    [SerializeField] private Health playerHealth;
    [SerializeField] private Canvas deathCanvas;

    private void OnEnable()
    {
        playerHealth.onDied += Show;
    }

    private void OnDisable()
    {
        playerHealth.onDied -= Show;
    }

    private void Show(Health _)
    {
        deathCanvas.enabled = true;
    }
}
