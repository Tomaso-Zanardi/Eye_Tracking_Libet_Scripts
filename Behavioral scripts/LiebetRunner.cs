using bmlTUX;
// ReSharper disable once RedundantUsingDirective
using UnityEngine;

/// <summary>
/// This class is the main communication between the toolkit and the Unity scene. Drag this script onto an empty gameObject in your Unity scene.
/// In the gameObject's inspector you need to drag in your design file and any custom scripts.
/// </summary>
public class LiebetRunner : ExperimentRunner 
{

    //Here is where you make a list of objects in your unity scene that need to be referenced by your scripts.
    //public GameObject ReferenceToGameObject;
    public GameObject Welcome;
    public GameObject Clock;
    public GameObject ClockCenter;
    public GameObject Lancetta1;
    public GameObject Lancetta2;
    public GameObject ClockCenter_Slow;
    public GameObject Practice1;
    public GameObject Practice2;
    
    public GameObject Practice1AB;
    public GameObject Practice2AB;
    public GameObject PauseScreen;
    public GameObject Practice3R;
    public GameObject Practice3B;
    public GameObject beforeEnd;
    
    public GameObject ExperInstructions;
    public GameObject ExperInstructionsB;
    public GameObject FixationCross;
    public GameObject PleaseFaster;
    public GameObject PleaseSlower;
    public GameObject Head;
    public GameObject EndExperiment;

    public GameObject Whithin;

    public Material blue, yellow, red;

}


 
    

