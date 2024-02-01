using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ScrollbarController : MonoBehaviour
{
    public TimelineManager timelineManager;
    public Scrollbar scrollbar;
    public Image fillImage;
    public TextMeshProUGUI selectedDateText;


    public TMP_Dropdown yearDropdown;
    public TMP_Dropdown monthDropdown;
    public TMP_Dropdown dayDropdown;

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

         if (yearDropdown != null)
        {
            yearDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
        }

        if (monthDropdown != null)
        {
            monthDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
        }

        if (dayDropdown != null)
        {
            dayDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
        }

        InitializeDropdowns();

    }

    void InitializeDropdowns()
    {
        if (yearDropdown != null)
        {
            int startYear = startDate.Year;
            int endYear = endDate.Year;
            for (int year = startYear; year <= endYear; year++)
            {
                yearDropdown.options.Add(new TMP_Dropdown.OptionData(year.ToString()));
            }
            yearDropdown.value = 0; 
        }

        if (monthDropdown != null)
        {
            for (int month = 1; month <= 12; month++)
            {
                monthDropdown.options.Add(new TMP_Dropdown.OptionData(new DateTime(1, month, 1).ToString("MMMM")));
            }
            monthDropdown.value = 0; 
        }

        if (dayDropdown != null)
        {
            int daysInMonth = DateTime.DaysInMonth(startDate.Year, startDate.Month);
            for (int day = 1; day <= daysInMonth; day++)
            {
                dayDropdown.options.Add(new TMP_Dropdown.OptionData(day.ToString()));
            }
            dayDropdown.value = 0;
        }
    }

    void Update()
    {



        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            int scrollDirection = (int)Mathf.Sign(scroll);
            SetTimeInterval((int)selectedInterval + scrollDirection);

            selectedInterval = (TimeInterval)Mathf.Clamp((int)selectedInterval, (int)TimeInterval.Daily, (int)TimeInterval.Monthly);
        }
        

  
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
                        fillImage.fillAmount = value;
    }



        void OnDropdownValueChanged(int value)
    {
        DateTime selectedDate = GetSelectedDateFromDropdowns();
        float normalizedValue = Mathf.InverseLerp((float)startDate.Ticks, (float)endDate.Ticks, (float)selectedDate.Ticks);
        scrollbar.value = Mathf.Clamp01(normalizedValue);

        selectedDateText.text = selectedDate.ToString("yyyy-MM-dd");
        fillImage.fillAmount = scrollbar.value;
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

        public DateTime GetSelectedDateFromDropdowns()
    {
        int selectedYear = int.Parse(yearDropdown.options[yearDropdown.value].text);
        int selectedMonth = monthDropdown.value + 1;
        int selectedDay = int.Parse(dayDropdown.options[dayDropdown.value].text);

        return new DateTime(selectedYear, selectedMonth, selectedDay);
    }
}
