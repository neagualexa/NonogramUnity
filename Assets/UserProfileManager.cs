using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using TMPro;
using System.Collections.Generic;

public class UserProfileManager : MonoBehaviour {

    private TMP_Text usernameDisplayText;
    private Toggle toggle;

    private void Awake() {
        usernameDisplayText = GameObject.Find("UsernameDisplay").GetComponent<TMP_Text>();
        toggle = GameObject.Find("HintStyleToggle").GetComponent<Toggle>();
        LoadUsername(); // Load the saved username when the game starts
        toggle.isOn = PlayerPrefs.GetInt("HintChat", 1) == 1;
        // GetAvailableLevels();
    }


    private void LoadUsername()
    {
        // Load the saved username from PlayerPrefs
        if (PlayerPrefs.HasKey("Username"))
        {
            string username = PlayerPrefs.GetString("Username");

            // Update the displayed username text
            if (usernameDisplayText != null)
            {
                usernameDisplayText.text = "Username: " + username;
            }
            else
            {
                usernameDisplayText.text = "TESTING user";
            }
        }
    }

    public void OnHintToggle()
    {
        // save the toggle state in the PlayerPrefs
        PlayerPrefs.SetInt("HintChat", toggle.isOn ? 1 : 0);
    }

    // public void GetAvailableLevels()
    // {
    //     // read all files under the path
    //     string path = "./Assets/LevelsJSON/";
    //     DirectoryInfo dir = new DirectoryInfo(path);
    //     // get all files with .json extension expect for *_hints.json
    //     FileInfo[] info = dir.GetFiles("*.json");
    //     List<string> availableLevels = new List<string>();
    //     foreach (FileInfo file in info)
    //     {
    //         if (!file.Name.Contains("_hint"))
    //         {
    //             availableLevels.Add(file.Name);
    //         }
    //     }
    //     string availableLevels_str = string.Join( ",", availableLevels);
    //     Debug.Log("Available levels: " + availableLevels_str);
    //     PlayerPrefs.SetString("AvailableLevels", availableLevels_str );
    // }
}
