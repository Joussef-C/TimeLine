using UnityEngine;
using UnityEngine.UI;

public class AttachToObject : MonoBehaviour
{
    public Scrollbar scrollbar;
    private float DisableTextStart = 0.18f;
    private float DisableTextEnd = 0.83f;
    public RectTransform  StartDateP;
    public RectTransform  EndDateP;
    public RectTransform  currentP;
    private bool hasActionBeenPerformed = false;

    void Start()
    {
        OnScrollbarValueChanged(0f);
        scrollbar.onValueChanged.AddListener(OnScrollbarValueChanged);
    }


void OnScrollbarValueChanged(float value)
{
if (scrollbar.value <= DisableTextStart)
{
    transform.SetParent(StartDateP, false);
    transform.localPosition = new Vector3(transform.localPosition.x, 0f, transform.localPosition.z);
    Debug.Log("Changed parent to StartDateP");
    hasActionBeenPerformed = false;
    

}
else if (scrollbar.value >= DisableTextEnd)
{
    transform.localPosition = new Vector3(transform.localPosition.x, 0f, transform.localPosition.z);
    transform.SetParent(EndDateP, false);
    Debug.Log("Changed parent to EndDateP");
    hasActionBeenPerformed = false;
            

}
else
{
   
if (!hasActionBeenPerformed && currentP != null && currentP.name == "Handle")
    {
        transform.SetParent(currentP, false);
        transform.localPosition += new Vector3(0f, 75f, 0f);
        hasActionBeenPerformed = true;
        Debug.Log("Changed parent to currentP");
    }  
}

}
}

