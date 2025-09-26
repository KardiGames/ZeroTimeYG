using System;
using System.Collections.Generic;
using UnityEngine;

public class Localisation : MonoBehaviour
{
    public event Action OnLanguageChangedEvent;

    [SerializeField] private string _currentLanguage;
    [SerializeField] private TranslationData _data;
	[SerializeField] [TextArea(2,20)] private string _json;
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
        OnLanguageChangedEvent?.Invoke();
    }
	
	public void SaveDictionary () {
		_json=JsonUtility.ToJson(_data);
	}
	
	public void ReplaceDictionary () {
		_data = Instantiate (_data);
		JsonUtility.FromJsonOverwrite(_json, _data);
	}
	
	public void AppendDictionary () {
        TranslationData data = Instantiate(_data);
		JsonUtility.FromJsonOverwrite(_json, data);
		
		int exists_phrases = 0;
		int exists_texts = 0;

        foreach (TranslationData.TranslationPhrase phrase in data.Phrases)
        {
            if (!_data.Phrases.Exists(ph => ph.English == phrase.English))
                _data.Phrases.Add(phrase);
            else
                exists_phrases++;
        }

        foreach (TranslationData.TranslationText text in data.Texts)
        {
            if (!_data.Texts.Exists(t => t.Tag == text.Tag))
                _data.Texts.Add(text);
            else
                exists_texts++;
        }	
		print ((data.Phrases.Count-exists_phrases) + " phrases added (" + exists_phrases + " NOT)");
		print ((data.Texts.Count-exists_texts) + " texts added (" + exists_texts + " NOT)");
	}
}
