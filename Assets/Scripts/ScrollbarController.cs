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

    public RectTransform timelineContainerPrefab;
    public TextMeshProUGUI dateTextPrefab; 
    public RectTransform MonthContainerPrefab;
    public TextMeshProUGUI MonthTextPrefab;


    public Button playButton;
    private bool isPlaying = false;

    public Sprite Playimage;
    public Sprite Stopimage;
    public int numberOfYearsToShow = 1;
    private RectTransform currentTimelineContainer = null;
    private RectTransform currentMonthContainer = null;


    

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
        endDate = startDate.AddYears(numberOfYearsToShow);
        scrollbar.onValueChanged.AddListener(OnScrollbarValueChanged);
        InitializeTimelineUI();
        playButton.onClick.AddListener(OnPlayButtonClicked);

    }


    public void UpdateNumberOfYearsToShow(int newNumberOfYears)
{
    numberOfYearsToShow = newNumberOfYears;
    endDate = startDate.AddYears(numberOfYearsToShow);

    if (endDate > timelineManager.endDate)
    {
        endDate = timelineManager.endDate;
    }

    InitializeTimelineUI();
}

    void InitializeDropdowns()
    {
        // if (yearDropdown != null)
        // {
        //     int startYear = startDate.Year;
        //     int endYear = endDate.Year;
        //     for (int year = startYear; year <= endYear; year++)
        //     {
        //         yearDropdown.options.Add(new TMP_Dropdown.OptionData(year.ToString()));
        //     }
        //     yearDropdown.value = 0; 
        // }

        // if (monthDropdown != null)
        // {
        //     for (int month = 1; month <= 12; month++)
        //     {
        //         monthDropdown.options.Add(new TMP_Dropdown.OptionData(new DateTime(1, month, 1).ToString("MMMM")));
        //     }
        //     monthDropdown.value = 0; 
        // }

        // if (dayDropdown != null)
        // {
        //     int daysInMonth = DateTime.DaysInMonth(startDate.Year, startDate.Month);
        //     for (int day = 1; day <= daysInMonth; day++)
        //     {
        //         dayDropdown.options.Add(new TMP_Dropdown.OptionData(day.ToString()));
        //     }
        //     dayDropdown.value = 0;
        // }
    }


     void OnPlayButtonClicked()
    {
        if (!isPlaying)
        {
            InvokeRepeating("MoveHandleEveryMonth", 0f, 0.1f); 
            playButton.image.sprite = Stopimage;

        }
        else
        {
            CancelInvoke("MoveHandleEveryMonth");
                        playButton.image.sprite = Playimage;


        }

        isPlaying = !isPlaying;
    }

    void MoveHandleEveryMonth()
    {
        float currentValue = scrollbar.value;
        float newValue = Mathf.Clamp(currentValue + 1.0f / (float)(endDate - startDate).TotalDays, 0f, 1f);
        scrollbar.value = newValue;

        OnScrollbarValueChanged(newValue);
    }

    void InitializeTimelineUI()
    {

    if (currentTimelineContainer != null)
    {
        Destroy(currentTimelineContainer.gameObject);
    }
    if (currentMonthContainer != null)
    {
        Destroy(currentMonthContainer.gameObject);
    }



        RectTransform timelineContainer = Instantiate(timelineContainerPrefab, transform);

        float containerWidth = timelineContainer.sizeDelta.x;
        int startYear = startDate.Year;
        int endYear = endDate.Year;

        for (int year = startYear; year <= endYear; year++)
        {
            TextMeshProUGUI yearText = Instantiate(dateTextPrefab, timelineContainer);
            yearText.text = year.ToString();
            yearText.rectTransform.anchoredPosition = new Vector2((year - startYear) * (containerWidth / (endYear - startYear)), 0f); 
        }



RectTransform MonthContainer = Instantiate(MonthContainerPrefab, transform);

float MonthcontainerWidth = MonthContainer.sizeDelta.x;


int totalMonths = ((endDate.Year - startDate.Year) * 12) + endDate.Month - startDate.Month;

for (int month = 0; month <= totalMonths; month++)
{
    DateTime currentMonth = startDate.AddMonths(month);
        if (currentMonth.Month == 1)
    {
        continue;
    }
    TextMeshProUGUI monthText = Instantiate(MonthTextPrefab, MonthContainer);
    monthText.text = currentMonth.ToString("MMM");
    monthText.rectTransform.anchoredPosition = new Vector2(month * (MonthcontainerWidth / totalMonths), 0f);
} 

        currentTimelineContainer = timelineContainer;
        currentMonthContainer = MonthContainer;
    }

    void Update()
    {
                    

    float scroll = Input.GetAxis("Mouse ScrollWheel");
    if (scroll != 0f)
    {
        int newNumberOfYears = numberOfYearsToShow + (int)(scroll * 10);
        newNumberOfYears = Mathf.Max(1, newNumberOfYears); 
        UpdateNumberOfYearsToShow(newNumberOfYears);
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

        selectedDateText.text = "Selected Date: " + selectedDate.ToString("yyyy-MM-dd");
        fillImage.fillAmount = value;
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


    void SetTimeInterval(int interval)
    {
        selectedInterval = (TimeInterval)interval;
    }

    DateTime AddWeekly(DateTime startDate, float value)
    {
        int weeks = (int)((endDate - startDate).TotalDays * value / 7);
        return startDate.AddDays(weeks * 7);
    }
}

