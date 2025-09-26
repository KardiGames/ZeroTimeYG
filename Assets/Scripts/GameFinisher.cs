using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

public class GameFinisher : MonoBehaviour
{
	private const string WIN_ITEM_NAME = "Game Ending City Kit";

	//public event Action OnGameFinished;
	[SerializeField] private WorldCharacter _player;
	[SerializeField] private GlobalUserInterface _globalUI;
	[SerializeField] private WorldUserInterface _worldUI;
	
	private int _finalScore = 0;
	
	public bool IsFinished => _finalScore > 0;
	public int Score => _finalScore;

    [DllImport("__Internal")]
    private static extern void UnityReady();

    public bool TryFinish () {
		if (IsFinished || _player==null)
			return false;
		
		if (_player.X==0 && _player.Y==0 && _player.Inventory.Contains(WIN_ITEM_NAME)) {
			_globalUI.ShowBlackMessage("@WinGameMessage");
			_worldUI.ShowBigMessage("You win the game!!!");
			
			_finalScore=Rating.GetRating(_player, true);
			_player.Inventory.Remove(this, WIN_ITEM_NAME);
			return true;
		}
		return false;
	}
	
	public string ToJson()
    {
        return _finalScore.ToString();
    }
	
	public void FromJson(string jsonString)
    {
		int.TryParse(jsonString, out _finalScore);
    }
}
