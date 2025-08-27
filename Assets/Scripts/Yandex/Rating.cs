using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rating : MonoBehaviour
{
    private const int LEVEL_MULTIPLER = 100;
    private const int WIN_GAME_MULTIPLER = 2;
    private const int EXPECTED_MAX_LEVEL = 30;

    [SerializeField] private WorldCharacter _player;

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

    }

    private int GetRating(bool isGameFinished = false)
    {

        int rating = 0;
        rating += _player.Level * LEVEL_MULTIPLER;

        foreach (string skill in Skills.InmplementedSkills)
        {
            rating += _player.Skills.GetSkillValue(skill);
        }

        if (isGameFinished)
        {
            int maxRating = EXPECTED_MAX_LEVEL * LEVEL_MULTIPLER + Skills.InmplementedSkills.Count * Skills.MAXIMUM_TOTAL_SKILL;
            rating = maxRating * WIN_GAME_MULTIPLER - rating;
        }

        return rating;
    }
}
