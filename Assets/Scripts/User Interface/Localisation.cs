using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Localisation : MonoBehaviour
{
    public event Action<string> OnLanguageChangedEvent;

    [SerializeField] private string _currentLanguage;
    [SerializeField] private TranslationData _data;
    private Dictionary<string, string> _russianPhrases = new Dictionary<string, string>();
    
    public void InitLanguage ()
    {
        if (_currentLanguage=="ru_ru" && _russianPhrases.Count == 0)
            foreach (var phrase in _data.Phrases)
                _russianPhrases.Add(phrase.English, phrase.Russian);
    }

    public string Translate (string text)
    {
        if (text.StartsWith("@"))
        {
            var translation = _data.Texts.Find((d) => d.Tag==text);
            if (translation != null)
                if (_currentLanguage == "en_en")
                    text = translation.English;
                else if (_currentLanguage == "ru_ru")
                    text = translation.Russian;
                
        } else if (_currentLanguage=="ru_ru" && _russianPhrases.ContainsKey(text))
        {
            text = _russianPhrases[text];
        }
        return text;
    }

    public string CurrentLanguage
    {
        set
        {
            if (value == _currentLanguage)
                return;
            if (value == "ru_ru" || value == "en_en")
            {
                _currentLanguage = value;
                ChangeLanguage(); 
            }
        }
        get => _currentLanguage;
    }

    private void ChangeLanguage()
    {
        InitLanguage();
        OnLanguageChangedEvent?.Invoke(_currentLanguage);
    }
}
