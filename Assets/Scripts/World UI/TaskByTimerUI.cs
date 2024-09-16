using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TaskByTimerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI taskName;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI startStopText;
    [SerializeField] private Button startPauseButton;

    private TimeSpan countdown;

    private TaskByTimer task;

    public void Init(TaskByTimer task)
    {
        if (this.task == null)
            this.task = task;
        else
            throw new Exception("TaskByTimerUI element is not empty and can't be Initialized.");
        taskName.text = task.TaskName;

        if (task.IsStarted())
        {
            UpdateTimer();
            Timer.Instance.EverySecondAction += UpdateSec;
            Timer.Instance.EveryMinuteAction += UpdateTimer;
        }
        else
        {
            countdown = new TimeSpan(0, 0, (int)task.SecondsToFinish);
            FormTimerText();
        }

        SetStartPauseButtonText();
    }

    private void OnDestroy()
    {
        if (task.IsStarted())
        {
            Timer.Instance.EverySecondAction -= UpdateSec;
            Timer.Instance.EveryMinuteAction -= UpdateTimer;
        }
    }

    private void UpdateSec()
    {
        countdown = countdown.Add(new TimeSpan(0, 0, -1));
        if (countdown.TotalSeconds <= 0)
        {
            task.Source.TaskTimer.CompletePastTasks();
            countdown = TimeSpan.Zero;
        }
        FormTimerText();
    }

    private void UpdateTimer()
    {
        countdown = task.FinishTime - DateTime.Now;
        FormTimerText();
    }

    private void FormTimerText()
    {
        timerText.text = "";
        if (countdown.TotalDays >= 1)
            timerText.text += (int)countdown.TotalDays + "d ";
        timerText.text += $"{countdown.Hours}:{countdown.Minutes}:{countdown.Seconds}";
    }

    public void StartPauseTask()
    {
        if (task == null)
            return;
        if (task.IsStarted())
        {            
            task.Pause();
            if (!task.IsStarted())
            {
                Timer.Instance.EverySecondAction -= UpdateSec;
                Timer.Instance.EveryMinuteAction -= UpdateTimer;
            }
        }
        else if (task.TryToStart())
        {
            Timer.Instance.EverySecondAction += UpdateSec;
            Timer.Instance.EveryMinuteAction += UpdateTimer;
        }

        SetStartPauseButtonText();
    }

    private void SetStartPauseButtonText()
    {
        if (task.IsStarted())
            startStopText.text = "Pause";
        else
            startStopText.text = "Start";
    }

    public void PauseUnpauseTask()
    {

	}
}
