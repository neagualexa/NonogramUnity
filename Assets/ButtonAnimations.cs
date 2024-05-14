using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;

public class ButtonAnimations : MonoBehaviour
{
    private Animator correct_level_animator;
    private Animator wrong_level_animator;
    private Animator correct_meaning_animator;
    private Animator wrong_meaning_animator;
    private Animator error_meaning_animator;
    private Animator hint_section_animator;
    private LevelSetup levelSetup;
    private LevelWrapper levelWrapper;
    private TMP_Text instruction_hint_text;

    private void Start()
    {
        // Get the Animator component attached to a separate GameObject
        correct_level_animator = GameObject.Find("Correct").GetComponent<Animator>();
        wrong_level_animator = GameObject.Find("Wrong").GetComponent<Animator>();
        correct_meaning_animator = GameObject.Find("CorrectMeaning").GetComponent<Animator>();
        wrong_meaning_animator = GameObject.Find("WrongMeaning").GetComponent<Animator>();
        error_meaning_animator = GameObject.Find("ErrorMeaning").GetComponent<Animator>();
        hint_section_animator = GameObject.Find("HintSection").GetComponent<Animator>();
        instruction_hint_text = GameObject.Find("InstructionsHints").GetComponent<TMP_Text>();

        levelSetup = GetComponent<LevelSetup>();
        levelWrapper = GetComponent<LevelWrapper>();

        InnactiveHintButton();
    }

    public void OnLevelCompletionCheck()
    {
        if (levelSetup.levelCompletion){
            // Trigger the animation
            wrong_level_animator.SetTrigger("Disable");
            correct_level_animator.SetTrigger("PopUp");
        } else {
            // Trigger the animation
            correct_level_animator.SetTrigger("Disable");
            wrong_level_animator.SetTrigger("PopUp");
        }
    }

    public void OnMeaningCompletionCheck()
    {
        if (levelSetup.levelMeaningCompletion){
            // Trigger the animation
            wrong_meaning_animator.SetTrigger("Disable");
            correct_meaning_animator.SetTrigger("PopUp");
            error_meaning_animator.SetTrigger("Disable");
        } else {
            // Trigger the animation
            correct_meaning_animator.SetTrigger("Disable");
            wrong_meaning_animator.SetTrigger("PopUp");
            error_meaning_animator.SetTrigger("Disable");
        }
    }

    public void OnMeaningServerError()
    {
        // Trigger the animation
        correct_meaning_animator.SetTrigger("Disable");
        wrong_meaning_animator.SetTrigger("Disable");
        error_meaning_animator.SetTrigger("PopUp");
    }

    public void OnHintSectionToggle()
    {
        // if the hint section is already visible, hide it
        
        if (hint_section_animator.GetCurrentAnimatorStateInfo(0).IsName("HintAppear")){
            print("HINT Disappearing");
            hint_section_animator.SetTrigger("Disappear");
            return;
        }
        else if (hint_section_animator.GetCurrentAnimatorStateInfo(0).IsName("idle")){
            print("HINT Appearing");
            hint_section_animator.SetTrigger("Appear");
            levelWrapper.ShowHint();
            return;
        }

    }

    public void SetHintSectionVisible()
    {
        Debug.Log("Setting hint section visible");
        // if the hint section is already visible, do not trigger the animation again
        if (hint_section_animator.GetCurrentAnimatorStateInfo(0).IsName("HintAppear")){
            print("HINT Already open");
            return;
        }
        hint_section_animator.SetTrigger("Appear");
    }

    public void InnactiveHintButton()
    {
        // if current level is car or invertedcar then disable the hint button
        if (PlayerPrefs.GetString("LevelFilename") == "car" || PlayerPrefs.GetString("LevelFilename") == "invertedcar")
        {
            GameObject.Find("HintButton").GetComponent<Button>().interactable = false;
            instruction_hint_text.text = "Hints not available for this level";
        }
        else
        {
            GameObject.Find("HintButton").GetComponent<Button>().interactable = true;
            instruction_hint_text.text = "";
        }
    }
}
