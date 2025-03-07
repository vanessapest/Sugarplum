using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public Slider timerSlider;
    public Text timerText;
    public GameObject currentSign;
    public GameObject nextSign;
    public GameObject finishedSign;

    public float totalTime; // total time for the timer
    public float remainingTime; // how much time is left
    public bool isRunning = false; // tracks whether the timer is currently active
    public bool isBaking = true;
    public bool isDisplayingUI = false;
    public float displayTimeRemaining = 0f; // time remaining for the slider and text to stay visible

    void Start()
    {
        timerSlider.gameObject.SetActive(false);
        timerText.gameObject.SetActive(false);
    }

    // the timer start for a task; initializes the total time
    public void StartTimer(float cakeTotalTime)
    {
        totalTime = cakeTotalTime; // set the timer
        remainingTime = totalTime;  // start with the full time remaining
        isBaking = true;
        isRunning = true;
        UpdateTimerUI();
    }

    public void ResumeTimer()
    {
        isRunning = true;
        isBaking = false; // switch to decorating sign
        UpdateTimerUI();
    }

    void Update()
    {
        if (isRunning) // if the timer is active
        {
            remainingTime -= Time.deltaTime; // decrease the remaining time
            UpdateTimerUI();

            if (isBaking && remainingTime <= totalTime / 2f) 
            {
                PauseAtHalfway();
            }
            else if (!isBaking && remainingTime <= 0f)
            {
                FinishTimer();
            }
        }

        // handle slider and text visibility timer
        if (isDisplayingUI)
        {
            displayTimeRemaining -= Time.deltaTime; // count down the display timer
            if (displayTimeRemaining <= 0f)
            {
                HideUI();
            }
        }
    }

    // when halfway through the baking time
    public void PauseAtHalfway()
    {
        isRunning = false; // pause the timer

        // hide the current sign (baking phase complete)
        if (currentSign != null)
        {
            currentSign.SetActive(false);
        }

        // show the next sign (decorating phase)
        if (nextSign != null)
        {
            nextSign.SetActive(true);
        }
    }

    // when the timer reaches 0
    public void FinishTimer()
    {
        isRunning = false; // stop the timer

        // hide the decorating sign
        if (nextSign != null)
        {
            nextSign.SetActive(false);
        }

        // show the finished sign
        if (finishedSign != null)
        {
            finishedSign.SetActive(true);
        }

        // hide the UI when the process finishes
        HideUI();
    }

    public void ShowUIForFiveSeconds()
    {
        // only show the UI if the oven is active
        if (!isRunning) return;

        // show the UI (slider and timer text)
        timerSlider.gameObject.SetActive(true);
        timerText.gameObject.SetActive(true);

        // start the timer for displaying the UI
        isDisplayingUI = true;
        displayTimeRemaining = 5f;
    }

    // hide the timer UI manually or when 5 seconds pass
    public void HideUI()
    {
        timerSlider.gameObject.SetActive(false);
        timerText.gameObject.SetActive(false);
        isDisplayingUI = false;
    }

    public void UpdateTimerUI()
    {
        if (timerSlider != null)
        {
            timerSlider.value = remainingTime / totalTime; // adjust the slider's position
        }

        if (timerText != null)
        {
            // Format the remaining time as hh:mm:ss
        System.TimeSpan time = System.TimeSpan.FromSeconds(remainingTime);
        timerText.text = string.Format("{0:D2}:{1:D2}:{2:D2}", time.Hours, time.Minutes, time.Seconds);
        }
    }
    
    public float GetRemainingTime()
    {
        return remainingTime;
    }
}
