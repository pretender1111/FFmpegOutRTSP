using UnityEngine;

public class Timer : MonoBehaviour
{
    public TextMesh timerText; // 用于显示时间的 TextMesh 组件
    private float elapsedTime; // 经过的时间

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
