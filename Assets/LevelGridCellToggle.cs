using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class LevelGridCellToggle : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    private Button button;
    public bool isPressed = false;
    private Color originalColor;
    private Color color;

    private LevelSetup gridReference; // Reference to the LevelSetup script
    private int rowIndex; // Index of the row this cell belongs to
    private int columnIndex; // Index of the column this cell belongs to

    private Image buttonImage;
    private TrackInput trackInput;
    private bool enteringCell = false;
    private bool exitedCell = false;
    private bool pressSwipe = false;


    void Start()
    {
        button = GetComponent<Button>();
        buttonImage = button.GetComponent<Image>();
        gridReference = GetComponentInParent<LevelSetup>();
        trackInput = gridReference.GetComponent<TrackInput>();
        // rectTransform = GetComponent<RectTransform>();

        // originalColor = buttonImage.color;
        setOriginalColor();
        // button.onClick.AddListener(OnButtonClick);
    }

    void setOriginalColor()
    {
        string hexoriginalColor = "#BFECF5"; // Replace this with your hex color string
        ColorUtility.TryParseHtmlString(hexoriginalColor, out originalColor);
    }

    void setColor(string hexColor)
    {
        ColorUtility.TryParseHtmlString(hexColor, out color);
    }

    // Method to set references to the grid and cell indices
    public void SetGridStateReference(LevelSetup grid, int row, int column)
    {
        gridReference = grid;
        rowIndex = row;
        columnIndex = column;
    }

    // void OnButtonClick()
    // {
    //     isPressed = !isPressed;
    //     buttonStateChange();
    // }

    void buttonStateChange(){
        if (isPressed)
        {
            setColor("#46A2B4");
            buttonImage.color = color;
            if (gridReference != null)
            {
                gridReference.SetCellState(rowIndex, columnIndex, true);
            }
        }
        else
        {
            buttonImage.color = originalColor;
            if (gridReference != null)
            {
                gridReference.SetCellState(rowIndex, columnIndex, false);
            }
        }
    }

    public void UpdateButton(bool active)
    {
        button = GetComponent<Button>();
        buttonImage = button.GetComponent<Image>();
        setOriginalColor();
        isPressed = active; // TODO: stil iffy indexing detencing if cell is on or off, also still have to press twice to start updating the grid (as if isPressed is False)

        if (active)
        {
            setColor("#46A2B4");
            buttonImage.color = color;
            // Debug.Log("Cell is active");
            if (gridReference != null)
            {
                // Debug.Log("Cell is active");
                gridReference.SetCellState(rowIndex, columnIndex, true);
            }
        }
        else
        {
            buttonImage.color = originalColor;
            // Debug.Log("Cell is inactive");
            if (gridReference != null)
            {
                gridReference.SetCellState(rowIndex, columnIndex, false);
            }
        }
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        //Output to console the GameObject's name and the following message
        if (trackInput.LeftMouseisPressed)
        {
            Debug.Log("Cursor Entering " + name + " GameObject" + "Left mouse is pressed: " + trackInput.LeftMouseisPressed);
            enteringCell = true;
            exitedCell = false;
            isPressed = !isPressed;
            buttonStateChange();
        }
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        //Output to console the GameObject's name and the following message
        if (trackInput.LeftMouseisPressed)
        {
            Debug.Log("Cursor Exiting " + name + " GameObject" + "Left mouse is pressed: " + trackInput.LeftMouseisPressed);
            enteringCell = false;
            exitedCell = true;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            isPressed = !isPressed;
            buttonStateChange();
        }

        if (eventData.pointerCurrentRaycast.gameObject.transform.IsChildOf(GameObject.Find("Panel").transform) && eventData.button == PointerEventData.InputButton.Left)
        {
            Debug.Log("CHILD:: Left mouse is pressed");
            pressSwipe = true;
        }
    }
}
