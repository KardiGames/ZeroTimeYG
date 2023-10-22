using System;
using UnityEngine;

public class Timer : MonoBehaviour
{	
    public static Timer Instance { get; private set; }
	private int secondsToMinute=60;
	private float deltaTimeSummator=0f;
	
	private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(this);
    }
		
	public event Action EverySecondAction;
	public event Action EveryMinuteAction;
	
	private void Update () {
		deltaTimeSummator+=Time.deltaTime;
		if (deltaTimeSummator>1.0f) 
		{
			deltaTimeSummator-=1.0f;
			EverySecondAction?.Invoke();
			secondsToMinute-=1;
			if (secondsToMinute<=0) {
				secondsToMinute+=60;
				EveryMinuteAction?.Invoke();
			}
		}	
	}

	
}
