using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollbarMove : MonoBehaviour
{
    public Scrollbar scrollbar; // Assign this in the inspector
    public RectTransform uiElement; // Assign the UI element whose X position you want to change

    private float initialX;

    // Start is called before the first frame update
    void Start()
    {
        initialX = uiElement.anchoredPosition.x;
        scrollbar.onValueChanged.AddListener(ChangePosition);
    }

    // This method will be called whenever the scrollbar's value changes
    void ChangePosition(float value)
    {
        float maxX = (uiElement.rect.width / 2) - (uiElement.parent.GetComponent<RectTransform>().rect.width / 2) + 100f;
        uiElement.anchoredPosition = new Vector2((value * maxX * 2) - maxX, uiElement.anchoredPosition.y);

    }

    // Update is called once per frame
    void Update()
    {

    }
}
