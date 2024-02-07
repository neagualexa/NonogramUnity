using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    public void GoToScene(string sceneName) {
        SceneManager.LoadScene(sceneName);
    }

    public void SelectLevelFile(string levelFilename) {
        // Save the username in PlayerPrefs
        // print("Level filename: " + levelFilename);
        PlayerPrefs.SetString("LevelFilename", levelFilename);
    }

    public void QuitApp() {
        Application.Quit();
        Debug.Log("Application has quit.");
    }
}
