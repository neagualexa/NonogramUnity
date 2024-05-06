using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelTimer : MonoBehaviour
{
    private float timeRemaining = 600; //10000; // in seconds
    private float timePassed = 0;
    private bool timerIsRunning = false;
    private TMP_Text timer_text;
    private LevelWrapper levelwrapper;
    private LevelSetup levelsetup;

    private void Start()
    {
        // Starts the timer automatically
        timerIsRunning = true;
        timer_text = GameObject.Find("TimerText").GetComponent<TMP_Text>();
        levelsetup = GetComponent<LevelSetup>();
        levelwrapper = GetComponent<LevelWrapper>();
        if (timePassed > 0)
        {
            timeRemaining -= timePassed;
        }
    }

    void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                timePassed += Time.deltaTime;
                DisplayTime(timeRemaining);
            }
            else
            {
                Debug.Log("Time has run out!");
                timeRemaining = 0;
                timerIsRunning = false;
                levelwrapper.LevelGameOver();
            }

            if (levelsetup.levelCompletion && levelsetup.levelMeaningCompletion)
            {
                timerIsRunning = false;
            }
        }
    }

    void DisplayTime(float timeToDisplay)
    {
    timeToDisplay += 1;

    float minutes = Mathf.FloorToInt(timeToDisplay / 60); 
    float seconds = Mathf.FloorToInt(timeToDisplay % 60);

    timer_text.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public float GetTimePassed()
    {
        return timePassed;
    }

    public void SetTimePassed(float time)
    {
        timePassed = time;
    }

    public bool LevelCompletedOnTime()
    {
        return timeRemaining > 0;
    }
}