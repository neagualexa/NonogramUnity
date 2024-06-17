using UnityEngine;
using UnityEngine.EventSystems;

public class TrackInput : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool LeftMouseisPressed = false;
    public bool RightMouseisPressed = false;
    // void Update()
    // {
    //     if (LeftMouseisPressed)
    //     {
    //         Debug.Log("Left mouse is pressed");
    //     }
    // }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left) // allowing to press and hold when outside of the cell
        {
            LeftMouseisPressed = true;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left) // allowing to press and hold when outside of the cell
        {
            LeftMouseisPressed = false;
        }
    }

    public void OnPointerRightDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right) // allowing to press and hold when outside of the cell
        {
            RightMouseisPressed = true;
        }
    }

    public void OnPointerRightUp(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right) // allowing to press and hold when outside of the cell
        {
            RightMouseisPressed = false;
        }
    }
}
