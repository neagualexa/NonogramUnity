using UnityEngine;
using UnityEngine.EventSystems;

public class TrackInput : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool LeftMouseisPressed = false;
    void Update()
    {
        if (LeftMouseisPressed)
        {
            Debug.Log("Left mouse is pressed");
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            LeftMouseisPressed = true;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            LeftMouseisPressed = false;
        }
    }
}
