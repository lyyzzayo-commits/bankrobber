using UnityEngine;

public class CanvasToggle : MonoBehaviour
{
    [SerializeField] private Canvas canvas;

    private void OnEnable()
    {
        LookController.OnAimStateChanged += SetVisible;
    }

    private void OnDisable()
    {
        LookController.OnAimStateChanged -= SetVisible;
    }

    // 이벤트 콜백 시그니처를 bool로 맞춘다
    private void SetVisible(bool isAiming)
    {
        if (canvas == null) return;

        // 방법 1: Canvas 자체 활성/비활성
        canvas.gameObject.SetActive(isAiming);

        // 방법 2(대안): Canvas.enabled 사용
        // canvas.enabled = isAiming;
    }
}

