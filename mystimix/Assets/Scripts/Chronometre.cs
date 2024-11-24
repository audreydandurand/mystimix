using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chronometre : MonoBehaviour
{
    public Text timerText; // Assign a UI Text element to display the time
    private float elapsedTime = 0f; // Tracks the elapsed time
    private bool isRunning = true; // Indicates if the chronometer is active

    void Update()
    {
        if (isRunning)
        {
            // Increment elapsed time
            elapsedTime += Time.deltaTime;

            // Update the timer text
            UpdateTimerDisplay();
        }
    }

    public void StopTimer()
    {
        isRunning = false; // Stop the chronometer
    }

    private void UpdateTimerDisplay()
    {
        // Format the time as minutes:seconds:milliseconds
        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);
        int milliseconds = Mathf.FloorToInt((elapsedTime * 1000) % 1000);

        timerText.text = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
    }
}
