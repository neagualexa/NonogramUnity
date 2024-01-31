using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Level1 : MonoBehaviour
{
    public LevelSetup levelGrid;
    string fileName = "coffeecup.json";
    public string user;

    void Start()
    {
        LoadLevel(fileName);
    }

    public void LoadLevel(string fileName)
    {
        levelGrid.LoadLevelToPlay(fileName);
    }

    public void SaveProgress()
    {
        string savedProgressFileName = user+"_progress_level_" + fileName;
        levelGrid.SaveProgress(savedProgressFileName);
    }
}
