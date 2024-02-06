using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonAnimations : MonoBehaviour
{
    private Animator correct_level_animator;
    private Animator wrong_level_animator;
    private Animator correct_meaning_animator;
    private Animator wrong_meaning_animator;
    private LevelSetup levelSetup;

    private void Start()
    {
        // Get the Animator component attached to a separate GameObject
        correct_level_animator = GameObject.Find("Correct").GetComponent<Animator>();
        wrong_level_animator = GameObject.Find("Wrong").GetComponent<Animator>();
        correct_meaning_animator = GameObject.Find("CorrectMeaning").GetComponent<Animator>();
        wrong_meaning_animator = GameObject.Find("WrongMeaning").GetComponent<Animator>();

        levelSetup = GetComponent<LevelSetup>();
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
        } else {
            // Trigger the animation
            correct_meaning_animator.SetTrigger("Disable");
            wrong_meaning_animator.SetTrigger("PopUp");
        }
    }
}
