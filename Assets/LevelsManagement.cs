using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEditor;
using TMPro;
using GridData;

public class LevelsManagement : MonoBehaviour {

    // private MainMenu mainMenu;
    // public TMP_FontAsset fontAsset; 
    private GameObject[] levelButtons;

    void Awake() {
        // mainMenu = GetComponent<MainMenu>();
        // GetAllLevelButtons();
        GetAllLevelButtonObjects();
        SetInnactiveLevelButtons();
    }

    private void GetAllLevelButtonObjects() {
        /*
        This functions gets all children objects from a given GameObject.
        */
        levelButtons = new GameObject[GameObject.Find("GroupContainer").transform.childCount];
        for (int i = 0; i < GameObject.Find("GroupContainer").transform.childCount; i++) {
            levelButtons[i] = GameObject.Find("GroupContainer").transform.GetChild(i).gameObject;
        }
    }

    private void SetInnactiveLevelButtons() {
        /*
        This function sets level buttons to be innactive if the variable onTime is false.
        Get the level file under the current PlayerPrefs username.
        For all levels under LevelsJSON/user_progress.
        */
        string username = PlayerPrefs.GetString("Username");
        string[] levelFiles = System.IO.Directory.GetFiles("./Assets/LevelsJSON/user_progress", "*.json");
        foreach (string levelFile in levelFiles) {
            if (levelFile.Contains(username)) {
                string fileContent = System.IO.File.ReadAllText(levelFile);
                GridStateData gridLoadedData = JsonUtility.FromJson<GridStateData>(fileContent);
                string levelName = gridLoadedData.level;
                // if the player ran out of time, set the button to be innactive
                if (!gridLoadedData.onTime || (gridLoadedData.levelCompletion && gridLoadedData.levelMeaningCompletion)) {
                    foreach (GameObject levelButton in levelButtons) {
                        Debug.Log("Checking button: " + levelButton.name + " for level: " + levelName);
                        if (levelButton.name.Contains(levelName)) {
                            Debug.Log("Setting button to be interactable: " + levelButton.name);
                            levelButton.GetComponent<Button>().interactable = false;
                        }
                    }
                }
            }
        }
    }

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

    // private void LoadSpriteByGUID(Image targetImage){
    //     string assetPath = "Assets/UnityAssets/SimplePixelUI/artwork/colorCustomizible/rounded/buttons/button2_rounded_CC.png";
    //     if (string.IsNullOrEmpty(assetPath) || targetImage == null)
    //     {
    //         Debug.LogError("Asset path or Image component is not set.");
    //         return;
    //     }

    //     // Load the sprite from the specified path
    //     Sprite loadedSprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);

    //     if (loadedSprite == null)
    //     {
    //         Debug.LogError($"Sprite at '{assetPath}' could not be loaded.");
    //         return;
    //     }

    //     // Assign the sprite to the Image component
    //     targetImage.sprite = loadedSprite;
    // }
    
    
}
