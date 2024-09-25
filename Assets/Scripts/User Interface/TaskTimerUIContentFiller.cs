using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskTimerUIContentFiller : MonoBehaviour
{
    
    [SerializeField] private TaskTimer taskTimer;
    
    [SerializeField] private GameObject objectToFill;
    private RectTransform contentTransform;

    [SerializeField] private float percentSpaceBetweenObjects = 0.05f;
    private float scrollableObjectHeight;
    private List<GameObject> children = new();

    public TaskTimer TaskTimer 
	{
		get => taskTimer; set
        {
            if (taskTimer!=value || value==null)
            {
                Unsubscribe();
                taskTimer = value;
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
        if (contentTransform != null)
            return;
        contentTransform = gameObject.GetComponent<ScrollRect>().content;
        scrollableObjectHeight = objectToFill.GetComponent<RectTransform>().sizeDelta.y;

        if (contentTransform.childCount != 0)
        {
            for (int i = 0; i < contentTransform.childCount; i++)
                children.Add(contentTransform.GetChild(i).gameObject);
        }

        SubscribeAndRefresh();
    }
	
	    private void OnDisable()
    {
        TaskTimer=null;
    }

    private void Clear()
    {
        foreach (GameObject child in children)
            Destroy(child);
        children.Clear();
    }

    public void Fill() => Fill(taskTimer.GetAllItems());
    
    public void Fill (IEnumerable<TaskByTimer> list)
    {
        if (contentTransform == null)
            Start();
        float nextYPosition = 0;

        foreach (var scrollableItem in list)
        {
            GameObject newScrollableObject = Instantiate(objectToFill,contentTransform);
            if (newScrollableObject != null)
                children.Add(newScrollableObject);
            else
                break;

            newScrollableObject.transform.localPosition = new Vector3(0, -nextYPosition);
            newScrollableObject.GetComponent<TaskByTimerUI>().Init(scrollableItem);

            nextYPosition += scrollableObjectHeight * (1.0f + percentSpaceBetweenObjects);

        }
        contentTransform.sizeDelta = new Vector2(contentTransform.sizeDelta.x, nextYPosition);
    }
	
	private void SubscribeAndRefresh()
    {
        if (taskTimer != null && contentTransform!=null)
        {
            taskTimer.OnTaskOrTimerChanged += Clear;
            taskTimer.OnTaskOrTimerChanged += Fill;
            Clear();
            Fill(taskTimer.GetAllItems());
        }
    }

    private void Unsubscribe()
    {
        if (taskTimer != null)
        {
            taskTimer.OnTaskOrTimerChanged -= Clear;
            taskTimer.OnTaskOrTimerChanged -= Fill;
        }
    }
}
