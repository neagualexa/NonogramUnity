using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;

public class Level1 : MonoBehaviour
{
    public LevelSetup levelGrid;
    public string fileName = "coffeecup.json";
    private string user;
    private TMP_Text hint_text;

    void Start()
    {
        hint_text = GameObject.Find("HintText").GetComponent<TMP_Text>();

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

    [System.Serializable]
    public class Hints
    {
        public List<string> hints;
    }

    public void ShowHint()
    {
        // read the hints from the JSON file: ./LevelsJSON/coffecup_hints.json
        // at random choose one and display it in the hint_text object
        // string filePath = "./LevelsJSON/"+fileName.Split('.')[0]+"_hints.json";
        // string fileContent = File.ReadAllText(filePath);;
        // print("Reading hints from: " + filePath);

        // Hints hints = JsonUtility.FromJson<Hints>(fileContent);

        // int randomIndex = UnityEngine.Random.Range(0, hints.hints.Count);
        // string randomHint = hints.hints[randomIndex];
        // hint_text.text = randomHint;
        print("TODO: test");
    }
}
