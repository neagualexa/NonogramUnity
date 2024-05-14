using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;
using GridData;
using Interactions;

public class LevelSetup : MonoBehaviour
{
    public int rows = 0; // Number of rows in the grid
    public int columns = 0; // Number of columns in the grid
    public float cellSizeX = 0;
    public float cellSizeY = 0;

    private GameObject cellPrefab; // Prefab for the grid cell
    private Transform gridParent; // Parent object for the grid cells
    private GameObject originalCellPrefab;
    private TMP_InputField meaningInputField;
    private Button checkMeaningButton;
    private Button checkPuzzleButton;
    private HTTPRequests httpRequests;
    private LevelTimer levelTimer;
    private MainMenu mainMenu;
    private ButtonAnimations buttonAnimations;
    private TMP_Text level_size_text;


    public TMP_FontAsset fontAsset; // Add a field for the font asset

    public GameObject[,] cells; // 2D array to hold references to the grid cells
    private bool[,] cellStates; // 2D array to store the state of each cell
    private bool[,] solutionCellStates; // 2D array to store the Solution state of each cell
    private CellsGrouping cellsGrouping; // Object to store the size of the group that each cell is part of
    public GridInteractions gridInteractions; // Object to handle past interactions with the grid
    private string solutionMeaning = "";

    public bool levelCompletion = false;
    public bool levelMeaningCompletion = false;
    public int hint_count = 0;

    void Awake()
    {
        cellPrefab = GameObject.Find("cellPrefab");
        gridParent = GameObject.Find("GridHolder").transform;
        originalCellPrefab = GameObject.Find("cellPrefab");
        checkMeaningButton = GameObject.Find("CheckMeaningButton").GetComponent<Button>();
        checkPuzzleButton = GameObject.Find("CheckPuzzleButton").GetComponent<Button>();
        meaningInputField = GameObject.Find("InputField -Puzzle Meaning").GetComponent<TMP_InputField>();
        level_size_text = GameObject.Find("PuzzleSize").GetComponent<TMP_Text>();
        httpRequests = GetComponent<HTTPRequests>();
        levelTimer = GetComponent<LevelTimer>();
        mainMenu = GetComponent<MainMenu>();
        buttonAnimations = GetComponent<ButtonAnimations>();
        // Add a listener to the input field's OnEndEdit event to check for Enter key
        meaningInputField.onEndEdit.AddListener(delegate { OnEndEdit(); });

        // assign functions to the buttons
        checkPuzzleButton.onClick.AddListener(delegate { CheckSolution(cellStates, solutionCellStates); });
        checkMeaningButton.onClick.AddListener(delegate { CheckMeaningSolution(); });
    }

    /// <summary>
    /// Function to create the empty grid based on the number of rows and columns
    /// Initialisation of the grid and its row & column indices
    /// </summary>
    void CreateGrid()
    {
        cells = new GameObject[rows, columns];  // Initialize the 2D array for cells
        // cellStates = new bool[rows, columns];   // Initialize the 2D array for cell states

        // Calculate the size of each cell based on the grid size
        cellSizeX = gridParent.GetComponent<RectTransform>().rect.width / columns;
        cellSizeY = gridParent.GetComponent<RectTransform>().rect.height / rows;

        // Calculate the total width and height of the grid
        float totalWidth = columns * cellSizeX;
        float totalHeight = rows * cellSizeY;

        // Calculate the starting position based on the total width and height
        float startX = -totalWidth / 2f + cellSizeX / 2f;
        float startY = totalHeight / 2f - cellSizeY / 2f;

        TMP_FontAsset font = fontAsset; // Use selected font asset or default font

        // ####################### Creating and positioning row indices
        for (int i = 0; i < rows; i++)
        {
            GameObject rowIndex = new GameObject("RowIndex_" + i);
            rowIndex.transform.SetParent(gridParent);
            // set layer to UI
            rowIndex.layer = 5;

            TextMeshProUGUI indexText = rowIndex.AddComponent<TextMeshProUGUI>();
            indexText.font = font;
            indexText.text = "0 "; //(i + 1).ToString();
            indexText.alignment = TextAlignmentOptions.BottomRight;
            indexText.color = Color.white;

            RectTransform indexRect = indexText.GetComponent<RectTransform>();
            indexRect.sizeDelta = new Vector2(cellSizeX, cellSizeY * 1.2f);
            indexRect.anchoredPosition = new Vector2(startX - (cellSizeX / 2 + indexRect.sizeDelta[0] / 5), startY - i * cellSizeY);
            // indexRect.localScale = new Vector3(1f, 1f, 1f); // Ensure the scale is reset to 1 to match cell size //zooms in
        
            // Adjust font size proportionally to cell size
            int fontSize = Mathf.RoundToInt(Mathf.Min(cellSizeX, cellSizeY) * 1.7f); // Adjust the multiplier as needed
            indexText.fontSize = fontSize;
        }

        // ####################### Creating and positioning column indices
        for (int j = 0; j < columns; j++)
        {
            GameObject colIndex = new GameObject("ColIndex_" + j);
            colIndex.transform.SetParent(gridParent);
            // set layer to UI
            colIndex.layer = 5;

            TextMeshProUGUI indexText = colIndex.AddComponent<TextMeshProUGUI>();
            indexText.font = font;
            indexText.text = "00"; //(j + 1).ToString();
            indexText.alignment = TextAlignmentOptions.Bottom;
            indexText.color = Color.white;

            RectTransform indexRect = indexText.GetComponent<RectTransform>();
            indexRect.sizeDelta = new Vector2(cellSizeX * 2f, cellSizeY);
            indexRect.anchoredPosition = new Vector2(startX + j * cellSizeX, startY + (cellSizeY / 2 + indexRect.sizeDelta[1] / 5));
            // indexRect.localScale = new Vector3(1f, 1f, 1f); // Ensure the scale is reset to 1 to match cell size //zooms in

            // Adjust font size proportionally to cell size
            int fontSize = Mathf.RoundToInt(Mathf.Min(cellSizeX, cellSizeY) * 1.7f);
            indexText.fontSize = fontSize;
        }

        // ####################### Creating and positioning cells
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                GameObject cell = Instantiate(cellPrefab, gridParent);
                RectTransform cellRect = cell.GetComponent<RectTransform>();

                // Calculate position and size for each cell
                float posX = startX + j * cellSizeX;
                float posY = startY - i * cellSizeY;

                cellRect.sizeDelta = new Vector2(cellSizeX, cellSizeY);
                cellRect.anchoredPosition = new Vector2(posX, posY);

                // Attach the LevelGridCellToggle script to handle toggle behavior
                // LevelGridCellToggle cellToggle = cell.AddComponent<LevelGridCellToggle>(); // Add the script to the cell
                LevelGridCellToggle cellToggle = cell.GetComponent<LevelGridCellToggle>(); // Get the script from the cell
                cellToggle.SetGridStateReference(this, i, j); // Pass reference to the grid and cell indices
                cellToggle.UpdateButton(cellStates[i, j]);

                cells[i, j] = cell; // Store reference to the cell in the array
                cell.SetActive(true);
            }
        }

        // Hide the original cellPrefab
        originalCellPrefab.SetActive(false);
    }

    /// <summary>
    /// Function to change grid size dynamically
    /// </summary>
    /// <param name="newRows"></param>
    /// <param name="newColumns"></param>
    public void ChangeGridSize(int newRows, int newColumns)
    {
        ClearRowIndices();
        ClearColumnIndices();

        rows = newRows;
        columns = newColumns;

        DestroyGrid();
        CreateGrid();
    }

    void DestroyGrid()
    {
        if (cells != null)
        {
            foreach (GameObject cell in cells)
            {
                Destroy(cell);
            }
        }
    }

    void ClearRowIndices()
    {
        for (int i = 0; i < rows; i++)
        {
            Transform rowIndex = gridParent.Find("RowIndex_" + i);
            if (rowIndex != null)
            {
                Destroy(rowIndex.gameObject);
            }
        }
    }

    void ClearColumnIndices()
    {
        for (int j = 0; j < columns; j++)
        {
            Transform colIndex = gridParent.Find("ColIndex_" + j);
            if (colIndex != null)
            {
                Destroy(colIndex.gameObject);
            }
        }
    }



    /// <summary>
    /// Cell State Management
    /// </summary>
    /// <returns></returns>
    public bool[,] GetCellStates()
    {
        return cellStates;
    }

    public bool[,] GetSolutionCellStates()
    {
        return solutionCellStates;
    }

    public string GetSolutionMeaning()
    {
        return solutionMeaning;
    }

    public bool GetCellState(int row, int column)
    {
        return cellStates[row, column];
    }

    public bool GetSolutionCellState(int row, int column)
    {
        return solutionCellStates[row, column];
    }
    public delegate void CellStateChangeEvent();
    public event CellStateChangeEvent OnCellStateChanged;

    public void SetCellState(int row, int column, bool state)
    {
        cellStates[row, column] = state;

        if (OnCellStateChanged != null)
        {
            OnCellStateChanged.Invoke();
        }

        // UpdateRowIndexText(row);
        // UpdateColumnIndexText(column);
    }

    /// <summary>
    /// Method to update the row and column indices text based on the grid state (either updated or from file read)
    /// </summary>
    /// <param name="rowIndex"></param>
    /// <param name="solution"></param>

    // Update row indices text with consecutive pressed cell counts
    void UpdateRowIndexText(int rowIndex, bool solution = false)
    {
        Transform rowTextTransform = gridParent.Find("RowIndex_" + rowIndex);
        if (rowTextTransform != null)
        {
            TMP_Text indexText = rowTextTransform.GetComponent<TMP_Text>();
            if (indexText != null)
            {
                RectTransform indexRect = indexText.GetComponent<RectTransform>();

                // get width of a text containing "0" to calculate the difference in size for position adjustment in horizontal direction
                indexText.text = "0 0 0 (00)";
                float text_width_0 = indexRect.rect.width;
                float text_height_0 = indexRect.rect.height;

                string consecutiveCount = CalculateGroupPressedCellsPerRow(rowIndex, solution);
                indexText.text = consecutiveCount + " (" + (rowIndex + 1).ToString() + ")";

                // Calculate preferred size of the text
                Vector2 preferredSize = new Vector2(indexText.preferredWidth, text_height_0);

                indexRect.sizeDelta = preferredSize;

                // // Calculate the difference in size for position adjustment in horizontal direction
                float difference = (indexRect.rect.width - text_width_0) / 6f; // Difference in text width

                // // Shift the text to the left based on the text size difference
                indexRect.anchoredPosition -= new Vector2(difference, 0f);
                indexText.fontSize = 35;
                
            }
        }
    }

    // Update column indices text with consecutive pressed cell counts
    void UpdateColumnIndexText(int colIndex, bool solution = false)
    {
        Transform colTextTransform = gridParent.Find("ColIndex_" + colIndex);
        if (colTextTransform != null)
        {
            TMP_Text indexText = colTextTransform.GetComponent<TMP_Text>();
            if (indexText != null)
            {
                RectTransform indexRect = indexText.GetComponent<RectTransform>();

                string consecutiveCount = CalculateGroupPressedCellsPerColumn(colIndex, solution);
                indexText.text = consecutiveCount + "\n(" + (colIndex + 1).ToString() + ")";

                indexText.fontSize = 35;
                indexRect.position = new Vector3(indexRect.position.x, indexRect.position.y + 1.5f, indexRect.position.z);
                
            }
        }
    }

    /// <summary>
    /// Method to calculate the count of consecutive pressed cells in a row and column
    /// </summary>
    /// <param name="rowIndex or colIndex"></param>
    /// <param name="solution"></param>
    /// <returns></returns>
    string CalculateGroupPressedCellsPerRow(int rowIndex, bool solution = false)
    {
        int currentGroup = 0;
        string rowGroupText = "";
        for (int j = 0; j < columns; j++)
        {
            bool currentCellState = (solution) ? GetSolutionCellState(rowIndex, j) : GetCellState(rowIndex, j);

            if (currentCellState)
            {
                currentGroup++;
            }
            else if (currentGroup > 0)
            {
                rowGroupText += currentGroup + "  ";
                currentGroup = 0;
            }
        }
        if (currentGroup > 0)
        {
            rowGroupText += currentGroup + " ";
        }

        if (rowGroupText.Length == 0)
        {
            rowGroupText = "0 ";
        }

        return rowGroupText;
    }

    string CalculateGroupPressedCellsPerColumn(int colIndex, bool solution = false)
    {
        int currentGroup = 0;
        string colGroupText = "";
        for (int j = 0; j < columns; j++)
        {
            bool currentCellState = (solution) ? GetSolutionCellState(j, colIndex) : GetCellState(j, colIndex);

            if (currentCellState)
            {
                currentGroup++;
            }
            else if (currentGroup > 0)
            {
                colGroupText += currentGroup + "\n";
                currentGroup = 0;
            }
        }
        if (currentGroup > 0)
        {
            colGroupText += currentGroup;
        }

        if (colGroupText.Length == 0)
        {
            colGroupText = "0";
        }
        // if last character is new line, remove it
        else if (colGroupText[colGroupText.Length - 1] == '\n')
        {
            colGroupText = colGroupText.Substring(0, colGroupText.Length - 1);
        }

        return colGroupText;
    }


    /// <summary>
    /// Section for saving and loading grid state from a JSON file
    /// </summary>
    /// <param name="fileName"></param>
    public void SaveProgress(string fileName)
    {
        // check if solved correctly
        CheckSolution(cellStates, solutionCellStates);
        // check if meaning is correct
        CheckMeaningSolution();
        if (levelCompletion && levelMeaningCompletion)
        {
            // Move player to next level
            PlayerPrefs.SetInt("CurrentLevelIndex", PlayerPrefs.GetInt("CurrentLevelIndex") + 1);
        }

        GridStateData gridProgressData = new GridStateData();
        gridProgressData.SetCellStates(cellStates);
        gridProgressData.SetSolutionCellStates(solutionCellStates);
        gridProgressData.rows = rows;
        gridProgressData.columns = columns;
        gridProgressData.meaning = solutionMeaning;
        gridProgressData.user = PlayerPrefs.GetString("Username");
        //   "./Assets/LevelsJSON/user_progress/"+user+"_progress_level_" + levelName + ".json";
        // gridProgressData.level = fileName.Split('.')[1].Split('/')[4].Split('_')[3]; // level name 
        gridProgressData.level = PlayerPrefs.GetString("LevelFilename");
        gridProgressData.onTime = levelTimer.LevelCompletedOnTime();
        gridProgressData.time = levelTimer.GetTimePassed();
        gridProgressData.levelCompletion = levelCompletion;
        gridProgressData.levelMeaningCompletion = levelMeaningCompletion;
        gridProgressData.hintCount = hint_count;

        Debug.Log("Saving grid state: " + gridProgressData.rows + " rows, " + gridProgressData.columns + " columns" + ", " + gridProgressData.cellStatesWrapper + " cell states");

        string jsonData = JsonUtility.ToJson(gridProgressData);

        // string filePath = Path.Combine(Application.persistentDataPath, fileName); // saved at C:\Users\{username}\AppData\LocalLow\DefaultCompany\{projectname}
        string filePath = fileName;

        try
        {
            File.WriteAllText(filePath, jsonData);
            Debug.Log("Grid state saved to: " + filePath);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error saving grid state: " + e.Message);
        }
    }

    public void LoadLevelToPlay(string filePath)
    {
        // string filePath = Path.Combine(Application.persistentDataPath, fileName); // load from C:\Users\{username}\AppData\LocalLow\DefaultCompany\{projectname}

        if (File.Exists(filePath))
        {
            // try
            // {
            string jsonData = File.ReadAllText(filePath);
            GridStateData gridLoadedData = JsonUtility.FromJson<GridStateData>(jsonData);

            if (gridLoadedData.onTime == false)
            {
                Debug.Log("Time has run out! You are blocked out of the level.");
                mainMenu.GoToScene("Levels");
            }

            // 1. Updating the states of the grid from the loaded data
            cellStates = gridLoadedData.GetCellStates();
            solutionCellStates = gridLoadedData.GetSolutionCellStates();
            solutionMeaning = gridLoadedData.meaning;
            levelCompletion = gridLoadedData.levelCompletion;
            levelMeaningCompletion = gridLoadedData.levelMeaningCompletion;
            levelTimer.SetTimePassed(gridLoadedData.time);
            hint_count = gridLoadedData.hintCount;
            level_size_text.text = gridLoadedData.rows + " x " + gridLoadedData.columns;

            // 2. Change grid size to match the loaded grid size, but empty states
            ChangeGridSize(gridLoadedData.rows, gridLoadedData.columns);


            // 3. Updating the indeces of the grid from the solution in the loaded data
            for (int i = 0; i < gridLoadedData.rows; i++)
            {
                UpdateRowIndexText(i, true);
            }
            for (int j = 0; j < gridLoadedData.columns; j++)
            {
                UpdateColumnIndexText(j, true);
            }

            // 4. update CellsGrouping and initialize GridInteractions
            cellsGrouping = new CellsGrouping();
            cellsGrouping.SetCellsGrouping(solutionCellStates, rows, columns);
            // Debug.Log("Grid state loaded cellsGrouping: " );
            // cellsGrouping.print(); // print the cellsGrouping
            
            gridInteractions = new GridInteractions(cellsGrouping);


            Debug.Log("Grid state loaded from: " + filePath);
            // }
            // catch (System.Exception e)
            // {
            //     Debug.LogError("Error loading grid state: " + e.Message);
            // }
        }
        else
        {
            Debug.LogError("File not found: " + filePath);
        }
    }


    /// <summary>
    /// Section for checking the solution of the puzzle
    /// Checking: grid state and user guess for meaning
    /// </summary>
    public void CheckSolution(bool[,] cellStates, bool[,] solutionCellStates)
    {
        bool solvedLevel = true;
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                bool currentCellState = cellStates[i,j];
                bool currentSolutionCellState = solutionCellStates[i, j];
                if (currentCellState != currentSolutionCellState)
                {
                    solvedLevel = false;
                    break;
                }
            }
        }
        Debug.Log("Solved level: " + solvedLevel);
        levelCompletion = solvedLevel;
        buttonAnimations.OnLevelCompletionCheck();
    }

    public void CheckMeaningSolution()
    {
        string user_meaning = meaningInputField.text;
        if (user_meaning == "" || user_meaning == "Enter...")
        {
            Debug.Log("Please enter a meaning");
            return;
        }
        StartCoroutine(httpRequests.SendPuzzleMeaningRequest(user_meaning, solutionMeaning, username: PlayerPrefs.GetString("Username"), level: PlayerPrefs.GetString("LevelFilename"))); 
        // levelMeaningCompletion is updated under update_levelMeaningCompletion() in HTTPRequests.cs
    }

    private void OnEndEdit()
    {
        // This method will be called when the user presses Enter in the input field
        checkMeaningButton.onClick.Invoke(); // Invoke the save button click event
    }


}

