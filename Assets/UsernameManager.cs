using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UsernameManager : MonoBehaviour
{
    private TMP_InputField usernameInputField;
    private TMP_Text usernameDisplayText;
    private Button saveButton;
    private Toggle toggle;

    private static string username;

    public static string Username
    {
        get { return username; }
    }
    private static bool hintChat;
    public static int HintChat
    {
        get { return hintChat ? 1 : 0; }
    }

    private static UsernameManager instance;

    private void Awake()
    {
        usernameInputField = GameObject.Find("InputField - Username").GetComponent<TMP_InputField>();
        usernameDisplayText = GameObject.Find("UsernameDisplay").GetComponent<TMP_Text>();
        saveButton = GameObject.Find("SaveUsername").GetComponent<Button>();
        toggle = GameObject.Find("HintStyleToggle").GetComponent<Toggle>();

        if (instance == null)
        {
            instance = this;
            LoadUsername(); // Load the saved username when the game starts
        }
        else
        {
            Destroy(gameObject);
        }

        // Add a listener to the input field's OnEndEdit event to check for Enter key
        usernameInputField.onEndEdit.AddListener(delegate { OnEndEdit(); });
    }

    private void Start()
    {   
        // Set the username input field text to the saved username
        if (usernameInputField != null)
        {
            usernameInputField.text = username;
        }

        toggle.isOn = hintChat;
    }

    public void SetUsername()
    {
        username = usernameInputField.text;

        // Save the username in PlayerPrefs
        PlayerPrefs.SetString("Username", username);

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

    private void LoadUsername()
    {
        // Load the saved username from PlayerPrefs
        if (PlayerPrefs.HasKey("Username"))
        {
            username = PlayerPrefs.GetString("Username");

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

    private void OnEndEdit()
    {
        // This method will be called when the user presses Enter in the input field
        saveButton.onClick.Invoke(); // Invoke the save button click event
    }

    public void OnHintToggle()
    {
        hintChat = toggle.isOn;
        // save the toggle state in the PlayerPrefs
        PlayerPrefs.SetInt("HintChat", hintChat ? 1 : 0);
        // print("HintChat: " + hintChat);
    }
}
