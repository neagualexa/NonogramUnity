using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NonogramStateManager : MonoBehaviour
{
    public NonogramGrid nonogramGrid; // Reference to the NonogramGrid script
    public TMP_InputField nameInputField; // Reference to the TextMeshPro input field for the save file name
    public TMP_InputField meaningInputField; 

    // Function to save the grid state
    public void SaveGridState()
    {
        string saveFileName = "./Assets/LevelsJSON/"+ nameInputField.text + ".json"; // Name of the save file
        string meaning = meaningInputField.text;
        nonogramGrid.SaveGridState(saveFileName, meaning);
    }

    // Function to load the grid state
    public void LoadGridState()
    {
        string saveFileName = "./Assets/LevelsJSON/" + nameInputField.text + ".json"; // Name of the save file
        nonogramGrid.LoadGridState(saveFileName);
    }
}
