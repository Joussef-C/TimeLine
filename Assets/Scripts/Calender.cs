using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;


public class Calendar : MonoBehaviour
{
    public int startYear = 2023;
    public int endYear = 2040;
    public TextMeshProUGUI yearText;
    public TextMeshProUGUI monthText;
    public GameObject dayPrefab;
    public Transform daysParent;

    private int currentYear;
    private int currentMonth;
    private int selectedDay;


    private void Start()
    {
        currentYear = startYear;
        currentMonth = 1;
        UpdateCalendar();
    }

    private void UpdateCalendar()
    {
        yearText.text = currentYear.ToString();
        monthText.text = GetMonthName(currentMonth);

        // Clear existing day objects
        foreach (Transform child in daysParent)
        {
            Destroy(child.gameObject);
        }

        // Get the number of days in the current month
        int daysInMonth = DateTime.DaysInMonth(currentYear, currentMonth);

        // Get the first day of the month
        DateTime firstDayOfMonth = new DateTime(currentYear, currentMonth, 1);

        // Get the day of the week for the first day of the month (0 = Sunday, 1 = Monday, etc.)
        int firstDayOfWeek = (int)firstDayOfMonth.DayOfWeek;

        // Calculate the number of empty slots before the first day of the month
        int emptySlots = firstDayOfWeek;

        // Instantiate empty slots before the first day of the month
        for (int i = 0; i < emptySlots; i++)
        {
            GameObject emptySlot = Instantiate(dayPrefab, daysParent);
            emptySlot.SetActive(false);
        }

        // Instantiate day objects
        for (int day = 1; day <= daysInMonth; day++)
        {
            GameObject dayObject = Instantiate(dayPrefab, daysParent);
            TextMeshProUGUI dayText = dayObject.GetComponentInChildren<TextMeshProUGUI>();
            dayText.text = day.ToString();

            // Calculate the row and column for the day
            int row = (day + emptySlots - 1) / 7;
            int col = (day + emptySlots - 1) % 7;

            // Set the position of the day object based on the row and column
            RectTransform rectTransform = dayObject.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(col * rectTransform.sizeDelta.x, -row * rectTransform.sizeDelta.y);


            // Add a click listener to the day object
            Button dayButton = dayObject.GetComponent<Button>();
            dayButton.onClick.AddListener(() => OnDayClicked(dayObject));

        }
    }

    public void OnDayClicked(GameObject dayObject)
    {
        TextMeshProUGUI dayText = dayObject.GetComponentInChildren<TextMeshProUGUI>();
        int day = int.Parse(dayText.text);
        PickDate(day);
    }

    private void PickDate(int day)
    {
        selectedDay = day;
        Debug.Log($"Date picked: {currentYear}-{currentMonth}-{selectedDay}");
    }





    private string GetMonthName(int month)
    {
        string[] monthNames = {
            "January", "February", "March", "April", "May", "June",
            "July", "August", "September", "October", "November", "December"
        };

        return monthNames[month - 1];
    }

    public void PreviousYear()
    {
        if (currentYear > startYear)
        {
            currentYear--;
            UpdateCalendar();
        }
    }

    public void NextYear()
    {
        if (currentYear < endYear)
        {
            currentYear++;
            UpdateCalendar();
        }
    }

    public void PreviousMonth()
    {
        if (currentMonth == 1)
        {
            if (currentYear > startYear)
            {
                currentYear--;
                currentMonth = 12;
            }
        }
        else
        {
            currentMonth--;
        }
        UpdateCalendar();
    }

    public void NextMonth()
    {
        if (currentMonth == 12)
        {
            if (currentYear < endYear)
            {
                currentYear++;
                currentMonth = 1;
            }
        }
        else
        {
            currentMonth++;
        }
        UpdateCalendar();
    }
}