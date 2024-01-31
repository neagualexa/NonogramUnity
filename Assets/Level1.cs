using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Level1 : MonoBehaviour
{
    public LevelSetup levelGrid;
    string fileName = "coffeecup.json";

    void Start()
    {
        LoadLevel(fileName);
    }

    public void LoadLevel(string fileName)
    {
        levelGrid.LoadLevelToPlay(fileName);
    }

    public void SaveProgress(string user="")
    {
        string saveFileName = "level1_"+user+"_progress_" + fileName + ".json"; // Name of the save file
        levelGrid.SaveProgress(saveFileName);
    }
}
