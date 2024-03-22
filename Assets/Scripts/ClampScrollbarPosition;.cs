using UnityEngine;

public class ClampScrollbarPosition : MonoBehaviour
{
    RectTransform scrollbarRectTransform;
    float scrollbarWidth;
    float minX;
    float maxX;

    void Start()
    {
        // Get the RectTransform of the scrollbar
        scrollbarRectTransform = GetComponent<RectTransform>();

        // Calculate the width of the scrollbar
        scrollbarWidth = scrollbarRectTransform.rect.width;

        // Calculate the minimum and maximum X positions
        minX = scrollbarWidth / 2; // Adjust if you want padding
        maxX = Screen.width - scrollbarWidth / 2; // Adjust if you want padding
    }

    void Update()
    {
        // Get the current position of the scrollbar
        Vector3 currentPosition = scrollbarRectTransform.position;

        // Calculate the change in position based on the mouse scroll wheel input
        float scrollDelta = Input.mouseScrollDelta.y;

        // Calculate the new position after scrolling
        float newX = currentPosition.x + scrollDelta;

        // Clamp the new X position within the screen bounds
        float clampedX = Mathf.Clamp(newX, minX, maxX);

        // Update the position of the scrollbar
        currentPosition.x = clampedX;
        scrollbarRectTransform.position = currentPosition;
    }
}