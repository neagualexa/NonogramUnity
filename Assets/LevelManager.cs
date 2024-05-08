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
    private ButtonAnimations buttonAnimations;
    private string user;
    private string level;
    public TMP_Text hint_text;

    // private int hint_index = 0;
    // private int general_hint_index = 0;
    private float user_progress = 0.0f;
    private float prev_user_progress = 0.0f;
    // private Hints hints;
    // private GeneralHints general_hints;

    void Awake()
    {
        hint_text = GameObject.Find("HintText").GetComponent<TMP_Text>();
        levelGrid = GetComponent<LevelSetup>();
        mainMenu = GetComponent<MainMenu>();
        httpRequests = GetComponent<HTTPRequests>();
        buttonAnimations = GetComponent<ButtonAnimations>();

        level = PlayerPrefs.GetString("LevelFilename");
        user = PlayerPrefs.GetString("Username");
        if (user == null){
            user = "test";
            Debug.Log("TEST user in use!!!");
        }
        Debug.Log("Username: " + user);

        // // prepare UNTAILORED hints for the level
        // // read the hints from the JSON file: ./LevelsJSON/levelname_hints.json
        // string filePath = "./Assets/LevelsJSON/"+level+"_hints.json";
        // string fileContent = File.ReadAllText(filePath);;
        // print("Reading UNTAILORED Descriptive hints from: " + filePath);
        // hints = JsonUtility.FromJson<Hints>(fileContent);
        // // shuffle the hints to randomize the order
        // Shuffle(hints.descriptive_hints);
        // Shuffle(hints.meaning_hints);

        // filePath = "./Assets/LevelsJSON/general_hints.json";
        // fileContent = File.ReadAllText(filePath);;
        // print("Reading UNTAILORED General hints from: " + filePath);
        // general_hints = JsonUtility.FromJson<GeneralHints>(fileContent);
        // // shuffle the hints to randomize the order
        // Shuffle(general_hints.general_hints);
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
        public List<string> descriptive_hints;
        public List<string> meaning_hints;
    }

    [System.Serializable]
    public class GeneralHints
    {
        public List<string> general_hints;
    }

    private float CalculateUserProgress(bool[,] cellStates, bool[,] solutionCellStates)
    {
        int totalCells = cellStates.Length;
        int correctCells = 0;
        for (int i = 0; i < cellStates.GetLength(0); i++)
        {
            for (int j = 0; j < cellStates.GetLength(1); j++)
            {
                if (cellStates[i, j] == solutionCellStates[i, j])
                {
                    correctCells += 1;
                }
            }
        }
        return (float)correctCells / totalCells;
    }

    public void ShowHint(string fileName)
    {
        int hintStyle = PlayerPrefs.GetInt("HintChat");
        levelGrid.hint_count += 1;
        // using the auto size feature in Unity with a max fontSize of 30 to fit the hint_text into the object

        if (hintStyle == 0)
        {
            prev_user_progress = user_progress;
            user_progress = CalculateUserProgress(levelGrid.GetCellStates(), levelGrid.GetSolutionCellStates());
            Debug.Log("User Progress: " + user_progress +" prev: "+prev_user_progress);
            if (user_progress == 1.0f)
            {
                // if user completed the puzzle, then return a meaning hint
                UntailoredDescriptiveHints(true);
                return;
            }
            if(prev_user_progress >= user_progress)
            {
                // if the user has not made progress, then provide a hint tailored to the level
                UntailoredDescriptiveHints();
                return;
            }
            else
            {
                // if the user has made progress, then provide a general hint
                UntailoredGeneralHints();
                return;
            }
        } else {
            hint_text.text = "Asking Nono_AI for hint...";
            // add 1.2seconds delay
            StartCoroutine(AskAIAssistant());
            StartCoroutine(ShowHintAfterDelay("Waiting for Nono_AI...", 2.5f)); //3.5f for old directional hint pipeline
            return;
        }
    }

    private void UntailoredDescriptiveHints(bool meaning=false)
    {
        // string randomHint;
        if (meaning)
        {   // return a meaning hint
            // randomHint = hints.meaning_hints[hint_index];
            // StartCoroutine(httpRequests.SendHintToVerbalise(randomHint, 7, level)); // send hint to verbalise server
            StartCoroutine(AskAIAssistant(false, 7));                                   // ask for untailored hint
            StartCoroutine(ShowHintAfterDelay("Waiting for Nono_AI...", 2.5f)); //3.5f for old directional hint pipeline

            // add a delay to receive the verbal response from the server
            // hint_text.text = "Requesting a hint...";
            // StartCoroutine(ShowHintAfterDelay(randomHint, 2.5f));
            // hint_index = (hint_index + 1) % hints.meaning_hints.Count;
            return;
        }

        // return a descriptive hint
        // randomHint = hints.descriptive_hints[hint_index];
        // StartCoroutine(httpRequests.SendHintToVerbalise(randomHint, 1, level)); // send hint to verbalise server
        StartCoroutine(AskAIAssistant(false, 1));                                   // ask for untailored hint
        StartCoroutine(ShowHintAfterDelay("Waiting for Nono_AI...", 2.5f)); //3.5f for old directional hint pipeline

        // add a delay to receive the verbal response from the server
        // hint_text.text = "Requesting a hint...";
        // StartCoroutine(ShowHintAfterDelay(randomHint, 2.5f));
        // hint_index = (hint_index + 1) % hints.descriptive_hints.Count;
    }

    private void UntailoredGeneralHints()
    {
        // string randomHint = general_hints.general_hints[general_hint_index];
        // StartCoroutine(httpRequests.SendHintToVerbalise(randomHint, 0, level)); // send hint to verbalise server
        StartCoroutine(AskAIAssistant(false, 0));                                   // ask for untailored hint
        StartCoroutine(ShowHintAfterDelay("Waiting for Nono_AI...", 2.5f)); //3.5f for old directional hint pipeline

        // add a delay to receive the verbal response from the server
        // hint_text.text = "Requesting a hint...";
        // StartCoroutine(ShowHintAfterDelay(randomHint, 2.5f));
        // general_hint_index = (general_hint_index + 1) % general_hints.general_hints.Count;
    }

    IEnumerator ShowHintAfterDelay(string hint, float delay)
    {
        yield return new WaitForSeconds(delay);
        hint_text.text = hint;
    }

    IEnumerator AskAIAssistant(bool tailoredHint = true, int level_hint = 0)
    {
        /// Method to request a hint from the Nono_AI for either Tailored or Untailored hints
        
        yield return new WaitForSeconds(1.5f);
        hint_text.text = "Progress sent to Nono_AI...";
        // Application.OpenURL("http://localhost:5000/");
        bool[,] cellStates = levelGrid.GetCellStates();
        bool[,] solutionCellStates = levelGrid.GetSolutionCellStates();
        string levelMeaning = levelGrid.GetSolutionMeaning();
        Debug.Log("Sending puzzle progress to server...");
        StartCoroutine(httpRequests.SendPuzzleProgressRequest(cellStates, solutionCellStates, levelMeaning, user, level, levelGrid.hint_count, tailoredHint, level_hint));
    }

    public void HintReminderLoop()
    {
        // Every 3 mins, send a request to the server to get a sendPuzzleProgressRequest
        Debug.Log("Starting Hint Reminder Loop...");
        StartCoroutine(HintReminder());
    }

    IEnumerator HintReminder()
    {
        // Every 2 mins, send a request to the server to get a sendPuzzleProgressRequest
        while (true)
        {
            Debug.Log("Starting HintReminder every 120s ...");
            yield return new WaitForSeconds(120);
            // display the Hint popup panel
            buttonAnimations.SetHintSectionVisible();
            ShowHint(PlayerPrefs.GetString("LevelFilename") + ".json");
        }
    }

    
    public void LevelGameOver(string fileName)
    {
        Debug.Log("Game Over!");
        // Move player to next level
        PlayerPrefs.SetInt("CurrentLevelIndex", PlayerPrefs.GetInt("CurrentLevelIndex") + 1);
        // Save the progress
        SaveProgress(fileName);
        mainMenu.GoToScene("Levels");
        // block user from playing the same level again
        // string availableLevels = PlayerPrefs.GetString("AvailableLevels");
        // PlayerPrefs.SetString("AvailableLevels", availableLevels.Replace(fileName, ""));
        // Debug.Log("Updated Available levels: " + PlayerPrefs.GetString("AvailableLevels") + " where removed: " + fileName);
    }


    private void Shuffle<T>(List<T> ts)
    {
        var count = ts.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i) {
            var r = UnityEngine.Random.Range(i, count);
            var tmp = ts[i];
            ts[i] = ts[r];
            ts[r] = tmp;
        }
    }
}
