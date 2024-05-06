using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    public void GoToScene(string sceneName) {
        /*
        This function loads the scene with the given name.
        It is called when the user clicks on a button in the main menu or when the user selects a level to play or by functions the game manager.
        */
        SceneManager.LoadScene(sceneName);
    }

    public void SelectLevelFile(string levelFilename) {
        /*
        This function is called when the user selects a level to play.
        It saves the selected level file name in PlayerPrefs.
        */
        PlayerPrefs.SetString("LevelFilename", levelFilename);
    }

    public void EndGame() {
        /*
        This function sends an end game request to the server.
        */
        Debug.Log("EndGame:: Sending end game request to server...");
        StartCoroutine(GetComponent<HTTPRequests>().SendEndGameRequest(PlayerPrefs.GetString("Username")));
    }

    public void QuitApp() {
        /*
        This function saves the PlayerPrefs (such as username, level progress state, hint style selected, etc) and quits the application.
        */
        Debug.Log("Quitting the application...");
        PlayerPrefs.Save(); // saving the username, level progress, etc.
        Application.Quit();
    }
}
