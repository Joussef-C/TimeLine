using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonProperties : MonoBehaviour
{
    public Button button;
    public GameObject uiElement;
    private Vector3 initialPosition;

    private void Start()
    {
        initialPosition = uiElement.transform.position;

        button.onClick.AddListener(ToggleUIElement);
    }

    private void ToggleUIElement()
    {
        uiElement.SetActive(!uiElement.activeSelf);

        if (uiElement.activeSelf)
        {
            uiElement.transform.position = initialPosition;
        }
    }
}
