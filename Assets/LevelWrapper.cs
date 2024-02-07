using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;

public class LevelWrapper : MonoBehaviour
{
    private LevelManager level_manager;
    private string fileName;

    void Start()
    {
        // Load the saved username from PlayerPrefs
        if (PlayerPrefs.HasKey("LevelFilename"))
        {
            fileName = PlayerPrefs.GetString("LevelFilename") + ".json";
        }
        else
        {
            fileName = "test.json";
        }
        level_manager = GetComponent<LevelManager>();

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
}
