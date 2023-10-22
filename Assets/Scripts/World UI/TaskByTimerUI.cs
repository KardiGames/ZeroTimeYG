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
    [SerializeField] private Button pauseUnpauseButton;
    [SerializeField] private TextMeshProUGUI pauseUnpauseText;
    [SerializeField] private Button startTaskButton;

    private TimeSpan countdown;

    private TaskByTimer task;
    //private InventoryUIContentFiller inventoryUI;

    private void Start()
    {

    }

    private void Texting() => print("Texting is started");
    public void Setup(TaskByTimer task)
    {
        if (this.task == null)
            this.task = task;
        //this.inventoryUI = inventoryUI;

        taskName.text = task.TaskName;

        if (task.IsStarted())
        {
            UpdateTimer();
            Timer.Instance.EverySecondAction += UpdateSec;
            Timer.Instance.EveryMinuteAction += UpdateTimer;
        }
        else
        {
            countdown = new TimeSpan(0, 0, (int)task.secondsToFinish);
            FormTimerText();
        }


        if (task.IsStarted())
            pauseUnpauseButton.gameObject.SetActive(true);
        else
            startTaskButton.gameObject.SetActive(true);

        if (task.OnPause)
        {
            pauseUnpauseButton.gameObject.SetActive(true);
            pauseUnpauseText.text = "Unpause";
        }

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
            task.Source.TaskTimer.CheckForCompletion();
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

    public void StartTask()
    {
        if (task == null)
            return;
        task.StartTask();
        if (task.IsStarted())
        {
            Timer.Instance.EverySecondAction += UpdateSec;
            Timer.Instance.EveryMinuteAction += UpdateTimer;
            pauseUnpauseButton.gameObject.SetActive(true);
        }
    }

    public void PauseUnpauseTask()
    {
        if (task == null)
            return;
        if (task.OnPause)
        {
            StartTask();
            pauseUnpauseText.text = "Pause";
        }
        else
        {
            task.SetPause();
            if (!task.OnPause)
                return;
            Timer.Instance.EverySecondAction -= UpdateSec;
            Timer.Instance.EveryMinuteAction -= UpdateTimer;
            pauseUnpauseText.text = "Unpause";
        }


    }



}
