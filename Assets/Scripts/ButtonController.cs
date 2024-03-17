using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class ButtonController : MonoBehaviour
{
    public GameObject targetObject;
    public GameObject targetObject2;
    private Button button;

    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(ToggleObject);
    }

    private void ToggleObject()
    {
        targetObject.SetActive(!targetObject.activeSelf);
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!IsClickedOnUI())
            {
                DisableObject();

            }
        }
    }

    private bool IsClickedOnUI()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    public void DisableObject()
    {
        targetObject.SetActive(false);
        targetObject2.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {

    }
}
