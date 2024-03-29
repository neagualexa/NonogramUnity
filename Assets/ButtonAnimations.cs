using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

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

    private void Start()
    {
        // Get the Animator component attached to a separate GameObject
        correct_level_animator = GameObject.Find("Correct").GetComponent<Animator>();
        wrong_level_animator = GameObject.Find("Wrong").GetComponent<Animator>();
        correct_meaning_animator = GameObject.Find("CorrectMeaning").GetComponent<Animator>();
        wrong_meaning_animator = GameObject.Find("WrongMeaning").GetComponent<Animator>();
        error_meaning_animator = GameObject.Find("ErrorMeaning").GetComponent<Animator>();
        hint_section_animator = GameObject.Find("HintSection").GetComponent<Animator>();

        levelSetup = GetComponent<LevelSetup>();
        levelWrapper = GetComponent<LevelWrapper>();
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
}
