using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;


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
    private HTTPRequests httpRequests;
    private LevelTimer levelTimer;

    public Font fontAsset; // Add a field for the font asset

    public GameObject[,] cells; // 2D array to hold references to the grid cells
    private bool[,] cellStates; // 2D array to store the state of each cell
    private bool[,] solutionCellStates; // 2D array to store the Solution state of each cell
    private string solutionMeaning = "";

    public bool levelCompletion = false;
    public bool levelMeaningCompletion = false;

    void Awake()
    {
        cellPrefab = GameObject.Find("cellPrefab");
        gridParent = GameObject.Find("GridHolder").transform;
        originalCellPrefab = GameObject.Find("cellPrefab");
        checkMeaningButton = GameObject.Find("CheckMeaningButton").GetComponent<Button>();
        meaningInputField = GameObject.Find("InputField -Puzzle Meaning").GetComponent<TMP_InputField>();
        httpRequests = GetComponent<HTTPRequests>();
        levelTimer = GetComponent<LevelTimer>();
        // Add a listener to the input field's OnEndEdit event to check for Enter key
        meaningInputField.onEndEdit.AddListener(delegate { OnEndEdit(); });
    }

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

        Font font = fontAsset != null ? fontAsset : Resources.GetBuiltinResource<Font>("Arial.ttf"); // Use selected font asset or default font

        // ####################### Creating and positioning row indices
        for (int i = 0; i < rows; i++)
        {
            GameObject rowIndex = new GameObject("RowIndex_" + i);
            rowIndex.transform.SetParent(gridParent);

            Text indexText = rowIndex.AddComponent<Text>();
            indexText.font = font;
            indexText.text = "0"; //(i + 1).ToString();
            indexText.alignment = TextAnchor.MiddleCenter;
            indexText.color = Color.white;

            RectTransform indexRect = indexText.GetComponent<RectTransform>();
            indexRect.sizeDelta = new Vector2(cellSizeX, cellSizeY * 2f);
            indexRect.anchoredPosition = new Vector2(startX - (cellSizeX / 2 + indexRect.sizeDelta[0] / 5), startY - i * cellSizeY);
            // indexRect.localScale = new Vector3(1f, 1f, 1f); // Ensure the scale is reset to 1 to match cell size //zooms in

            // Adjust font size proportionally to cell size
            int fontSize = Mathf.RoundToInt(Mathf.Min(cellSizeX, cellSizeY) * 1f); // Adjust the multiplier as needed
            indexText.fontSize = fontSize;
        }

        // ####################### Creating and positioning column indices
        for (int j = 0; j < columns; j++)
        {
            GameObject colIndex = new GameObject("ColIndex_" + j);
            colIndex.transform.SetParent(gridParent);

            Text indexText = colIndex.AddComponent<Text>();
            indexText.font = font;
            indexText.text = "0"; //(j + 1).ToString();
            indexText.alignment = TextAnchor.MiddleCenter;
            indexText.color = Color.white;

            RectTransform indexRect = indexText.GetComponent<RectTransform>();
            indexRect.sizeDelta = new Vector2(cellSizeX * 2f, cellSizeY);
            indexRect.anchoredPosition = new Vector2(startX + j * cellSizeX, startY + (cellSizeY / 2 + indexRect.sizeDelta[1] / 5));
            // indexRect.localScale = new Vector3(1f, 1f, 1f); // Ensure the scale is reset to 1 to match cell size //zooms in

            // Adjust font size proportionally to cell size
            int fontSize = Mathf.RoundToInt(Mathf.Min(cellSizeX, cellSizeY) * 1f); // Adjust the multiplier as needed
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

    // Function to change grid size dynamically
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



    // ####################### Cell State Management
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

    // Update row indices text with consecutive pressed cell counts
    void UpdateRowIndexText(int rowIndex, bool solution = false)
    {
        Transform rowTextTransform = gridParent.Find("RowIndex_" + rowIndex);
        if (rowTextTransform != null)
        {
            Text indexText = rowTextTransform.GetComponent<Text>();
            if (indexText != null)
            {
                RectTransform indexRect = indexText.GetComponent<RectTransform>();
                float previous_text_width = indexRect.rect.width; // remember the previous text width

                string consecutiveCount = CalculateGroupPressedCellsPerRow(rowIndex, solution);
                indexText.text = consecutiveCount;

                // Calculate preferred size of the text
                Vector2 preferredSize = indexText.preferredWidth > indexText.preferredHeight ?
                    new Vector2(indexText.preferredWidth, indexText.preferredHeight) :
                    new Vector2(indexText.preferredHeight, indexText.preferredWidth);


                // Calculate the minimum size (size of the cell in the grid)
                float cellSizeY = gridParent.GetComponent<RectTransform>().rect.height / rows;
                Vector2 minSize = new Vector2(cellSizeY, cellSizeY);

                // Set the size to the maximum between preferred size and minimum cell size
                Vector2 newSize = new Vector2(
                    Mathf.Max(preferredSize.x, minSize.x),
                    Mathf.Max(preferredSize.y, minSize.y)
                );
                indexRect.sizeDelta = newSize;

                // Calculate the difference in size for position adjustment
                float difference = (indexRect.rect.width - previous_text_width) / 6f; // Difference in text width

                // Shift the text to the left based on the text size difference
                indexRect.anchoredPosition -= new Vector2(difference, 0f);
            }
        }
    }

    // Update column indices text with consecutive pressed cell counts
    void UpdateColumnIndexText(int colIndex, bool solution = false)
    {
        Transform colTextTransform = gridParent.Find("ColIndex_" + colIndex);
        if (colTextTransform != null)
        {
            Text indexText = colTextTransform.GetComponent<Text>();
            if (indexText != null)
            {
                RectTransform indexRect = indexText.GetComponent<RectTransform>();
                float previous_text_height = indexRect.rect.height; // remember the previous text width

                string consecutiveCount = CalculateGroupPressedCellsPerColumn(colIndex, solution);
                indexText.text = consecutiveCount;

                // Calculate preferred size of the text
                Vector2 preferredSize = indexText.preferredHeight > indexText.preferredWidth ?
                    new Vector2(indexText.preferredWidth, indexText.preferredHeight) :
                    new Vector2(indexText.preferredHeight, indexText.preferredWidth);

                // Calculate the minimum size (size of the cell in the grid)
                float cellSizeX = gridParent.GetComponent<RectTransform>().rect.width / columns;
                Vector2 minSize = new Vector2(cellSizeX, cellSizeX);

                // Set the size to the maximum between preferred size and minimum cell size
                Vector2 newSize = new Vector2(
                    Mathf.Max(preferredSize.x, minSize.x),
                    Mathf.Max(preferredSize.y, minSize.y)
                );
                indexRect.sizeDelta = newSize;
                // rotate by -90 degrees
                // indexRect.Rotate(new Vector3(0, 0, -90));

                float difference = (indexRect.rect.height - previous_text_height) / 6f;
                indexRect.anchoredPosition += new Vector2(0f, difference);
            }
        }
    }

    // Method to calculate the count of consecutive pressed cells in a row
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
                rowGroupText += currentGroup + " ";
                currentGroup = 0;
            }
        }
        if (currentGroup > 0)
        {
            rowGroupText += currentGroup + " ";
        }

        if (rowGroupText.Length == 0)
        {
            rowGroupText = "0";
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


    // ####################### Grid State Management (Save & Load)
    [System.Serializable]
    public class GridStateData
    {
        public int rows;
        public int columns;
        public string meaning;
        public string user;
        public string level;
        public bool onTime;
        public float time;
        public bool levelCompletion;
        public CellStatesWrapper cellStatesWrapper;
        public CellStatesWrapper solutionCellStatesWrapper;

        public void SetCellStates(bool[,] cellStates)
        {
            cellStatesWrapper = new CellStatesWrapper(cellStates);
            rows = cellStates.GetLength(0);
            columns = cellStates.GetLength(1);
        }

        public bool[,] GetCellStates()
        {
            return cellStatesWrapper.GetCellStates(rows, columns);
        }

        public void SetSolutionCellStates(bool[,] cellStates)
        {
            solutionCellStatesWrapper = new CellStatesWrapper(cellStates);
        }

        public bool[,] GetSolutionCellStates()
        {
            return solutionCellStatesWrapper.GetCellStates(rows, columns);
        }
    }

    [System.Serializable]
    public class CellStatesWrapper
    {
        public bool[] cellStates;

        public CellStatesWrapper(bool[,] cellStates)
        {
            int rows = cellStates.GetLength(0);
            int columns = cellStates.GetLength(1);
            this.cellStates = new bool[rows * columns];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    this.cellStates[i * columns + j] = cellStates[i, j];
                }
            }
        }

        public bool[,] GetCellStates(int rows, int columns)
        {
            bool[,] result = new bool[rows, columns];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    result[i, j] = cellStates[i * columns + j];
                }
            }

            return result;
        }
    }
    public void SaveProgress(string fileName)
    {
        // check if solved correctly
        CheckSolution();
        // check if meaning is correct
        CheckMeaningSolution();

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
            GridStateData gridSolutionData = JsonUtility.FromJson<GridStateData>(jsonData);

            // 1. Updating the states of the grid from the loaded data
            cellStates = gridSolutionData.GetCellStates();
            solutionCellStates = gridSolutionData.GetSolutionCellStates();
            solutionMeaning = gridSolutionData.meaning;
            levelCompletion = gridSolutionData.levelCompletion;
            levelTimer.SetTimePassed(gridSolutionData.time);

            // 2. Change grid size to match the loaded grid size, but empty states
            ChangeGridSize(gridSolutionData.rows, gridSolutionData.columns);


            // 3. Updating the indeces of the grid from the solution in the loaded data
            for (int i = 0; i < gridSolutionData.rows; i++)
            {
                UpdateRowIndexText(i, true);
            }
            for (int j = 0; j < gridSolutionData.columns; j++)
            {
                UpdateColumnIndexText(j, true);
            }


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


    // CHECKING SOLUTIONS
    public void CheckSolution()
    {
        bool solvedLevel = true;
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                bool currentCellState = GetCellState(i, j);
                bool currentSolutionCellState = GetSolutionCellState(i, j);
                if (currentCellState != currentSolutionCellState)
                {
                    solvedLevel = false;
                    break;
                }
            }
        }
        Debug.Log("Solved level: " + solvedLevel);
        levelCompletion = solvedLevel;
        // return solvedLevel;
    }

    public void CheckMeaningSolution()
    {
        string user_meaning = meaningInputField.text;
        if (user_meaning == "")
        {
            Debug.Log("Please enter a meaning");
            return;
        }
        StartCoroutine(httpRequests.SendPuzzleMeaningRequest(user_meaning, solutionMeaning)); 
    }

    private void OnEndEdit()
    {
        // This method will be called when the user presses Enter in the input field
        checkMeaningButton.onClick.Invoke(); // Invoke the save button click event
    }


}

