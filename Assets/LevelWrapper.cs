using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;
using TMPro;

public class LevelWrapper : MonoBehaviour
{
    private LevelManager level_manager;
    private MainMenu mainMenu;
    private string fileName;

    void Start()
    {
        level_manager = GetComponent<LevelManager>();
        mainMenu = GetComponent<MainMenu>();
        
        // Load the saved username from PlayerPrefs
        if (PlayerPrefs.HasKey("LevelFilename"))
        {
            fileName = PlayerPrefs.GetString("LevelFilename") + ".json";
        }
        else
        {
            fileName = "test.json";
        }

        // check if level is available to the user
        // string[] availableLevels = PlayerPrefs.GetString("AvailableLevels").Split(',');
        // if(!availableLevels.Any(fileName.Contains))
        // {
        //     Debug.Log("Level not available to the user: " + fileName);
        //     mainMenu.GoToScene("Levels");
        // }
        // else
        // {
        //     Debug.Log("Level available to the user: " + fileName);
        //     level_manager.LoadLevel(fileName);
        // }
        level_manager.LoadLevel(fileName);
    }

    public void SaveProgress()
    {
        level_manager.SaveProgress(fileName);
    }

    public void ShowHint()
    {
        level_manager.ShowHint(fileName);
    }

    public void LevelGameOver()
    {
        level_manager.LevelGameOver(fileName);
    }
}
