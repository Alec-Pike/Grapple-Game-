using UnityEngine;
using TMPro;

/*
* Based on a tutorial by BMo on YouTube
* Link: https://www.youtube.com/watch?v=u_n3NEi223E
*/
public class MyTimer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMesh;
    [SerializeField] private bool timerIsRunning = true;
    private float currentTime = 0f;
    private float minutes, seconds;
    private string displayString, extraZero;

    private void Update()
    {
        if (!timerIsRunning) return;

        // increment timer
        currentTime += Time.deltaTime;

        // break into minutes and seconds
        minutes = Mathf.Floor(currentTime / 60);
        seconds = currentTime % 60;

        // throw in an extra zero if needed to keep the seconds place as double digits
        extraZero = (seconds < 10f) ? "0" : "";
        // build the string
        displayString = minutes.ToString() + ":" + extraZero + seconds.ToString("0.00");
        // display
        textMesh.text = displayString;
    }

    public void StopTimer()
    {
        timerIsRunning = false;
    }

    public void StartTimer()
    {
        timerIsRunning = true;
    }
}