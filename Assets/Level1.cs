using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class Level1 : MonoBehaviour
{
    public LevelSetup levelGrid;
    string fileName = "coffeecup.json";
    private string user;

    void Start()
    {
        user = UsernameManager.Username;
        if (user == null){
            user = "test";
            Debug.Log("TEST user in use!!!");
        }
        Debug.Log("Username: " + user);

        LoadLevel(fileName);

    }

    public void LoadLevel(string fileName)
    {
        // if savedProgressFileName exists, load it
        string savedProgressFileName = user+"_progress_level_" + fileName;
        string progressFilePath = Path.Combine(Application.persistentDataPath, savedProgressFileName);
        if (System.IO.File.Exists(progressFilePath))
        {
            Debug.Log("Loading saved progress from " + progressFilePath + " instead of " + fileName + ".");
            levelGrid.LoadLevelToPlay(savedProgressFileName);
        }
        else
        {
            levelGrid.LoadLevelToPlay(fileName);
        }
    }

    public void SaveProgress()
    {
        string savedProgressFileName = user+"_progress_level_" + fileName;
        levelGrid.SaveProgress(savedProgressFileName);
    }
}
