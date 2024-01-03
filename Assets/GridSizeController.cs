using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GridSizeController : MonoBehaviour
{
    public TMP_InputField rowsInputField; // Reference to the TextMeshPro input field for rows
    public TMP_InputField columnsInputField; // Reference to the TextMeshPro input field for columns

    public NonogramGrid nonogramGrid; // Reference to the NonogramGrid script

    // Call this function when a button is pressed to update the grid size
    public void UpdateGridSize()
    {
        int newRows = int.Parse(rowsInputField.text); // Parse the text to get the number of rows
        int newColumns = int.Parse(columnsInputField.text); // Parse the text to get the number of columns

        // if newRows or newColumns is less than 1, set it to 1
        if (newRows < 1)
        {
            newRows = 1;
        }
        if (newColumns < 1)
        {
            newColumns = 1;
        }

        Debug.Log("Updating grid size to " + newRows + " rows and " + newColumns + " columns");
        
        nonogramGrid.ChangeGridSize(newRows, newColumns); // Call the function in NonogramGrid to update the grid size
    }

}
