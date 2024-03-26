using UnityEngine;

public class Timer : MonoBehaviour
{
    public TextMesh timerText; // ������ʾʱ��� TextMesh ���
    private float elapsedTime; // ������ʱ��

    private void Update()
    {
        elapsedTime += Time.deltaTime;
        UpdateTimerText();
    }

    private void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);
        float milliseconds = (elapsedTime * 1000) % 1000;

        timerText.text = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
    }
}
