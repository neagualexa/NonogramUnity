using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;

public class Level2 : MonoBehaviour
{
    private LevelManager level_manager;
    public string fileName = "heart.json";

    void Start()
    {
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
