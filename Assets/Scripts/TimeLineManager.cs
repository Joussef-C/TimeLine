using System;
using TMPro;
using System.Collections;
using UnityEngine;

public class TimelineManager : MonoBehaviour
{
    public TextMeshProUGUI startDateText;
    public TextMeshProUGUI endDateText;

    public DateTime startDate = new DateTime(2022, 01, 1);
    public DateTime endDate = new DateTime(2022, 06, 15);

    void Start()
    {
        UpdateTimeline();
    }

    void UpdateTimeline()
    {
        startDateText.text = "Start Date: " + startDate.ToString("yyyy-MM-dd");
        endDateText.text = "End Date: " + endDate.ToString("yyyy-MM-dd");
    }
}
