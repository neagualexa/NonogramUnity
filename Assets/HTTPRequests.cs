using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class HTTPRequests : MonoBehaviour
{
    public bool puzzleMeaningCheck = false;
    private LevelSetup levelSetup;
    private ButtonAnimations buttonAnimations;

    private void Start()
    {
        levelSetup = GetComponent<LevelSetup>();
        buttonAnimations = GetComponent<ButtonAnimations>();
    }
    public IEnumerator SendPuzzleMeaningRequest(string userGuess, string solution)
    {
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

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
                puzzleMeaningCheck = false;
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
        levelSetup.levelMeaningCompletion = puzzleMeaningCheck;
        buttonAnimations.OnMeaningCompletionCheck();
    }

     public IEnumerator SendPuzzleProgressRequest(bool[,] cellStates, bool[,] solutionCellStates, string levelMeaning)
    {
        string apiUrl = "http://localhost:5000/check_puzzle_progress";
        string jsonData = $"{{ \"cellStates\": \"{cellStates}\", \"solutionCellStates\": \"{solutionCellStates}\", \"levelMeaning\": \"{levelMeaning}\" }}";
        WWWForm form = new WWWForm();
        form.AddField("puzzleProgress", jsonData);

        using UnityWebRequest www = UnityWebRequest.Post(apiUrl, form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(www.error);
            puzzleMeaningCheck = false;
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
        Debug.Log("Current progress: " + cellStates + ", solution progress: " + solutionCellStates + ", level meaning: " + levelMeaning);
        
    }
}