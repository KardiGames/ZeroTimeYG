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

    private TimeSpan _countdown;
    private TaskByTimer task;

    public static string FormTimerText(int secondsToFinish)
    {
        return FormTimerText(new TimeSpan(0, 0, secondsToFinish));
    }
    public static string FormTimerText(TimeSpan countdown)
    {
        string timerText = "";
        if (countdown.TotalDays >= 1)
            timerText += (int)countdown.TotalDays + GlobalUserInterface.Instance.Localisation.Translate("d ");
        timerText += $"{countdown.Hours}:{countdown.Minutes}:{countdown.Seconds}";
        return timerText;
    }

    public void Init(TaskByTimer task)
    {
        if (this.task == null)
            this.task = task;
        else
            throw new Exception("TaskByTimerUI element is not empty and can't be Initialized.");

        taskName.text = GlobalUserInterface.Instance.Localisation.Translate (task.TaskName);

        if (task.IsStarted())
        {
            UpdateTimer();
            Timer.Instance.EverySecondAction += UpdateSec;
            Timer.Instance.EveryMinuteAction += UpdateTimer;
        }
        else
        {
            _countdown = new TimeSpan(0, 0, (int)task.SecondsToFinish);
            SetTimerText();
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
        _countdown = _countdown.Add(new TimeSpan(0, 0, -1));
        if (_countdown.TotalSeconds <= 0)
        {
            task.Source.TaskTimer.CompletePastTasks();
            _countdown = TimeSpan.Zero;
        }
        SetTimerText();
    }

    private void UpdateTimer()
    {
        _countdown = task.FinishTime - DateTime.Now;
        SetTimerText();
    }

    private void SetTimerText()
    {
        timerText.text = FormTimerText(_countdown);
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
            startStopText.text = GlobalUserInterface.Instance.Localisation.Translate("Pause");
        else
            startStopText.text = GlobalUserInterface.Instance.Localisation.Translate("Start");
    }

}
