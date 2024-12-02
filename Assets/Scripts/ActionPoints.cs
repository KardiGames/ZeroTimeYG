using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionPoints : MonoBehaviour
{
	public const int ADDITIONAL_DEATH_AP_COST = 4;
	private const int SECONDS_TO_ADD_AP = 900;
	private const int MAX_AP_BASE = 24;
	private const int MAX_AP_LEVEL_INCREASE = 6;
	private const int MAX_AP_VIP_MULTIPLER = 3;
	private const int VIP_SECONDS=86400;
	public event Action OnAPValueChanged;

	[SerializeField] private WorldCharacter _playerCharacter;

    private int _ap;
	private DateTime _timeToAddAP=new();
	private DateTime _vipFinishTime=new();
	
	public int Value {get {
		AddPointsByTimer();
		return _ap;
	}}

	public int MaxValue => PastMaxValue(DateTime.Now);

	private int PastMaxValue (DateTime pastTime)
    {
		if (pastTime == null || pastTime > DateTime.Now)
			pastTime = DateTime.Now;
		int maxAp = MAX_AP_BASE + _playerCharacter.Level * MAX_AP_LEVEL_INCREASE;
		if (pastTime < _vipFinishTime)
			maxAp *= MAX_AP_VIP_MULTIPLER;
		return maxAp;
	}
	
	private void AddPointsByTimer() {

		bool apChanged = false;
		while (_timeToAddAP<DateTime.Now) {
			if (_ap< PastMaxValue(_timeToAddAP)) {
				_ap++;
				apChanged = true;
			}
			_timeToAddAP=_timeToAddAP.AddSeconds(SECONDS_TO_ADD_AP);
		}
		if (apChanged)
			OnAPValueChanged?.Invoke();
	}
	
	public bool TrySpendAP (int actionPoints) {
		AddPointsByTimer();
		if (_ap<actionPoints)
			return false;
		_ap-=actionPoints;
		OnAPValueChanged?.Invoke();
		return true;
	}
	
/*	public void GetAP (int actionPoints) {
		_ap+=actionPoints;
		OnAPValueChanged?.Invoke();
	}*/
	
	public void StartVip ()
    {
		if (_vipFinishTime < DateTime.Now)
			_vipFinishTime = DateTime.Now.AddSeconds(VIP_SECONDS);
		else
			_vipFinishTime = _vipFinishTime.AddSeconds(VIP_SECONDS);
    }

	public string ToJson()
	{
		ActionPointsJsonData jsonAP = new() { Points= _ap, FinishTime = _timeToAddAP.ToString("ddMMyyyyHHmmss"), VipFinishTime = _vipFinishTime.ToString("ddMMyyyyHHmmss") };
		return JsonUtility.ToJson(jsonAP);
	}

	public void FromJson(string jsonString)
	{
		ActionPointsJsonData jsonAP = JsonUtility.FromJson<ActionPointsJsonData>(jsonString);
		if (jsonAP == null)
			return;
		
		_ap=jsonAP.Points;
		_timeToAddAP=DateTime.ParseExact(jsonAP.FinishTime, "ddMMyyyyHHmmss", null);
		if (jsonAP.VipFinishTime=="")
			jsonAP.VipFinishTime="22112024143646";
		_vipFinishTime=DateTime.ParseExact(jsonAP.VipFinishTime, "ddMMyyyyHHmmss", null);
		print("TTA "+ _timeToAddAP+" VIP "+ _vipFinishTime);


		OnAPValueChanged?.Invoke();
	}

	[Serializable]
	private class ActionPointsJsonData
    {
		public int Points;
		public string FinishTime;
		public string VipFinishTime;
	}
}
