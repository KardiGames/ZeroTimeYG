using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionPoints : MonoBehaviour
{
    private const int SECONDS_TO_ADD_AP = 3600;
	public event Action OnAPValueChanged;

	[SerializeField] private WorldCharacter _playerCharacter;

    private int _ap;
	private DateTime _timeToAddAP=new();
	
	public int Value {get {
		AddPointsByTimer();
		return _ap;
	}}
	
	private void AddPointsByTimer() {
		while (_timeToAddAP<DateTime.Now) {
			_ap++;
			OnAPValueChanged?.Invoke();
			_timeToAddAP=_timeToAddAP.AddSeconds(SECONDS_TO_ADD_AP);
		}
	}
	
	public bool TrySpendAP (int actionPoints) {
		AddPointsByTimer();
		if (_ap<actionPoints)
			return false;
		_ap-=actionPoints;
		OnAPValueChanged?.Invoke();
		return true;
	}
	
	public void GetAP (int actionPoints) {
		_ap+=actionPoints;
		OnAPValueChanged?.Invoke();
	}
	
	public string ToJson()
	{
		ActionPointsJsonData jsonAP = new() { Points= _ap, FinishTime = _timeToAddAP.ToString("ddMMyyyyHHmmss")};
		return JsonUtility.ToJson(jsonAP);
	}

	public void FromJson(string jsonString)
	{
		ActionPointsJsonData jsonAP = JsonUtility.FromJson<ActionPointsJsonData>(jsonString);
		if (jsonAP == null)
			return;
		
		_ap=jsonAP.Points;

		_timeToAddAP=DateTime.ParseExact(jsonAP.FinishTime, "ddMMyyyyHHmmss", null);
		AddPointsByTimer();
	}

	[Serializable]
	private class ActionPointsJsonData
    {
		public int Points;
		public string FinishTime;
	}
}
