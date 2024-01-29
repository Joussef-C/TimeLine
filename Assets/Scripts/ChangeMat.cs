using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class SerializableDateTime
{
    public int year;
    public int month;
    public int day;

    public SerializableDateTime(int year, int month, int day)
    {
        this.year = year;
        this.month = month;
        this.day = day;
    }

    public DateTime ToDateTime()
    {
        return new DateTime(year, month, day);
    }
}
[System.Serializable]
public class DateMaterialPair
{
    public SerializableDateTime specificDate;
    public Material material;
}

public class ChangeMat : MonoBehaviour
{
    public DateMaterialPair[] dateMaterialPairs;

    void Update()
    {
        ScrollbarController scrollbarController = FindObjectOfType<ScrollbarController>();

        if (scrollbarController != null)
        {
            DateTime selectedDate = scrollbarController.GetSelectedDate();

            foreach (DateMaterialPair pair in dateMaterialPairs)
            {
                DateTime specifiedDateTime = pair.specificDate.ToDateTime();

                if (selectedDate >= specifiedDateTime)
                {
                    ChangeMaterial(pair.material);
                }
            }
            
        }
    }

    void ChangeMaterial(Material material)
    {
        Renderer renderer = GetComponent<Renderer>();

        if (renderer != null && material != null)
        {
            renderer.material = material;
        }
    }
}
