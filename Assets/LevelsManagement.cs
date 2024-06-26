using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
// using UnityEditor;
using TMPro;
using GridData;

public class LevelsManagement : MonoBehaviour {

    // private MainMenu mainMenu;
    // public TMP_FontAsset fontAsset; 
    private GameObject[] levelButtons;
    private int currentLevelIndex = -1; // 1 indexed

    void Awake() {
        // mainMenu = GetComponent<MainMenu>();
        // GetAllLevelButtons();

        currentLevelIndex = PlayerPrefs.GetInt("CurrentLevelIndex");
        Debug.Log("Current level index: " + currentLevelIndex);

        GetAllLevelButtonObjects();
        RanOutOfTimeWarning(); 
        EndGameWarning();
        // SetInnactiveLevelButtons();
    }

    private void GetAllLevelButtonObjects() {
        /*
        This functions gets all children objects from a given GameObject.
        */
        levelButtons = new GameObject[GameObject.Find("GroupContainer").transform.childCount];
        for (int i = 0; i < GameObject.Find("GroupContainer").transform.childCount; i++) {
            levelButtons[i] = GameObject.Find("GroupContainer").transform.GetChild(i).gameObject;

            // if the level is the current level, set the button to be active, else all innactive // 0 indexed
            if ( i != currentLevelIndex - 1) {
                levelButtons[i].GetComponent<Button>().interactable = false;
            } else {
                levelButtons[i].GetComponent<Button>().interactable = true;
            }
        }
    }

    public void RanOutOfTimeWarning() {
        /*
        This function is called when the player runs out of time on the previous level.
        */
        if (currentLevelIndex > 1) {
            int previousLevelIndex = currentLevelIndex - 1 - 1; // convert to 0 indexed and get the previous level
            // read the level file for the previous level and check the onTime variable
            string username = PlayerPrefs.GetString("Username");
            string previousLevelName = levelButtons[previousLevelIndex].name.Split(' ')[3];
            string levelFile = "./Assets/LevelsJSON/user_progress/" + username + "_progress_level_" + previousLevelName + ".json";

            Debug.Log("Checking level file: " + levelFile);

            string fileContent = System.IO.File.ReadAllText(levelFile);
            GridStateData gridLoadedData = JsonUtility.FromJson<GridStateData>(fileContent);
            
            if (!gridLoadedData.onTime) {
                Debug.Log("Player ran out of time on level: " + previousLevelName);
                // set the text to the Warning message
                GameObject.Find("WarningText").GetComponent<TMP_Text>().text = "You ran out of time on the previous level! Don't worry, you can continue to the next level: " + currentLevelIndex;
            }
            
        }

    }

    public void EndGameWarning() {
        /*
        This function is called when the player completes the last level.
        */
        if (currentLevelIndex == levelButtons.Length) {
            // set the text to the Warning message
            GameObject.Find("WarningText").GetComponent<TMP_Text>().text = "Congratulations! You have completed all levels! Please press the <End Game> button and ask for the supervisor to return in the room.";
        }
    }

    // private void SetInnactiveLevelButtons() {
    //     /*
    //     This function sets level buttons to be innactive if the variable onTime is false.
    //     Get the level file under the current PlayerPrefs username.
    //     For all levels under LevelsJSON/user_progress.
    //     */
    //     string username = PlayerPrefs.GetString("Username");
    //     string[] levelFiles = System.IO.Directory.GetFiles("./Assets/LevelsJSON/user_progress", "*.json");
    //     foreach (string levelFile in levelFiles) {
    //         if (levelFile.Contains(username)) {
    //             string fileContent = System.IO.File.ReadAllText(levelFile);
    //             GridStateData gridLoadedData = JsonUtility.FromJson<GridStateData>(fileContent);
    //             string levelName = gridLoadedData.level;
    //             // if the player ran out of time, set the button to be innactive
    //             Debug.Log("Checking level: " + levelName + " for onTime: " + gridLoadedData.onTime + ","+ gridLoadedData.levelCompletion + ","+ gridLoadedData.levelMeaningCompletion);
    //             if (!gridLoadedData.onTime || (gridLoadedData.levelCompletion && gridLoadedData.levelMeaningCompletion)) {
    //                 foreach (GameObject levelButton in levelButtons) {
    //                     // Debug.Log("Checking button: " + levelButton.name + " for level: " + levelName);
    //                     if (levelButton.name.Contains(levelName)) {
    //                         Debug.Log("Setting button to be not interactable: " + levelButton.name);
    //                         levelButton.GetComponent<Button>().interactable = false;
    //                     }
    //                 }
    //             }
    //         }
    //     }
    // }

    // private void GetAllLevelButtons() {
    //     /*
    //     This function reads the level files in the LevelsJSON folder and creates a button for each level.
    //     Make buttons to be children of the GroupContainer game object.
    //     */
    //     string[] levelFiles = System.IO.Directory.GetFiles("./Assets/LevelsJSON", "*.json");
    //     foreach (string levelFile in levelFiles) {
    //         if (levelFile.Contains("_hints")) {
    //             continue;
    //         }
    //         string levelName = System.IO.Path.GetFileNameWithoutExtension(levelFile);
    //         GameObject levelButton = new GameObject("LevelButton (" + levelName + ")");
    //         levelButton.transform.SetParent(GameObject.Find("GroupContainer").transform, false);
    //         // add button component to game object
    //         levelButton.AddComponent<Button>();
    //         levelButton.GetComponent<Button>().onClick.AddListener(() => mainMenu.GoToScene(levelName));

    //         // add image component to game object
    //         levelButton.AddComponent<Image>();
    //         // set source image to the level button sprite
    //         LoadSpriteByGUID(levelButton.GetComponent<Image>());
    //         Color buttonColor = new Color();
	//         ColorUtility.TryParseHtmlString("#47A2B4", out buttonColor);
    //         levelButton.GetComponent<Image>().color = buttonColor;
    //         levelButton.GetComponent<Image>().type = Image.Type.Sliced;

    //         // Set the button's text to the level name
    //         GameObject buttonText = new GameObject("Text (TMP)");
    //         buttonText.transform.SetParent(levelButton.transform, false);
    //         TMP_Text text = buttonText.AddComponent<TMP_Text>();
    //         text.text = levelName;
    //         text.font = fontAsset;
    //     }

    // }

    // private void LoadSpriteByPath(Image targetImage, string assetPath){
    //     // asset must be in Assets/Resources folder for this to work
    //     if (string.IsNullOrEmpty(assetPath) || targetImage == null)
    //     {
    //         Debug.LogError("Asset path or Image component is not set.");
    //         return;
    //     }

    //     // Load the sprite from the specified path
    //     Sprite loadedSprite = Resources.Load<Sprite>(assetPath);

    //     if (loadedSprite == null)
    //     {
    //         Debug.LogError($"Sprite at '{assetPath}' could not be loaded.");
    //         return;
    //     }

    //     // Assign the sprite to the Image component
    //     targetImage.sprite = loadedSprite;
    //     buttonImage.type = Image.Type.Sliced;
    // }
    
    
}
