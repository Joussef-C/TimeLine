using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollbarMove : MonoBehaviour
{
    public Scrollbar scrollbar;
    public RectTransform uiElement;
    private float initialX;

    void Start()
    {
        initialX = uiElement.anchoredPosition.x;
        scrollbar.onValueChanged.AddListener(ChangePosition);
    }
    void ChangePosition(float value)
    {
        float width = uiElement.GetComponent<RectTransform>().rect.width;
        if (width > 1860f)
        {
            float maxX = (uiElement.rect.width / 2) - (uiElement.parent.GetComponent<RectTransform>().rect.width / 2) + 100f;
            uiElement.anchoredPosition = new Vector2((value * maxX * 2) - maxX, uiElement.anchoredPosition.y);
        }
    }

}
