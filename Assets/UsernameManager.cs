using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UsernameManager : MonoBehaviour
{
    public TMP_InputField usernameInputField;
    public TMP_Text usernameDisplayText;
    public Button saveButton;

    private static string username;

    public static string Username
    {
        get { return username; }
    }

    private static UsernameManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            // DontDestroyOnLoad(gameObject);
            // DontDestroyOnLoad(usernameInputField.gameObject);
            // DontDestroyOnLoad(usernameDisplayText.gameObject);
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
}
