using System;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;


public class ScrollbarController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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
    public Sprite yourStartDateSprite;
    public Sprite yourEndDateSprite;


    public Button playButton;
    public Button ForwardButton;
    public Button BackwardButton;
    private bool isPlaying = false;

    public Sprite Playimage;
    public Sprite Stopimage;
    private RectTransform currentTimelineContainer = null;
    private RectTransform currentMonthContainer = null;
    float yearWidth = 500f;
    private float HanddleSpeed = 0.015f;
    private bool isMouseOver = false;
    float minimumWidth = 1860f; // Replace with your desired minimum width




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
        ForwardButton.onClick.AddListener(OnForwardButtonClicked);
        BackwardButton.onClick.AddListener(OnBackwardButtonClicked);
        scrollbarRect = scrollbar.GetComponent<RectTransform>();
    }




    public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseOver = false;
    }
    void MoveHandleForward()
    {
        MoveHandleEveryMonth(true);
    }

    void MoveHandleBackward()
    {
        MoveHandleEveryMonth(false);
    }
    void OnPlayButtonClicked()
    {
        if (!isPlaying)
        {
            InvokeRepeating("MoveHandleForward", 0f, HanddleSpeed);
            playButton.image.sprite = Stopimage;

        }
        else
        {
            CancelInvoke("MoveHandleForward");
            playButton.image.sprite = Playimage;


        }

        isPlaying = !isPlaying;
    }



    void OnForwardButtonClicked()
    {
        if ((HanddleSpeed > 0.0001F) || HanddleSpeed >= 0.015f)
        {
            //0.003125
            HanddleSpeed *= 0.25f;
            if (isPlaying)
            {
                CancelInvoke("MoveHandleForward");
                InvokeRepeating("MoveHandleForward", 0f, HanddleSpeed);
            }
        }

    }
    void OnBackwardButtonClicked()
    {
        if (HanddleSpeed < 0.015f)
        {

            if (isPlaying)
            {
                HanddleSpeed += HanddleSpeed * 2.5f;
                CancelInvoke("MoveHandleForward");
                InvokeRepeating("MoveHandleForward", 0f, HanddleSpeed);

            }
        }
        else if (HanddleSpeed >= 0.015f)
        {
            return;
        }

    }



    void MoveHandleEveryMonth(bool isForward)
    {
        float currentValue = scrollbar.value;
        float step = 0.5f / (float)(endDate - startDate).TotalDays;
        float newValue;

        if (isForward)
        {
            newValue = Mathf.Clamp(currentValue + step, 0f, 1f);
        }
        else
        {
            newValue = Mathf.Clamp(currentValue - step, 0f, 1f);
        }

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




        /// update 22/03
        float totalYearWidth = Mathf.Max(((endYear - startYear + 1) * yearWidth) - yearWidth, minimumWidth);
        totalYearWidth = ((endYear - startYear + 1) * yearWidth) - yearWidth;
        ///

        for (int year = startYear; year <= endYear; year++)
        {

            TextMeshProUGUI yearText = Instantiate(dateTextPrefab, timelineContainer);
            yearText.text = year.ToString();
            yearText.rectTransform.anchoredPosition = new Vector2((year - startYear) * yearWidth, 0f);


        }



        double totalDays = (endDate - startDate).TotalDays;
        float endDatePosition = (float)(totalDays / totalDays) * totalYearWidth;




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
                monthText.rectTransform.anchoredPosition = new Vector2(monthPosition, -5f);
            }
        }

        currentTimelineContainer = timelineContainer;
        currentMonthContainer = MonthContainer;
    }





    void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        int totalMonths = ((endDate.Year - startDate.Year) * 12) + endDate.Month - startDate.Month;
        float monthWidth = currentTimelineContainer.sizeDelta.x / totalMonths;
        float threshold = 50.0f; // threshold 
        if (isMouseOver)
        {


            if (scroll > 0f)
            {
                if (monthWidth < threshold)
                {
                    yearWidth += scroll * 350;
                }
                scrollbarRect.anchoredPosition = new Vector2(1f, scrollbarRect.anchoredPosition.y);

                InitializeTimelineUI();
            }



            else if (scroll < 0f)
            {
                if (scrollbarRect.sizeDelta.x > minimumWidth)
                {
                    yearWidth += scroll * 350;
                }
                scrollbarRect.anchoredPosition = new Vector2(1f, scrollbarRect.anchoredPosition.y);

                InitializeTimelineUI();
            }

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

        selectedDateText.text = selectedDate.ToString("MMM yyyy");
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

