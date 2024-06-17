using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;
// using UnityEditor;
// using Interactions;

public class LevelGridCellToggle : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler
{
    private Button button;
    public int isPressed = 0;
    private Color originalColor;
    private Color color;

    private LevelSetup gridReference; // Reference to the LevelSetup script
    private int rowIndex; // Index of the row this cell belongs to
    private int columnIndex; // Index of the column this cell belongs to
    private TMP_Text clickedCell;

    private Image buttonImage;
    private TrackInput trackInput;

    private HTTPRequests httpRequests;

    public Sprite sprite_normal;
    public Sprite sprite_blocked;

    public bool crossedInteraction = false;


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

    public void StateMachinePressed()
    {
        /*
         A cell can be 
             1 : filled
             0 : empty
            // -1 : blocked

        Toggle state change: empty -> filled -> blocked -> empty
        */
        crossedInteraction = false;
        if (isPressed == 0)
        {
            isPressed = 1;
        }
        else if (isPressed == -1)
        {
            isPressed = 1;
        }
        else
        {
            isPressed = 0;
        }
    }

    public void CrossStateMachinePressed()
    {
        /*
         A cell can be 
             X: any state
             -1 : blocked

        Toggle state change: crossed -> empty; any -> crossed
        */
        if (isPressed == -1)
        {
            crossedInteraction = false;
            isPressed = 0;
        }
        else {
            crossedInteraction = true;
            isPressed = -1;
        }
    }

    void buttonStateChange(){
        if (isPressed == 1)
        {
            // fill the cell
            setColor("#46A2B4");
            buttonImage.color = color;
            buttonImage.sprite = sprite_normal;
            if (gridReference != null)
            {
                gridReference.SetCellState(rowIndex, columnIndex, true);
            }
        }
        else if (isPressed == 0)
        {
            // empty the cell
            buttonImage.color = originalColor;
            buttonImage.sprite = sprite_normal;
            if (gridReference != null)
            {
                gridReference.SetCellState(rowIndex, columnIndex, false);
            }
        }
        else
        {
            // block the cell
            // setColor("#FF0000");
            buttonImage.color = originalColor;
            buttonImage.sprite = sprite_blocked;
            if (gridReference != null)
            {
                gridReference.SetCellState(rowIndex, columnIndex, false);
            }
        }

        if (!crossedInteraction){
            // record the last pressed cell
            gridReference.gridInteractions.SetLastPressedCell(rowIndex, columnIndex);
            clickedCell.text = "(" + (rowIndex + 1) + ", " + (columnIndex + 1) + ")";
            StartCoroutine(httpRequests.SendGridInteractionRequest(username: PlayerPrefs.GetString("Username"), level: PlayerPrefs.GetString("LevelFilename"), gridReference.gridInteractions, gridReference.GetCellStates(), gridReference.GetSolutionCellStates() ));
        }
    }

    // only used at initialisation of grid when loading a level
    public void UpdateButton(bool active)
    {
        button = GetComponent<Button>();
        buttonImage = button.GetComponent<Image>();
        setOriginalColor();
        if (active)
        {
            isPressed = 1;
        } else {
            isPressed = 0;
        }

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
            // isPressed = !isPressed;
            StateMachinePressed();
            buttonStateChange();
        } else if (trackInput.RightMouseisPressed)
        {
            // Debug.Log("Cursor Entering " + name + " GameObject" + "Right mouse is pressed: " + trackInput.RightMouseisPressed);
            // isPressed = !isPressed;
            CrossStateMachinePressed();
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
            // isPressed = !isPressed;
            StateMachinePressed();
            buttonStateChange();
        } else if (eventData.button == PointerEventData.InputButton.Right)
        {
            // isPressed = !isPressed;
            CrossStateMachinePressed();
            buttonStateChange();
        }

        if (eventData.button == PointerEventData.InputButton.Left) // also allowing to press and hold when on cell vs TrackInput.cs
        {
            // Debug.Log("CHILD:: Left mouse is pressed");
            trackInput.LeftMouseisPressed = true;
        } else if (eventData.button == PointerEventData.InputButton.Right) // also allowing to press and hold when on cell vs TrackInput.cs
        {
            // Debug.Log("CHILD:: Right mouse is pressed");
            trackInput.RightMouseisPressed = true;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left) // also allowing to press and hold when on cell vs TrackInput.cs
        {
            trackInput.LeftMouseisPressed = false;
        } else if (eventData.button == PointerEventData.InputButton.Right) // also allowing to press and hold when on cell vs TrackInput.cs
        {
            trackInput.RightMouseisPressed = false;
        }
    }
}
