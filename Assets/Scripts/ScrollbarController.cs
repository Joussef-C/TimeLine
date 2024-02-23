using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ScrollbarController : MonoBehaviour
{
    public TimelineManager timelineManager;
    public Scrollbar scrollbar;
    private RectTransform scrollbarRect;

    public Image fillImage;
    public TextMeshProUGUI selectedDateText;

    public RectTransform timelineContainerPrefab;
    public TextMeshProUGUI dateTextPrefab;
    public RectTransform MonthContainerPrefab;
    public TextMeshProUGUI MonthTextPrefab;
    public RectTransform ScrollbarBG;


    public Button playButton;
    private bool isPlaying = false;

    public Sprite Playimage;
    public Sprite Stopimage;
    private RectTransform currentTimelineContainer = null;
    private RectTransform currentMonthContainer = null;
    float yearWidth = 500f;

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
        InitializeTimelineUI();
        playButton.onClick.AddListener(OnPlayButtonClicked);
        scrollbarRect = scrollbar.GetComponent<RectTransform>();


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

        int startYear = startDate.Year;
        int endYear = endDate.Year;

        float totalYearWidth = ((endYear - startYear + 1) * yearWidth) - yearWidth;

        float minimumWidth = 1763f;
        if (totalYearWidth < minimumWidth)
        {
            Debug.Log("MAXIMUM WIDTH REACHED");

            yearWidth = minimumWidth / (endYear - startYear + 1);
            totalYearWidth = ((endYear - startYear + 1) * yearWidth) - yearWidth;


        }

        for (int year = startYear; year <= endYear; year++)
        {

            TextMeshProUGUI yearText = Instantiate(dateTextPrefab, timelineContainer);
            yearText.text = year.ToString();
            yearText.rectTransform.anchoredPosition = new Vector2((year - startYear) * yearWidth, 0f);
        }

        timelineContainer.sizeDelta = new Vector2(totalYearWidth, timelineContainer.sizeDelta.y);





        RectTransform uiRectTransform = ScrollbarBG.GetComponent<RectTransform>();
        uiRectTransform.sizeDelta = new Vector2(timelineContainer.sizeDelta.x, uiRectTransform.sizeDelta.y);

        RectTransform scrollbarReact = scrollbar.GetComponent<RectTransform>();
        scrollbarReact.sizeDelta = new Vector2(timelineContainer.sizeDelta.x, scrollbarReact.sizeDelta.y);


        RectTransform MonthContainer = Instantiate(MonthContainerPrefab, transform);
        MonthContainer.sizeDelta = new Vector2(timelineContainer.sizeDelta.x, MonthContainer.sizeDelta.y);

        int totalMonths = ((endDate.Year - startDate.Year) * 12) + endDate.Month - startDate.Month;
        float monthWidth = timelineContainer.sizeDelta.x / totalMonths;

        for (int month = 0; month <= totalMonths; month++)
        {
            DateTime currentMonth = startDate.AddMonths(month);
            if (currentMonth.Month == 1)
            {
                continue;
            }
            TextMeshProUGUI monthText = Instantiate(MonthTextPrefab, MonthContainer);
            monthText.text = currentMonth.ToString("MMM");
            float monthPosition = month * monthWidth;
            float nextMonthPosition = (month + 1) * monthWidth;
            float monthTextWidth = LayoutUtility.GetPreferredWidth(monthText.rectTransform);
            float threshold = -50.0f;
            if (nextMonthPosition - monthPosition < monthTextWidth + threshold)
            {
                monthText.gameObject.SetActive(false);
            }
            else
            {
                monthText.rectTransform.anchoredPosition = new Vector2(monthPosition, 0f);
            }
        }

        currentTimelineContainer = timelineContainer;
        currentMonthContainer = MonthContainer;
    }





    void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        Vector3 mousePosition = Input.mousePosition;
        float screenCenterX = Screen.width / 2;
        float distanceFromCenter = Mathf.Abs(mousePosition.x - screenCenterX); // Calculate the distance from the center
        float scrollFactor = distanceFromCenter / screenCenterX; // Normalize the distance to get a factor between 0 and 1

        float containerWidthHalf = currentTimelineContainer.sizeDelta.x / 2;
        float negativeContainerWidthHalf = -containerWidthHalf;
        float minScrollPosition = negativeContainerWidthHalf + 857f; // Set your minimum limit
        float maxScrollPosition = containerWidthHalf - 857f; // Set your maximum limit

        int totalMonths = ((endDate.Year - startDate.Year) * 12) + endDate.Month - startDate.Month;
        float monthWidth = currentTimelineContainer.sizeDelta.x / totalMonths;
        float threshold = 50.0f; // threshold 



        if (scroll > 0f)
        {
            if (monthWidth < threshold)
            {
                yearWidth += scroll * 350;
            }


            Debug.Log("Scrolled up");
            Debug.Log("monthwidth" + monthWidth);

            float newPosition;
            if (mousePosition.x < screenCenterX)
            {
                newPosition = Mathf.Clamp(scrollbarRect.anchoredPosition.x + 300 * scrollFactor, minScrollPosition, maxScrollPosition);
            }
            else
            {
                newPosition = Mathf.Clamp(scrollbarRect.anchoredPosition.x - 300 * scrollFactor, minScrollPosition, maxScrollPosition);
            }
            scrollbarRect.anchoredPosition = new Vector2(newPosition, scrollbarRect.anchoredPosition.y);
            InitializeTimelineUI();
        }
        else if (scroll < 0f)
        {

            yearWidth += scroll * 350;

            Debug.Log("Scrolled Down");
            float newPosition;
            if (mousePosition.x < screenCenterX)
            {
                newPosition = Mathf.Clamp(scrollbarRect.anchoredPosition.x - 300 * scrollFactor, minScrollPosition, maxScrollPosition);
            }
            else
            {
                newPosition = Mathf.Clamp(scrollbarRect.anchoredPosition.x + 300 * scrollFactor, minScrollPosition, maxScrollPosition);
            }
            scrollbarRect.anchoredPosition = new Vector2(newPosition, scrollbarRect.anchoredPosition.y);
            InitializeTimelineUI();
        }
    }



    public void SelectDate(DateTime date)
    {
        if (date < startDate || date > endDate)
        {
            throw new ArgumentOutOfRangeException("date", "The selected date is outside the valid range.");
        }

        double totalDays = (endDate - startDate).TotalDays;
        double selectedDays = (date - startDate).TotalDays;

        float value = (float)(selectedDays / totalDays);

        scrollbar.value = value;
        OnScrollbarValueChanged(value);
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

