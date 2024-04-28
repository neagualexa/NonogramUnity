using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;

using Interactions;

public class LevelGridCellToggle : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler
{
    private Button button;
    public bool isPressed = false;
    private Color originalColor;
    private Color color;

    private LevelSetup gridReference; // Reference to the LevelSetup script
    private int rowIndex; // Index of the row this cell belongs to
    private int columnIndex; // Index of the column this cell belongs to
    private TMP_Text clickedCell;

    private Image buttonImage;
    private TrackInput trackInput;

    private HTTPRequests httpRequests;


    void Start()
    {
        button = GetComponent<Button>();
        buttonImage = button.GetComponent<Image>();
        gridReference = GetComponentInParent<LevelSetup>();
        httpRequests = GetComponentInParent<HTTPRequests>();
        trackInput = gridReference.GetComponent<TrackInput>();
        clickedCell = GameObject.Find("ClickedCell").GetComponent<TMP_Text>();

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

        // record the last pressed cell
        gridReference.gridInteractions.SetLastPressedCell(rowIndex, columnIndex);
        clickedCell.text = "(" + (rowIndex + 1) + ", " + (columnIndex + 1) + ")";
        StartCoroutine(httpRequests.SendGridInteractionRequest(username: PlayerPrefs.GetString("Username"), level: PlayerPrefs.GetString("LevelFilename"), gridReference.gridInteractions, gridReference.GetCellStates(), gridReference.GetSolutionCellStates() ));
    }

    // only used at initialisation of grid when loading a level
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
            // Debug.Log("Cursor Entering " + name + " GameObject" + "Left mouse is pressed: " + trackInput.LeftMouseisPressed);
            isPressed = !isPressed;
            buttonStateChange();
        }
    }

    // public void OnPointerExit(PointerEventData pointerEventData)
    // {
    //     //Output to console the GameObject's name and the following message
    //     if (trackInput.LeftMouseisPressed)
    //     {
    //         Debug.Log("Cursor Exiting " + name + " GameObject" + "Left mouse is pressed: " + trackInput.LeftMouseisPressed);
    //     }
    // }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            isPressed = !isPressed;
            buttonStateChange();
        }

        if (eventData.button == PointerEventData.InputButton.Left) // also allowing to press and hold when on cell vs TrackInput.cs
        {
            // Debug.Log("CHILD:: Left mouse is pressed");
            trackInput.LeftMouseisPressed = true;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left) // also allowing to press and hold when on cell vs TrackInput.cs
        {
            trackInput.LeftMouseisPressed = false;
        }
    }
}
