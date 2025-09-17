using System.Collections;
using bmlTUX;
// ReSharper disable once RedundantUsingDirective
using UnityEngine;

/// <summary>
/// Classes that inherit from Experiment define custom behaviour for the start and end of your experiment.
/// This might useful for experiment setup, instructions, and debrief.
///
/// This template shows how to set up a custom experiment script using the toolkit's built-in functions.
///
/// You can delete any unused methods and unwanted comments. The only required part is the constructor.
///
/// You cannot edit the main execution part of experiments since their main execution is to run the trials and blocks.
/// </summary>
public class LiebetExperiment : Experiment
{
    public GameObject welcome;
    public GameObject clock;
    public GameObject endScreen;


    // // You usually want to store a reference to your experiment runner
    // YourCustomExperimentRunner myRunner;
    public LiebetRunner lRunner;

    // Required Constructor. Good place to set up references to objects in the unity scene
    public LiebetExperiment(ExperimentRunner runner, RunnableDesign runnableDesign) : base(runner, runnableDesign) 
    {
        lRunner = (LiebetRunner)runner;
        welcome = lRunner.Welcome;
        clock = lRunner.Clock;
        endScreen = lRunner.EndExperiment;
        // myRunner = (YourCustomExperimentRunner)runner;  //cast the generic runner to your custom type.
        // GameObject myGameObject = myRunner.MyGameObject;  // get reference to gameObject stored in your custom runner

    }


    // Optional Pre-Experiment code. Useful for pre-experiment calibration and setup.
    protected override void PreMethod() 
    {
        clock.SetActive(false);
    }


    // Optional Pre-Experiment code. Useful for pre-experiment instructions.
    protected override IEnumerator PreCoroutine()
    {
        welcome.SetActive(true);
        bool waitingForParticipantResponse = true;
        Debug.Log("Press the spacebar to end this trial.");
        while (waitingForParticipantResponse) 
        {   // keep check each frame until waitingForParticipantResponse set to false.
            if (Input.GetKeyDown(KeyCode.Space)) 
            { // check return key pressed
                welcome.SetActive(false);
                waitingForParticipantResponse = false;  // escape from while loop
            }
        
            yield return null; // wait for next frame while allowing rest of program to run (without this the program will hang in an infinite loop)
        } //required for coroutine
    }


    // Optional Post-Experiment code. Useful for experiment debrief instructions.
    protected override IEnumerator PostCoroutine() 
    {
        endScreen.SetActive(true);
        yield return null; //required for coroutine
    }


    // Optional Post-Experiment code.
    protected override void PostMethod() 
    {
        // cleanup code (happens all in one frame)
    }
}

