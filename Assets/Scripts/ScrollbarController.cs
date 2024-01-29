using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ScrollbarController : MonoBehaviour
{
    public TimelineManager timelineManager;
    public Scrollbar scrollbar;
    public TextMeshProUGUI selectedDateText;
    DateTime startDate;
    DateTime endDate;



    enum TimeInterval
    {
        Daily,
        Weekly,
        Monthly
    }

    TimeInterval selectedInterval = TimeInterval.Daily;

    void Start()
    {
        
        startDate = timelineManager.startDate;
        endDate = timelineManager.endDate;

        scrollbar.onValueChanged.AddListener(OnScrollbarValueChanged);


        Button dailyButton = GameObject.Find("day").GetComponent<Button>();
        dailyButton.onClick.AddListener(() => SetTimeInterval((int)TimeInterval.Daily));

        Button weeklyButton = GameObject.Find("week").GetComponent<Button>();
        weeklyButton.onClick.AddListener(() => SetTimeInterval((int)TimeInterval.Weekly));

        Button monthlyButton = GameObject.Find("month").GetComponent<Button>();
        monthlyButton.onClick.AddListener(() => SetTimeInterval((int)TimeInterval.Monthly));
    
        
    }


     void OnScrollbarValueChanged(float value)
    {
        DateTime selectedDate;

        switch (selectedInterval)
        {
            case TimeInterval.Daily:
                selectedDate = startDate.AddDays((endDate - startDate).TotalDays * value);
                break;

            case TimeInterval.Weekly:
                selectedDate = AddWeekly(startDate, value);
                break;

            case TimeInterval.Monthly:
                selectedDate = startDate.AddMonths((int)((endDate - startDate).TotalDays * value / 30));
                break;

            default:
                selectedDate = startDate;
                break;
        }

        selectedDateText.text = selectedDate.ToString("yyyy-MM-dd");

    }

    DateTime AddWeekly(DateTime startDate, float value)
    {
        int weeks = (int)((endDate - startDate).TotalDays * value / 7);
        return startDate.AddDays(weeks * 7);
    }

    public void SetTimeInterval(int interval)
    {
        selectedInterval = (TimeInterval)interval;
    }

    public DateTime GetSelectedDate()
    {
        DateTime selectedDate;

        switch (selectedInterval)
        {
            case TimeInterval.Daily:
                selectedDate = startDate.AddDays((endDate - startDate).TotalDays * scrollbar.value);
                break;

            case TimeInterval.Weekly:
                selectedDate = AddWeekly(startDate, scrollbar.value);
                break;

            case TimeInterval.Monthly:
                selectedDate = startDate.AddMonths((int)((endDate - startDate).TotalDays * scrollbar.value / 30));
                break;

            default:
                selectedDate = startDate;
                break;
        }

        return selectedDate;
    }

}
