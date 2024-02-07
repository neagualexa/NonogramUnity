using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class LevelTEST : MonoBehaviour
{
    public LevelSetup levelGrid;
    string fileName = "test.json";
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
        // string progressFilePath = Path.Combine(Application.persistentDataPath, savedProgressFileName);
        string progressFilePath =  "./Assets/LevelsJSON/user_progress/"+savedProgressFileName;
        if (System.IO.File.Exists(progressFilePath))
        {
            Debug.Log("Loading saved progress from " + progressFilePath + " instead of " + fileName + ".");
            levelGrid.LoadLevelToPlay(progressFilePath);
        }
        else
        {
            levelGrid.LoadLevelToPlay("./Assets/LevelsJSON/"+fileName);
        }
    }

    public void SaveProgress()
    {
        string savedProgressFileName =  "./Assets/LevelsJSON/user_progress/"+user+"_progress_level_" + fileName;
        levelGrid.SaveProgress(savedProgressFileName);
    }
}
