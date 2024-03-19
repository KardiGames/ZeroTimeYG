using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterUI : MonoBehaviour
{
    [SerializeField] private List<SkillUI> _skillUIPrefabPool;
    [SerializeField] private List<string> _skills = new ();
    [SerializeField] private Dictionary<string, SkillUI> _skillsUI = new Dictionary<string, SkillUI>(); //TODO Delete SerField
    private WorldCharacter _worldCharacter;

    public void Init (WorldCharacter worldCharacter) {
        if (worldCharacter == null)
            return;
        _worldCharacter = worldCharacter;



        _worldCharacter.Skills.TaskTimer.CompletePastTasks();

        if (_skillsUI.Count != _skills.Count)
            GetObjectsFromPool();
        
        foreach (string skill in _skills)
        {
            _skillsUI[skill].Init(skill, _worldCharacter.Skills, this);
        }
        
        foreach (TaskByTimer trainingSkill in _worldCharacter.Skills.TaskTimer.GetAllItems())
        {
            _skillsUI[trainingSkill.TaskTag].TrainButton.interactable = false;
            if (trainingSkill.IsStarted())
            {
                _skillsUI[trainingSkill.TaskTag].StartCountdown();
            }
        }
    }

    private void GetObjectsFromPool ()
    {
        foreach (var element in _skillsUI)
            element.Value.gameObject.SetActive(false);
        _skillsUI.Clear();

        int poolElement = 0;
        foreach (string skill in _skills)
        {
            if (poolElement < _skillUIPrefabPool.Count)
            {
                _skillsUI[skill] = _skillUIPrefabPool[poolElement];
                _skillUIPrefabPool[poolElement].gameObject.SetActive(true);
                poolElement++;
            }
            else
                break;
        }
    }

}
