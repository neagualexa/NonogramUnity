using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;

public class HTTPRequests : MonoBehaviour
{
    public bool puzzleMeaningCheck = false;
    private LevelSetup levelSetup;
    private ButtonAnimations buttonAnimations;
    private bool puzzleMeaningError = false;

    private void Start()
    {
        levelSetup = GetComponent<LevelSetup>();
        buttonAnimations = GetComponent<ButtonAnimations>();
    }
    public IEnumerator SendPuzzleMeaningRequest(string userGuess, string solution)
    {
        puzzleMeaningError = false;
        // initial condition
        if (userGuess == solution)
        {
            // Debug.Log("Correct meaning");
            puzzleMeaningCheck = true;
            update_levelMeaningCompletion();
        }
        else
        {
            string apiUrl = "http://localhost:5000/check_puzzle_meaning";
            string jsonData = $"{{ \"user_guess\": \"{userGuess}\", \"solution\": \"{solution}\" }}";
            WWWForm form = new WWWForm();
            form.AddField("puzzleMeaning", jsonData);

            using UnityWebRequest www = UnityWebRequest.Post(apiUrl, form);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success) // error happening from UnityWebRequest or cannot connect to server
            {
                Debug.LogError(www.error);
                puzzleMeaningCheck = false;
                puzzleMeaningError = true;
            }
            else if( www.downloadHandler.text.Contains("error") ){  // error happening from hugging face model
                puzzleMeaningCheck = false;
                puzzleMeaningError = true;
            }
            else
            {
                Debug.Log("Form upload complete!");
                Debug.Log("Request successful!");

                string responseContent = www.downloadHandler.text;
                Debug.Log("Response received from LLM server: " + responseContent);
                responseContent = responseContent.ToLower();
                puzzleMeaningCheck = responseContent.Contains("true");
            }
            update_levelMeaningCompletion();
            Debug.Log("Solved meaning: " + levelSetup.levelMeaningCompletion + ", real meaning: " + solution + ", input meaning: " + userGuess);
        }
    }

    private void update_levelMeaningCompletion()
    {
        if (puzzleMeaningError)
        {
            Debug.Log("Error in response from hugging face model");
            buttonAnimations.OnMeaningServerError();
            return;
        }
        levelSetup.levelMeaningCompletion = puzzleMeaningCheck;
        buttonAnimations.OnMeaningCompletionCheck();
    }

    public IEnumerator SendPuzzleProgressRequest(bool[,] cellStates, bool[,] solutionCellStates, string levelMeaning)
    {
        Debug.Log("SendPuzzleProgressRequest:: Checking solution...");
        levelSetup.CheckSolution();
        string cellStatesString = "";
        string solutionCellStatesString = "";
        convert_boolList_to_string(cellStates, solutionCellStates, ref cellStatesString, ref solutionCellStatesString);
        string apiUrl = "http://localhost:5000/check_puzzle_progress";
        string jsonData = $"{{ \"cellStates\": \"{cellStatesString}\", \"solutionCellStates\": \"{solutionCellStatesString}\", \"levelMeaning\": \"{levelMeaning}\", \"completed\": \"{levelSetup.levelCompletion}\"}}";
        Debug.Log("SendPuzzleProgressRequest:: jsonData: " + jsonData);
        WWWForm form = new WWWForm();
        form.AddField("puzzleProgress", jsonData);

        using UnityWebRequest www = UnityWebRequest.Post(apiUrl, form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(www.error);
        }
        else
        {
            Debug.Log("SendPuzzleProgressRequest:: Form upload complete!");
            Debug.Log("SendPuzzleProgressRequest:: Request successful!");

            string responseContent = www.downloadHandler.text;
            Debug.Log("SendPuzzleProgressRequest:: Response received from LLM server: " + responseContent);
        }        
    }

    private void convert_boolList_to_string(bool[,] cellStates, bool[,] solutionCellStates, ref string cellStatesString, ref string solutionCellStatesString)
    {
        for (int i = 0; i < cellStates.GetLength(0); i++)
        {
            for (int j = 0; j < cellStates.GetLength(1); j++)
            {
                cellStatesString += cellStates[i, j] ? "1" : "0";
                solutionCellStatesString += solutionCellStates[i, j] ? "1" : "0";
            }
            cellStatesString += "|";
            solutionCellStatesString += "|";
        }
    }

        public IEnumerator SendHintToVerbalise(string hint)
    {
        string apiUrl = "http://localhost:5000/verbalise_hint";
        string jsonData = $"{{ \"hint\": \"{hint}\" }}";
        WWWForm form = new WWWForm();
        form.AddField("hint", jsonData);

        using UnityWebRequest www = UnityWebRequest.Post(apiUrl, form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(www.error);
        }
        else
        {
            Debug.Log("SendHintToVerbalise:: Request successful!");
        }
    }

    // public IEnumerator SendAudioClipRequest(string audioFilePath)
    // {
    //     string apiUrl = "http://localhost:5005/verbal";
    //     WWWForm form = new WWWForm();
    //     string fileName = Path.GetFileName(audioFilePath);
    //     form.AddBinaryData("audioFile", File.ReadAllBytes(audioFilePath), fileName, "audio/wav");
    //     form.AddField("fileName", fileName);

    //     using UnityWebRequest www = UnityWebRequest.Post(apiUrl, form);
    //     yield return www.SendWebRequest();

    //     if (www.result != UnityWebRequest.Result.Success)
    //     {
    //         Debug.LogError(www.error);
    //     }
    //     else
    //     {
    //         Debug.Log("SendAudioClipRequest:: Form upload complete!");
    //         Debug.Log("SendAudioClipRequest:: Request successful!");

    //         string responseContent = www.downloadHandler.text;
    //         Debug.Log("SendAudioClipRequest:: Response received from LLM server: " + responseContent);
    //     }
    // }
}