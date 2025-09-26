using System.Runtime.InteropServices;
using UnityEngine;

public class Rating : MonoBehaviour
{
    private const int LEVEL_MULTIPLER = 100;
    private const int WIN_GAME_MULTIPLER = 2;
    private const int EXPECTED_MAX_LEVEL = 30;

    [SerializeField] private WorldCharacter _player;
    [SerializeField] private GameFinisher _gameFinisher;
    [SerializeField] private Yandex _yandexSDKconnector;
    private bool _isFinishedRatingSet = false;

    [DllImport("__Internal")]
    private static extern void SetScore(int score);
    public static int GetRating(WorldCharacter player, bool isGameFinished = false)
    {
        int rating = 0;
        rating += player.Level * LEVEL_MULTIPLER;

        foreach (string skill in Skills.InmplementedSkills)
        {
            rating += player.Skills.GetSkillValue(skill);
        }

        if (isGameFinished)
        {
            int maxRating = EXPECTED_MAX_LEVEL * LEVEL_MULTIPLER + Skills.InmplementedSkills.Count * Skills.MAXIMUM_TOTAL_SKILL;
            rating = maxRating * WIN_GAME_MULTIPLER - rating;
        }

        return rating;
    }

    private void OnEnable()
    {
        _player.Skills.OnSkillTrained += UpdateRating;
        _player.OnLeveUp += UpdateRating;
    }

    private void OnDisable()
    {
        _player.Skills.OnSkillTrained -= UpdateRating;
        _player.OnLeveUp -= UpdateRating;
    }

    private void UpdateRating ()
    {
        if (_player==null || _yandexSDKconnector==null || _gameFinisher==null) 
            return;

        if (_yandexSDKconnector.Offline)
            return;

        if (_gameFinisher.IsFinished)
        {
            if (_isFinishedRatingSet==false)
                SetScore(_gameFinisher.Score);
            _isFinishedRatingSet = true;
        } else
        {
            SetScore(GetRating(_player));
        }
    }

}
