using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskTimerUIContentFiller : MonoBehaviour
{
    
    [SerializeField] private TaskTimer _taskTimer;
    
    [SerializeField] private GameObject _objectToFill;
    private RectTransform _contentTransform;

    [SerializeField] private float _percentSpaceBetweenObjects = 0.05f;
    private float _scrollableObjectHeight;
    private List<GameObject> _childrenTimers = new();

    public TaskTimer TaskTimer 
	{
		get => _taskTimer; set
        {
            if (_taskTimer!=value || value==null)
            {
                Unsubscribe();
                _taskTimer = value;
                SubscribeAndRefresh();
            } else
            {
                Clear();
                Fill();
            }
       
        }
	
	}

    private void Start()
    {
        if (_contentTransform != null)
            return;
        _contentTransform = gameObject.GetComponent<ScrollRect>().content;
        _scrollableObjectHeight = _objectToFill.GetComponent<RectTransform>().sizeDelta.y;

        if (_contentTransform.childCount != 0)
        {
            for (int i = 0; i < _contentTransform.childCount; i++)
                _childrenTimers.Add(_contentTransform.GetChild(i).gameObject);
        }

        SubscribeAndRefresh();
    }
	
	    private void OnDisable()
    {
        TaskTimer=null;
    }

    private void Clear()
    {
        foreach (GameObject child in _childrenTimers)
            Destroy(child);
        _childrenTimers.Clear();
    }

    private void ClosePanel() => gameObject.SetActive(false);

    public void Fill() => Fill(_taskTimer.GetAllItems());
    
    public void Fill (IEnumerable<TaskByTimer> list)
    {
        if (_contentTransform == null)
            Start();
        float nextYPosition = 0;

        foreach (var scrollableItem in list)
        {
            GameObject newScrollableObject = Instantiate(_objectToFill,_contentTransform);
            if (newScrollableObject != null)
                _childrenTimers.Add(newScrollableObject);
            else
                break;

            newScrollableObject.transform.localPosition = new Vector3(0, -nextYPosition);
            newScrollableObject.GetComponent<TaskByTimerUI>().Init(scrollableItem);

            nextYPosition += _scrollableObjectHeight * (1.0f + _percentSpaceBetweenObjects);

        }
        _contentTransform.sizeDelta = new Vector2(_contentTransform.sizeDelta.x, nextYPosition);
    }
	
	private void SubscribeAndRefresh()
    {
        if (_taskTimer != null && _contentTransform!=null)
        {
            _taskTimer.OnTaskOrTimerChanged += Clear;
            _taskTimer.OnTaskOrTimerChanged += Fill;
            GlobalUserInterface.Instance.Localisation.OnLanguageChangedEvent += ClosePanel;
            Clear();
            Fill(_taskTimer.GetAllItems());
        }
    }

    private void Unsubscribe()
    {
        if (_taskTimer != null)
        {
            _taskTimer.OnTaskOrTimerChanged -= Clear;
            _taskTimer.OnTaskOrTimerChanged -= Fill;
            GlobalUserInterface.Instance.Localisation.OnLanguageChangedEvent -= ClosePanel;
        }
    }
}
