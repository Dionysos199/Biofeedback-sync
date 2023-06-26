using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class Timer : MonoBehaviour 
{
   

    [SerializeField] private Image uiFill; // Reference to the UI fill image component
    [SerializeField] private TextMeshProUGUI uiText; // Reference to the UI text component

    public int Duration; // The total duration of the timer in seconds

    private int remainingDuration; // The remaining duration of the timer in seconds

    private bool Pause; // Indicates whether the timer is paused or not

    private void Start()
    {
        Begin(Duration); // Start the timer with the specified duration
    }

    private void Begin(int seconds)
    {
        remainingDuration = seconds; // Set the remaining duration to the specified seconds
        StartCoroutine(UpdateTimer()); // Start the coroutine to update the timer
    }

    private IEnumerator UpdateTimer()
    {
        while (remainingDuration >= 0)
        {
            if (!Pause) // If the timer is not paused
            {
                // Update the UI text to display the remaining minutes and seconds
                uiText.text = $"{remainingDuration / 60:00}:{remainingDuration % 60:00}";

                // Update the UI fill amount to reflect the progress of the timer
                uiFill.fillAmount = Mathf.InverseLerp(0, Duration, remainingDuration);

                remainingDuration--; // Decrease the remaining duration by 1 second
                yield return new WaitForSeconds(1f); // Wait for 1 second before the next iteration
            }
            yield return null; // Yield to allow other processes to run
        }
    }
}



