using UnityEngine;
using UnityEngine.UI;

public class GridCellToggle : MonoBehaviour
{
    private Button button;
    private bool isPressed = false;
    private Color originalColor;

    private NonogramGrid gridReference; // Reference to the NonogramGrid script
    private int rowIndex; // Index of the row this cell belongs to
    private int columnIndex; // Index of the column this cell belongs to

    private Image buttonImage;

    void Start()
    {
        button = GetComponent<Button>();
        buttonImage = button.GetComponent<Image>();
        originalColor = buttonImage.color;

        button.onClick.AddListener(OnButtonClick);
    }

    // Method to set references to the grid and cell indices
    public void SetGridStateReference(NonogramGrid grid, int row, int column)
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
            Debug.Log("Cell is pressed");
            // Update the grid state when the cell is pressed
            if (gridReference != null)
            {
                gridReference.SetCellState(rowIndex, columnIndex, true);
            }
        }
        else
        {
            buttonImage.color = originalColor;
            Debug.Log("Cell is released");
            // Update the grid state when the cell is released
            if (gridReference != null)
            {
                gridReference.SetCellState(rowIndex, columnIndex, false);
            }
        }
    }
}
