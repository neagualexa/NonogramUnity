using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// To activate testbench, the Start loop in LevelManager.cs should be commented out.
/// </summary>

public class HintTestbench : MonoBehaviour
{
    private LevelSetup levelGrid;
    private HTTPRequests httpRequests;
    private string level;
    private string user;

    void Awake()
    {
        levelGrid = GameObject.Find("Panel").GetComponent<LevelSetup>();
        httpRequests = GameObject.Find("Panel").GetComponent<HTTPRequests>();

        level = PlayerPrefs.GetString("LevelFilename");
        user = PlayerPrefs.GetString("Username");
    }

    /// <summary>
    /// Start the testbench
    /// Function attached to temporary button in the Level Scene UI
    /// </summary>
    public void StartTestbench()
    {
        StartCoroutine(StartTestbenchCoroutine());
    }
    private IEnumerator StartTestbenchCoroutine()
    {
        for (int i = 0; i < levelGrid.rows; i++)
        {
            for (int j = 0; j < levelGrid.columns; j++)
            {
                bool[,] solutionCellStates = levelGrid.GetSolutionCellStates();
                string levelMeaning = levelGrid.GetSolutionMeaning();
                levelGrid.hint_count += 1;
                AskAIAssistant_testbench(solutionCellStates, levelMeaning, i, j, levelGrid.hint_count);
                yield return new WaitForSeconds(10f); // wait for couple of seconds and start the next iteration
            }
        }

    }

    private void AskAIAssistant_testbench(bool[,] solutionCellStates, string levelMeaning, int row, int column, int hint_id)
    {
        // create a copy of solutionCellStates
        bool[,] cellStates = new bool[solutionCellStates.GetLength(0), solutionCellStates.GetLength(1)];
        Array.Copy(solutionCellStates, cellStates, solutionCellStates.Length);
        // modify one location in the copy
        cellStates = ToggleCellStateAtLocation(cellStates,row,column);
        Debug.Log("TESTBENCH:: (row,col):("+(row+1)+","+(column+1)+") Sending puzzle progress to server..." + cellStates[row,column] +"-"+ solutionCellStates[row,column]);
        // send the modified copy to the server
        StartCoroutine(httpRequests.SendPuzzleProgressRequest(cellStates, solutionCellStates, levelMeaning, user, level, hint_id));
    }

    private bool[,] ToggleCellStateAtLocation(bool[,] cellStates, int row, int column)
    {
        cellStates[row, column] = !cellStates[row, column];
        Debug.Log("TESTBENCH:: Toggling cell state at location (" + (row + 1) + "," + (column + 1) + ") to " + cellStates[row, column]);
        return cellStates;
    }

}