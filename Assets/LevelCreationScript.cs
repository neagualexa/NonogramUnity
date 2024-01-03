using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelCreationScript : MonoBehaviour
{
    public InputField rowInputField;
    public InputField columnInputField;

    public void StartGame()
    {
        int rows = int.Parse(rowInputField.text);
        int columns = int.Parse(columnInputField.text);

        // You can perform validations on row and column inputs here
        
        CreateLevel(rows, columns);
    }

    void CreateLevel(int rows, int columns)
    {
        // Logic to start the game or create levels based on row and column inputs
        Debug.Log("Creating level with rows: " + rows + ", columns: " + columns);
        // Add your game initialization or level creation code here
    }
}
