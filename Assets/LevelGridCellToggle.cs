using UnityEngine;
using UnityEngine.UI;

public class LevelGridCellToggle : MonoBehaviour
{
    private Button button;
    private bool isPressed = false;
    private Color originalColor;

    private LevelSetup gridReference; // Reference to the LevelSetup script
    private int rowIndex; // Index of the row this cell belongs to
    private int columnIndex; // Index of the column this cell belongs to

    private Image buttonImage;

    void Start()
    {
        button = GetComponent<Button>();
        buttonImage = button.GetComponent<Image>();
        // originalColor = buttonImage.color;
        setOriginalColor();
        button.onClick.AddListener(OnButtonClick);
    }

    void setOriginalColor()
    {
        string hexoriginalColor = "#C7F6FF"; // Replace this with your hex color string
        ColorUtility.TryParseHtmlString(hexoriginalColor, out originalColor);
    }

    // Method to set references to the grid and cell indices
    public void SetGridStateReference(LevelSetup grid, int row, int column)
    {
        gridReference = grid;
        rowIndex = row;
        columnIndex = column;
    }

    void OnButtonClick()
    {
        isPressed = !isPressed;

        if (isPressed)
        {
            buttonImage.color = Color.red;
            // Debug.Log("Cell is pressed");
            // Update the grid state when the cell is pressed
            if (gridReference != null)
            {
                gridReference.SetCellState(rowIndex, columnIndex, true);
            }
        }
        else
        {
            buttonImage.color = originalColor;
            // Debug.Log("Cell is released");
            // Update the grid state when the cell is released
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
        // originalColor = buttonImage.color;
        setOriginalColor();
        isPressed = active; // TODO: stil iffy indexing detencing if cell is on or off, also still have to press twice to start updating the grid (as if isPressed is False)

        if (active)
        {
            buttonImage.color = Color.red;
            // Debug.Log("Cell is active");
            if (gridReference != null)
            {
                Debug.Log("Cell is active");
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
}
