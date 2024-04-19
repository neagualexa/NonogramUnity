namespace GridData{
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
        public bool levelMeaningCompletion;
        public int hintCount;
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
}