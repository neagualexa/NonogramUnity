using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class NonogramGrid : MonoBehaviour
{
    public int rows = 4; // Number of rows in the grid
    public int columns = 4; // Number of columns in the grid

    public GameObject cellPrefab; // Prefab for the grid cell
    public Transform gridParent; // Parent object for the grid cells
    public GameObject originalCellPrefab;

    public Font fontAsset; // Add a field for the font asset

    private GameObject[,] cells; // 2D array to hold references to the grid cells
    private bool[,] cellStates; // 2D array to store the state of each cell


    void Start()
    {
        CreateGrid();
        // Attach a listener to the event that triggers when cell states change
        OnCellStateChanged += HandleCellStateChanged;
    }

    // Method to handle the cell state change event
    void HandleCellStateChanged()
    {
        // PrintCellStates(); // Call PrintCellStates whenever cell states change
        // UpdateGridIndices(); // Update the row and column indices whenever cell states change
    }

    void CreateGrid()
    {
        cells = new GameObject[rows, columns];  // Initialize the 2D array for cells
        cellStates = new bool[rows, columns];   // Initialize the 2D array for cell states

        // Calculate the size of each cell based on the grid size
        float cellSizeX = gridParent.GetComponent<RectTransform>().rect.width / columns;
        float cellSizeY = gridParent.GetComponent<RectTransform>().rect.height / rows;

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
            int fontSize = Mathf.RoundToInt(Mathf.Min(cellSizeX, cellSizeY) * 0.8f); // Adjust the multiplier as needed
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
            int fontSize = Mathf.RoundToInt(Mathf.Min(cellSizeX, cellSizeY) * 0.8f); // Adjust the multiplier as needed
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

                // Set initial state of the cell
                cellStates[i, j] = false; // Assuming all cells start as unpressed

                // Attach the GridCellToggle script to handle toggle behavior
                GridCellToggle cellToggle = cell.AddComponent<GridCellToggle>();
                cellToggle.SetGridStateReference(this, i, j); // Pass reference to the grid and cell indices

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

    public bool GetCellState(int row, int column)
    {
        return cellStates[row, column];
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

        UpdateRowIndexText(row);
        UpdateColumnIndexText(column);
    }

    // Function to print the states of all cells in the grid
    public void PrintCellStates()
    {
        Debug.Log("####################### Updated Cell States #######################");
        for (int i = 0; i < rows; i++)
        {
            string rowStates = "";
            for (int j = 0; j < columns; j++)
            {
                bool cellState = GetCellState(i, j);
                rowStates += (cellState ? "1" : "0") + " "; // Assuming true is represented by "1" and false by "0"
            }
            Debug.Log("Row " + (i + 1) + ": " + rowStates);
        }
    }

    // Update row indices text with consecutive pressed cell counts
    void UpdateRowIndexText(int rowIndex)
    {
        Transform rowTextTransform = gridParent.Find("RowIndex_" + rowIndex);
        if (rowTextTransform != null)
        {
            Text indexText = rowTextTransform.GetComponent<Text>();
            if (indexText != null)
            {
                RectTransform indexRect = indexText.GetComponent<RectTransform>();
                float previous_text_width = indexRect.rect.width; // remember the previous text width

                string consecutiveCount = CalculateGroupPressedCellsPerRow(rowIndex);
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
    void UpdateColumnIndexText(int colIndex)
    {
        Transform colTextTransform = gridParent.Find("ColIndex_" + colIndex);
        if (colTextTransform != null)
        {
            Text indexText = colTextTransform.GetComponent<Text>();
            if (indexText != null)
            {
                RectTransform indexRect = indexText.GetComponent<RectTransform>();
                float previous_text_height = indexRect.rect.height; // remember the previous text width

                string consecutiveCount = CalculateGroupPressedCellsPerColumn(colIndex);
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
    string CalculateGroupPressedCellsPerRow(int rowIndex)
    {
        int currentGroup = 0;
        string rowGroupText = "";
        for (int j = 0; j < columns; j++)
        {
            bool currentCellState = GetCellState(rowIndex, j);
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

    string CalculateGroupPressedCellsPerColumn(int colIndex)
    {
        int currentGroup = 0;
        string colGroupText = "";
        for (int j = 0; j < columns; j++)
        {
            bool currentCellState = GetCellState(j, colIndex);
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
        public CellStatesWrapper cellStatesWrapper;

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
    public void SaveGridState(string fileName)
    {
        GridStateData gridData = new GridStateData();
        gridData.SetCellStates(cellStates);
        gridData.rows = rows;
        gridData.columns = columns;

        Debug.Log("Saving grid state: " + gridData.rows + " rows, " + gridData.columns + " columns" + ", " + gridData.cellStatesWrapper + " cell states");

        string jsonData = JsonUtility.ToJson(gridData);

        string filePath = Path.Combine(Application.persistentDataPath, fileName); // saved at C:\Users\{username}\AppData\LocalLow\DefaultCompany\{projectname}

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

    public void LoadGridState(string fileName)
    {
        string filePath = Path.Combine(Application.persistentDataPath, fileName); // load from C:\Users\{username}\AppData\LocalLow\DefaultCompany\{projectname}

        if (File.Exists(filePath))
        {
            // try
            // {
                string jsonData = File.ReadAllText(filePath);
                GridStateData gridData = JsonUtility.FromJson<GridStateData>(jsonData);

                // 1. Change grid size to match the loaded grid size, but empty states
                ChangeGridSize(gridData.rows, gridData.columns); 

                // 2. Updating the states of the grid from the loaded data
                bool[,] loadedCellStates = gridData.GetCellStates();
                Debug.Log("Loaded grid state: " + loadedCellStates[0, 0] + loadedCellStates[0,1]);
                for (int i = 0; i < gridData.rows; i++)
                {
                    for (int j = 0; j < gridData.columns; j++)
                    {
                        bool loadedCellState = loadedCellStates[i, j];
                        GridCellToggle cellToggle = cells[i, j].GetComponent<GridCellToggle>();
                        if (cellToggle != null)
                        {
                            // Debug.Log("Loaded cell state: " + i + ", " + j + ": " + loadedCellState);
                            // 3. Update row and column indices by updating the button cell state
                            cellToggle.SetGridStateReference(this, i, j);
                            cellToggle.UpdateButton(loadedCellState);                           //TODO: error as indexes are not updated if they were covered by the grid beforehand (size)
                        }
                    }
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


}

