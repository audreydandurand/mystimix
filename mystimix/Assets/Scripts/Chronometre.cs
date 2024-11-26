using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Chronometre : MonoBehaviour
{
    public TMP_Text timerText;
    private float elapsedTime = 0f;
    private bool isRunning = true;
    public float finalTime { get; private set; } = 0f;

    void Update()
    {
        if (isRunning)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimerDisplay();
        }
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    private void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int secondes = Mathf.FloorToInt(elapsedTime % 60f);
        int millisecondes = Mathf.FloorToInt((elapsedTime * 1000) % 1000);

        timerText.text = string.Format("{0:00}:{1:00}:{2:000}", minutes, secondes, millisecondes);
    }

    public string GetFormattedFinalTime()
    {
        int minutes = Mathf.FloorToInt(finalTime / 60f);
        int secondes = Mathf.FloorToInt(finalTime % 60f);
        int millisecondes = Mathf.FloorToInt((finalTime * 1000) % 1000);

        return string.Format("{0:00}:{1:00}:{2:000}", minutes, secondes, millisecondes);
    }
}
