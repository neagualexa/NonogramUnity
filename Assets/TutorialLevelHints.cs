using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class TutorialLevelHints : MonoBehaviour {

    private ButtonAnimations buttonAnimations;
    private TMP_Text hint_text;
    private Animator arrowUIHint_animator;
    private Animator arrowGridInfo_animator;
    private Animator arrowCell_animator;
    private Animator arrowColumn_animator;
    private Animator arrowRow_animator;
    private Animator arrowMeaning_animator;
    private Animator arrowCheck_animator;
    private Animator arrowHint_animator;

    private List<Animator> animation_list;
    private int UIHintIndex = 0;

    private string[] UI_Hints = {
        "Welcome to the tutorial level! You will have to solve the Nonogram puzzle and guess the meaning of the image it represents. \nClick 'Next' to move on to the next tutorial hint.",
        "On the left is the grid where you will be solving the puzzles. Below it, you can view details like the cell you've clicked, the grid size (rows x columns), and the time remaining for the level.",
        "To fill a cell, click on the corresponding square in the grid. Click again to cross it out and click a third time to mark it back as empty. \nCrossing a cell helps you identify squares that should not be filled, it is considered empty.",
        "Here you can find the clues for each row and their respective index number in brackets.",
        "The clues indicate how many consecutive squares should be filled. If there are multiple numbers, there is at least one empty square between groups of filled squares.",
        "Here you can find the clues for each column and their respective index number. \nThe same rules apply as for the row clues.",
        "This is where you can guess what the completed Nonogram image represents. Once you finish the puzzle, you can check your guess by pressing the check button or hitting the <Enter> key.",
        "Here you can check if your current progress of the puzzle is correct. If you think you have completed the puzzle, press the check button to see if your solution is correct.",
        "Once you complete the level and guess the correct meaning, the timer at the bottom will stop and you will be able to move on to the next level.",
        "Lastly, here you can request for a hint. \nI highly recommend pressing the hint button if you need help understanding the game rules or getting unstuck.",
        "The hints aim to understand your progress and describe strategies to solve the Nonogram or point out areas to focus on next. Mistakes found will be given in (row, column) format.",
        "If you encounter any errors or long loading times in receiving the hints, don't worry and just request for another one."
    };

    void Awake() {
        buttonAnimations = GetComponent<ButtonAnimations>();
        hint_text = GameObject.Find("HintText").GetComponent<TMP_Text>();
        arrowUIHint_animator = GameObject.Find("ArrowUIHint").GetComponent<Animator>();
        arrowGridInfo_animator = GameObject.Find("ArrowGridInfo").GetComponent<Animator>();
        arrowCell_animator = GameObject.Find("ArrowCell").GetComponent<Animator>();
        arrowColumn_animator = GameObject.Find("ArrowColumn").GetComponent<Animator>();
        arrowRow_animator = GameObject.Find("ArrowRow").GetComponent<Animator>();
        arrowMeaning_animator = GameObject.Find("ArrowMeaning").GetComponent<Animator>();
        arrowCheck_animator = GameObject.Find("ArrowCheck").GetComponent<Animator>();
        arrowHint_animator = GameObject.Find("ArrowHint").GetComponent<Animator>();

        animation_list = new List<Animator> {
            arrowUIHint_animator,
            arrowGridInfo_animator,
            arrowCell_animator,
            arrowRow_animator,
            arrowRow_animator,
            arrowColumn_animator,
            arrowMeaning_animator,
            arrowCheck_animator,
            arrowGridInfo_animator,
            arrowHint_animator,
            arrowHint_animator,
            arrowHint_animator
        };

        animation_list[0].SetTrigger("ArrowPopUp");
    }

    void Update(){
        // if last hint, disable the next button
        if ( UIHintIndex >= UI_Hints.Length ) {
            GameObject.Find("NextHintIcon").GetComponent<Button>().interactable = false;
            GameObject.Find("NextText").GetComponent<TMP_Text>().text = "";
        }
        else {
            GameObject.Find("NextHintIcon").GetComponent<Button>().interactable = true;
            GameObject.Find("NextText").GetComponent<TMP_Text>().text = "Next";
        }
    }

    public void ShowUIHints() {
        UIHintIndex = 0;
        // open Hint popup panel with a welcome message
        buttonAnimations.SetHintSectionVisible();

        NextHint();
    }

    public void CloseHintPanel() {
        GameObject.Find("HintPanel").SetActive(false);
    }

    public void NextHint() {
        // increment the hint index
        if ( UIHintIndex >= UI_Hints.Length ) {
            // close the hint panel
            hint_text.text = "You have reached the end of the tutorial level hints. Good luck on completing the first level!";
        }
        else if ( UIHintIndex == 0 ) {
            hint_text.text = UI_Hints[UIHintIndex];
            UIHintIndex += 1;
        }
        else
        {
            animation_list[UIHintIndex].SetTrigger("ArrowPopUp");
            hint_text.text = UI_Hints[UIHintIndex];
            UIHintIndex += 1;
        }
    }   

    public void BackHint() {
        // decrement the hint index
        if ( UIHintIndex <= 0 ) {
            UIHintIndex = 0;
            hint_text.text = UI_Hints[UIHintIndex];
        }
        else 
        {
            UIHintIndex -= 1;
            animation_list[UIHintIndex].SetTrigger("ArrowPopUp");
            hint_text.text = UI_Hints[UIHintIndex];
        }
        
    }

}