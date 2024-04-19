using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;

public class LevelManager : MonoBehaviour
{
    private LevelSetup levelGrid;
    private MainMenu mainMenu;
    private HTTPRequests httpRequests;
    private string user;
    private string level;
    public TMP_Text hint_text;

    private int hint_index = 0;

    void Awake()
    {
        hint_text = GameObject.Find("HintText").GetComponent<TMP_Text>();
        levelGrid = GetComponent<LevelSetup>();
        mainMenu = GetComponent<MainMenu>();
        httpRequests = GetComponent<HTTPRequests>();

        level = PlayerPrefs.GetString("LevelFilename");
        user = PlayerPrefs.GetString("Username");
        if (user == null){
            user = "test";
            Debug.Log("TEST user in use!!!");
        }
        Debug.Log("Username: " + user);
    }

    void Start()
    {
        // Comment out if testbench is active
        HintReminderLoop();
    }

    public void LoadLevel(string fileName)
    {
        print("Loading level from LevelManager: " + fileName);
        // if savedProgressFileName exists, load it
        string savedProgressFileName = user+"_progress_level_" + fileName;
        // string progressFilePath = Path.Combine(Application.persistentDataPath, savedProgressFileName);
        string progressFilePath =  "./Assets/LevelsJSON/user_progress/"+savedProgressFileName;
        // print("Progress file path: " + progressFilePath);
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

    public void SaveProgress(string fileName)
    {
        string savedProgressFileName =  "./Assets/LevelsJSON/user_progress/"+user+"_progress_level_" + fileName;
        levelGrid.SaveProgress(savedProgressFileName);
    }

    [System.Serializable]
    public class Hints
    {
        public List<string> hints;
    }

    public void ShowHint(string fileName)
    {
        int hintStyle = PlayerPrefs.GetInt("HintChat");
        levelGrid.hint_count += 1;
        // using the auto size feature in Unity with a max fontSize of 30 to fit the hint_text into the object

        if (hintStyle == 0)
        {
            // read the hints from the JSON file: ./LevelsJSON/coffecup_hints.json
            // at random choose one and display it in the hint_text object
            string filePath = "./Assets/LevelsJSON/"+fileName.Split('.')[0]+"_hints.json";
            string fileContent = File.ReadAllText(filePath);;
            print("Reading hints from: " + filePath);

            Hints hints = JsonUtility.FromJson<Hints>(fileContent);

            // int randomIndex = UnityEngine.Random.Range(0, hints.hints.Count);
            string randomHint = hints.hints[hint_index];
            StartCoroutine(httpRequests.SendHintToVerbalise(randomHint)); // send hint to verbalise server

            // add a delay to receive the verbal response from the server
            hint_text.text = "Requesting a hint...";
            StartCoroutine(ShowHintAfterDelay(randomHint, 2.5f));
            hint_index = (hint_index + 1) % hints.hints.Count;
            return;
        } else {
            hint_text.text = "Asking NonoAI for hint...";
            // add 1.2seconds delay
            StartCoroutine(AskAIAssistant());
            StartCoroutine(ShowHintAfterDelay("Waiting for NonoAI...", 3.5f));
            return;
        }
    }

    IEnumerator ShowHintAfterDelay(string hint, float delay)
    {
        yield return new WaitForSeconds(delay);
        hint_text.text = hint;
    }

    IEnumerator AskAIAssistant()
    {
        yield return new WaitForSeconds(1.3f);
        hint_text.text = "Progress sent to NonoAI!";
        // Application.OpenURL("http://localhost:5000/");
        bool[,] cellStates = levelGrid.GetCellStates();
        bool[,] solutionCellStates = levelGrid.GetSolutionCellStates();
        string levelMeaning = levelGrid.GetSolutionMeaning();
        Debug.Log("Sending puzzle progress to server...");
        StartCoroutine(httpRequests.SendPuzzleProgressRequest(cellStates, solutionCellStates, levelMeaning, user, level, levelGrid.hint_count));
    }

    public void HintReminderLoop()
    {
        // Every 3 mins, send a request to the server to get a sendPuzzleProgressRequest
        Debug.Log("Starting Hint Reminder Loop...");
        StartCoroutine(HintReminder());
    }

    IEnumerator HintReminder()
    {
        // Every 3 mins, send a request to the server to get a sendPuzzleProgressRequest
        while (true)
        {
            Debug.Log("Starting HintReminder every 180s ...");
            yield return new WaitForSeconds(180);
            ShowHint(PlayerPrefs.GetString("LevelFilename") + ".json");
        }
    }

    
    public void LevelGameOver(string fileName)
    {
        Debug.Log("Game Over!");
        // Save the progress
        SaveProgress(fileName);
        mainMenu.GoToScene("Levels");
        // block user from playing the same level again
        // string availableLevels = PlayerPrefs.GetString("AvailableLevels");
        // PlayerPrefs.SetString("AvailableLevels", availableLevels.Replace(fileName, ""));
        // Debug.Log("Updated Available levels: " + PlayerPrefs.GetString("AvailableLevels") + " where removed: " + fileName);
    }
}
