using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NonogramStateManager : MonoBehaviour
{
    public NonogramGrid nonogramGrid; // Reference to the NonogramGrid script
    public TMP_InputField nameInputField; // Reference to the TextMeshPro input field for the save file name

    // Function to save the grid state
    public void SaveGridState()
    {
        string saveFileName = nameInputField.text + ".json"; // Name of the save file
        nonogramGrid.SaveGridState(saveFileName);
    }

    // Function to load the grid state
    public void LoadGridState()
    {
        string saveFileName = nameInputField.text + ".json"; // Name of the save file
        nonogramGrid.LoadGridState(saveFileName);
    }
}
