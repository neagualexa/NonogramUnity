using System;
// using System.IO;
using UnityEngine;
// using System.Collections;
using System.Collections.Generic;

namespace Interactions
{
    /// <summary>
    /// This class is used to store the size of the group that each cell is part of.
    /// </summary>
    [System.Serializable]
    public class CellsGrouping
    {
        public int[,] cellGroupingRows;
        public int[,] cellGroupingColumns;

        public int[,] GetCellGroupingRows()
        {
            return cellGroupingRows;
        }

        public int[,] GetCellGroupingColumns()
        {
            return cellGroupingColumns;
        }

        public void SetCellsGrouping(bool[,] cellStates, int rows, int columns)
        {
            cellGroupingRows = new int[rows, columns];
            cellGroupingColumns = new int[rows, columns];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    GetCellGroupSize(cellStates, i, j);
                }
            }
        }

        private void GetCellGroupSize(bool[,] cellStates, int row, int column)
        {
            this.cellGroupingRows[row,column] = FindGroupSizePerRow(cellStates, row, column);
            this.cellGroupingColumns[row,column] = FindGroupSizePerColumn(cellStates, row,column);
        }

        private int FindGroupSizePerRow(bool[,] cellStates, int row, int column)
        {
            // calculate the size of the group of pressed cells in a row
            int currentGroup = 0;
            int column_copy = column;
            int sizeRow = cellStates.GetLength(0);

            if (!cellStates[row, column])
            {
                return 0;
            }

            // Initial Condition, check if this cell is next to a cell that is already part of a group; skip the while loops
            if (column > 0 && cellStates[row,column-1] && cellStates[row, column])
            {
                return this.cellGroupingRows[row, column-1];
            }

            // start from the column index on the given row and count towrds left and right until a cell is False
            while (column_copy >= 0 && cellStates[row, column_copy])
            {
                // start from the cell to the left
                currentGroup++;
                column_copy--;
            }
            while (column < sizeRow && cellStates[row, column])
            {
                // start from the cell to the right
                currentGroup++;
                column++;
            }
            currentGroup = currentGroup - 1; // remove the double count of the current cell
            // Debug.Log("FindGroupSizePerRow Row: " + row + " Column: " + column + " column_copy:"+column_copy+" Group Size: " + currentGroup);
            return currentGroup;
        }

        private int FindGroupSizePerColumn(bool[,] cellStates, int row, int column)
        {
            // calculate the size of the group of pressed cells in a column
            int currentGroup = 0;
            int row_copy = row;
            int sizeColumn = cellStates.GetLength(1);

            if (!cellStates[row, column])
            {
                return 0;
            }

            // Initial Condition, check if this cell is next to a cell that is already part of a group; skip the while loops
            if (row > 0 && cellStates[row-1,column] && cellStates[row, column])
            {
                return this.cellGroupingColumns[row-1, column];
            }
            
            // start from the row index on the given column and count towrds up and down until a cell is False
            while (row_copy >= 0 && cellStates[row_copy, column])
            {
                // start from the cell to up
                currentGroup++;
                row_copy--;
            }
            while (row < sizeColumn && cellStates[row, column])
            {
                // start from the cell to the bottom
                currentGroup++;
                row++;
            }
            currentGroup = currentGroup - 1; // remove the double count of the current cell
            // Debug.Log("FindGroupSizePerColumn Row: " + row + " Column: " + column + " row_copy:"+row_copy+" Group Size: " + currentGroup);
            return currentGroup;
        }

        public void print()
        {
            Debug.Log("Rows: " + this.cellGroupingRows);
            int rows = this.cellGroupingRows.GetLength(0); // Number of rows
            int columns = this.cellGroupingRows.GetLength(1); // Number of columns

            for (int i = 0; i < rows; i++)
            {
                string rowOutput = "";
                for (int j = 0; j < columns; j++)
                {
                    rowOutput += this.cellGroupingRows[i, j].ToString() + " ";
                }
                Debug.Log(rowOutput.Trim()); // Print each row
            }

            Debug.Log("Columns: " + this.cellGroupingColumns);
            for (int i = 0; i < rows; i++)
            {
                string rowOutput = "";
                for (int j = 0; j < columns; j++)
                {
                    rowOutput += this.cellGroupingColumns[i, j].ToString() + " ";
                }
                Debug.Log(rowOutput.Trim()); // Print each row
            }
        }
    }

    public class GridInteractions
    {
        public List<int> lastPressedCell_1;
        public List<int> lastPressedCell_2;
        public List<int> lastPressedCell_3;
        CellsGrouping cellsGrouping;
        private int StateMachine = 0;

        private void UpdateStateMachine(int StateMachine_prev)
        {
            this.StateMachine = (StateMachine_prev + 1) % 3;
        }

        public GridInteractions(CellsGrouping cellsGrouping)
        {
            this.cellsGrouping = cellsGrouping;
        }

        // each lastPressedCell_i is a list of ints: (Row, Column, Row Group Size, Column Group Size)
        public void SetLastPressedCell(int row, int column)
        {
            int rowGroupSize = cellsGrouping.cellGroupingRows[row, column];
            int columnGroupSize = cellsGrouping.cellGroupingColumns[row, column];

            // switch (StateMachine)
            // {
            //     case 0:
            //         lastPressedCell_1 = new List<int> {row, column, rowGroupSize, columnGroupSize};
            //         UpdateStateMachine(StateMachine);
            //         break;
            //     case 1:
            //         lastPressedCell_2 = new List<int> {row, column, rowGroupSize, columnGroupSize};
            //         UpdateStateMachine(StateMachine);
            //         break;
            //     case 2:
            //         lastPressedCell_3 = new List<int> {row, column, rowGroupSize, columnGroupSize};
            //         UpdateStateMachine(StateMachine);
            //         break;
            //     default:
            //         Debug.Log("Invalid State Machine Value");
            //         break;
            // }
            lastPressedCell_3 = lastPressedCell_2;
            lastPressedCell_2 = lastPressedCell_1;
            lastPressedCell_1 = new List<int> {row, column, rowGroupSize, columnGroupSize};
        }
    }
}