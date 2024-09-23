using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TranslationData", menuName = "Translation Data")]

public class TranslationData : ScriptableObject
{
    [SerializeField] private List<TranslationPhrase> _phrases = new();
    [SerializeField] private List<TranslationText> _texts  = new();
    public List<TranslationPhrase> Phrases => _phrases;
    public List<TranslationText> Texts => _texts;

    [Serializable]
    public class TranslationPhrase
    {
        [SerializeField] private string _english;
        [SerializeField] private string _russian;
        public string English => _english; 
        public string Russian => _russian; 
    }

    [Serializable]
    public class TranslationText
    {
        [SerializeField] private string _tag;
        [SerializeField][TextArea(2,20)] private string _english;
        [SerializeField][TextArea(2, 20)] private string _russian;

        public string Tag => _tag;
        public string English => _english;
        public string Russian => _russian;
    }
}
