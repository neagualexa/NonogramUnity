using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class UserProfileManager : MonoBehaviour {

    private TMP_Text usernameDisplayText;
    private Toggle toggle;

    private void Awake() {
        usernameDisplayText = GameObject.Find("UsernameDisplay").GetComponent<TMP_Text>();
        toggle = GameObject.Find("HintStyleToggle").GetComponent<Toggle>();
        LoadUsername(); // Load the saved username when the game starts
        toggle.isOn = PlayerPrefs.GetInt("HintChat", 1) == 1;
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
}
