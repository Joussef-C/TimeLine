using System;
using TMPro;
using System.Collections;
using UnityEngine;

public class TimelineManager : MonoBehaviour
{
    public TextMeshProUGUI startDateText;
    public TextMeshProUGUI endDateText;

    [SerializeField]
    private int startYear = 2022, startMonth = 1, startDay = 1;

    [SerializeField]
    private int endYear = 2027, endMonth = 12, endDay = 15;

    public DateTime startDate;
    public DateTime endDate;

    void Start()
    {
        startDate = new DateTime(startYear, startMonth, startDay);
        endDate = new DateTime(endYear, endMonth, endDay);
        UpdateTimeline();
    }

    void UpdateTimeline()
    {
        startDateText.text = startDate.ToString("yyyy-MM-dd");
        endDateText.text = endDate.ToString("yyyy-MM-dd");
    }
}