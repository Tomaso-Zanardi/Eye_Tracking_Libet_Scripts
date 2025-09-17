using System.Collections;
using System.Data;
using bmlTUX;
using Realter;
using UnityEngine;

/// <summary>
/// Classes that inherit from Trial define custom behaviour for your experiment's trials.
/// Most experiments will need to edit this file to describe what happens in a trial.
///
/// This template shows how to set up a custom trial script using the toolkit's built-in functions.
///
/// You can delete any unused methods and unwanted comments. The only required parts are the constructor and the MainCoroutine.
/// </summary>
public class LiebetTrial : Trial
{
    public int t;
    public string block_type;

    public GameObject clock;
    public GameObject pract1,pract1B;
    public GameObject pract2;
    public GameObject pract3R, pract3B;
    public GameObject expInst;
    public GameObject expInstB;
    public GameObject plsFaster, plsSlower;
    public GameObject clockcenter;
    public GameObject clockcenter_slow;
    public GameObject fixationcross;
    public GameObject pause;
    public GameObject beforeEnd;

    public GameObject head;

    // // You usually want to store a reference to your experiment runner
    // YourCustomExperimentRunner myRunner;
    public LiebetRunner lRunner;
    public Behaviour rotationx, rotationx_slow, positionx, eyeTracker;
    
    public int countValue, countValue_slow;
    
    
    //dependent variables variables
    public float timechoice; //the clock position at which participants say they decided

    public float pressTime,
        pressTime_Slow, // the time of the clock when they pressed
        appearClockTime, //the time the clock shown upon appearing
        appearClockTime_slow,
        disappearClockTime, //the time the clock shown upon appearing
        appearChoice, //the time the clock shown upon appearing for them to choose
        decisionTimeUnity, //the time of Unity when the clock shown upon appearing
        pressTimeUnity, // the time of Unity when they pressed 
        disappearTimeUnity, // 
        appearTimeUnity,
        disappearClockTime_Slow,
        disappearTimeUnity_Slow,
        appearChoiceUnity,
        trialTimeStartms,
        length;
    public string trialcomplete ;
    public static float pressMs;

    public Material clockarm_WAIT;

    public GameObject lancetta1, lancetta2;
    public Material b, y, r, fast, slow;

    public bool ar, ab, rf, bf;


    // Required Constructor. Good place to set up references to objects in the unity scene
    public LiebetTrial(ExperimentRunner runner, DataRow data) : base(runner, data) 
    {
        lRunner = (LiebetRunner)runner;
        clock = lRunner.Clock;
        pract1 = lRunner.Practice1;
        pract1B = lRunner.Practice1AB;
        pract2 = lRunner.Practice2;
        pract3R = lRunner.Practice3R;
        pract3B = lRunner.Practice3B;
        expInst = lRunner.ExperInstructions;
        expInstB = lRunner.ExperInstructionsB;
        plsFaster = lRunner.PleaseFaster;
        plsSlower = lRunner.PleaseSlower;
        clockcenter = lRunner.ClockCenter;
        lancetta1 = lRunner.Lancetta1;
        lancetta2 = lRunner.Lancetta2;
        pause = lRunner.PauseScreen;
        beforeEnd = lRunner.beforeEnd;
        clockcenter_slow = lRunner.ClockCenter_Slow;
        fixationcross = lRunner.FixationCross;
        rotationx  = clockcenter.GetComponent<RotateX>();
        rotationx_slow = clockcenter_slow.GetComponent<RotateX_Slow>();
        positionx = clockcenter.GetComponent<positionXinactive>();
        countValue = clockcenter.GetComponent<RotateX>().rotationCount;
        appearClockTime = clockcenter.GetComponent<RotateX>().currentRotationX;
        countValue_slow = clockcenter_slow.GetComponent<RotateX_Slow>().rotationCount;
        appearClockTime_slow = clockcenter_slow.GetComponent<RotateX_Slow>().currentRotationX;
        b = lRunner.blue;
        y = lRunner.yellow;
        r = lRunner.red;
        ar = lRunner.Whithin.GetComponent<within>().Attend_Red;
        ab = lRunner.Whithin.GetComponent<within>().Attend_Blue;
        rf = lRunner.Whithin.GetComponent<within>().RedFast;
        bf = lRunner.Whithin.GetComponent<within>().BlueFast;

        head = lRunner.Head;
        eyeTracker = head.GetComponent<EyeTrackingMAT_v2>();
    }


    // PREMETHOD ////////////////////////////////////////////////////////////////////////////////////////////////////
    protected override void PreMethod()
    {
         t = (int)Data["Trial"];
         Debug.Log(t);
    }
    // PRECOROUTINE ////////////////////////////////////////////////////////////////////////////////////////////////////

    protected override IEnumerator PreCoroutine() 
    {
        yield return null; //required for coroutine
    }


    // MAINCOROUNTINE ////////////////////////////////////////////////////////////////////////////////////////////////
    protected override IEnumerator RunMainCoroutine()
    {
     if (t == 131)
        {
            lRunner.StartCoroutine(WaitingResponse(pause));
        }
        if (t == 257)
        {
            lRunner.StartCoroutine(WaitingResponse(beforeEnd));
        }
        if (rf)
        {
            Debug.Log("ROSSO VELOOOOOOCE");
            fast = r;
            slow = b;
            lancetta1.GetComponent<Renderer>().material = fast;
            lancetta2.GetComponent<Renderer>().material = slow;
        }

        if (bf)
        {
            Debug.Log("BLU VELOOOOOOCE");
            fast = r;
            slow = b;
            lancetta1.GetComponent<Renderer>().material = slow;
            lancetta2.GetComponent<Renderer>().material = fast;
        }

        
        trialTimeStartms = EyeTrackingMAT_v2.time_stamp;
        Debug.Log(pract1.activeInHierarchy);
        if (pract1.activeInHierarchy == false && pract2.activeInHierarchy == false &&
            expInst.activeInHierarchy == false && expInstB.activeInHierarchy == false && 
            pract1B.activeInHierarchy == false && pract3B.activeInHierarchy == false && 
            pract3R.activeInHierarchy == false && pause.activeInHierarchy == false)
        {
            //clockcenter.transform.rotation.x = Random.Range(-180f, 180f);
            clock.SetActive(true);
            appearClockTime = clockcenter.GetComponent<RotateX>().currentRotationX;
            appearClockTime_slow = clockcenter_slow.GetComponent<RotateX_Slow>().currentRotationX;
            appearTimeUnity = EyeTrackingMAT_v2.time_stamp;


        }

        

    
        // You might want to do a while-loop to wait for participant response: 
        bool waitingForParticipantResponse = true;
        Debug.Log("Press the spacebar to end this trial.");
        
        
        while (waitingForParticipantResponse) 
        {   // keep check each frame until waitingForParticipantResponse set to false.
            countValue = clockcenter.GetComponent<RotateX>().rotationCount;
            
            if (Input.GetKeyDown(KeyCode.Space)) 
            { 
                pressTime = clockcenter.GetComponent<RotateX>().currentRotationX;
                pressTime_Slow = clockcenter_slow.GetComponent<RotateX_Slow>().currentRotationX;
                pressTimeUnity = /*appearTimeUnity -*/ Time.fixedTime;

                pressMs = EyeTrackingMAT_v2.time_stamp;
                waitingForParticipantResponse = false;  // escape from while loop
                if (trialcomplete != "wrong")
                {
                    trialcomplete = "true";

                }
            }
            

            if ( t == 0 && t == 7 && t == 12 && t == 131  && t == 132 && t == 138  && t == 257)
            {
                clock.SetActive(false);
                waitingForParticipantResponse = false;  // escape from while loop
                trialcomplete = "false";
            }

            yield return null; // wait for next frame while allowing rest of program to run (without this the program will hang in an infinite loop)
        }
    
    }


    // Optional Post-Trial code. Useful for waiting for the participant to do something after each trial (multiple frames)
    protected override IEnumerator PostCoroutine()
    {
        float r = Random.Range(0.5f, 0.8f);
        length = r;
        yield return new WaitForSeconds(r);
        disappearClockTime = clockcenter.GetComponent<RotateX>().currentRotationX;
        //disappearClockTime_Slow = clockcenter_slow.GetComponent<RotateX_Slow>().currentRotationX;
        disappearTimeUnity = EyeTrackingMAT_v2.time_stamp;
        clock.SetActive(false);
        fixationcross.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        fixationcross.SetActive(false);
        
        // Enable the clock object
        if (trialcomplete == "true" && (t != 0 && t != 6 && t != 12 && t != 131 && t != 132 && t != 138 && t != 257))
        {
            clock.SetActive(true);
            clockcenter_slow.SetActive(false);
            lancetta1.GetComponent<Renderer>().material = y;
            appearChoice = clockcenter.GetComponent<RotateX>().currentRotationX;
            appearChoiceUnity = EyeTrackingMAT_v2.time_stamp;
            // Disable the script component
            rotationx.enabled = false;
            //positionx.enabled = true;
            bool waitingForParticipantResponse = true;

            Transform clockcenterTransform = clockcenter.transform;

            float rotationSpeed = 175f;
            float accumulatedRotation = 0f;
            float currentRotation = clockcenterTransform.localRotation.eulerAngles.x;
            currentRotation = Random.Range(0f,360f);
           // clockcenterTransform.localRotation.

            while (waitingForParticipantResponse)
            {
           
                float rotationInput = Input.GetAxis("Mouse ScrollWheel");
                currentRotation += rotationInput * rotationSpeed*30 * Time.deltaTime;
                currentRotation = (currentRotation + 360f) % 360f;

                Quaternion rotationDelta = Quaternion.Euler(currentRotation, 0, -90);
                clockcenterTransform.localRotation = rotationDelta;
                
                timechoice = currentRotation;
                decisionTimeUnity = EyeTrackingMAT_v2.time_stamp;

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    clock.SetActive(false); //PROVA! *** seems to fix the extra random position frame
                    rotationx.enabled = true;
                    waitingForParticipantResponse = false; // escape from while loop
                }

                yield return null; // wait for next frame while allowing rest of program to run (without this the program will hang in an infinite loop)
            }
        }
        else if (trialcomplete != "true" && (t > 12))
        {
            clockcenter_slow.SetActive(false);
            clock.SetActive(true);
            appearChoice = clockcenter.GetComponent<RotateX>().currentRotationX;
            appearChoiceUnity = EyeTrackingMAT_v2.time_stamp;
            // Disable the script component
            rotationx.enabled = false;
            //positionx.enabled = true;
            bool waitingForParticipantResponse = true;

            Transform clockcenterTransform = clockcenter.transform;

            float rotationSpeed = 175f;
            float accumulatedRotation = 0f;
            float currentRotation = clockcenterTransform.localRotation.eulerAngles.x;
            currentRotation = Random.Range(0f,360f);
           // clockcenterTransform.localRotation.

            while (waitingForParticipantResponse)
            {
                // keep check each frame until waitingForParticipantResponse set to false.

                // Check for input to modify the rotation
                float rotationInput = Input.GetAxis("Mouse ScrollWheel");

                // Modify the rotation based on input
                currentRotation += rotationInput * rotationSpeed*30 * Time.deltaTime;

                // Ensure the x-rotation stays within a full 360-degree range
                currentRotation = (currentRotation + 360f) % 360f;

                Quaternion rotationDelta = Quaternion.Euler(currentRotation, 0, -90);

                clockcenterTransform.localRotation = rotationDelta;
                timechoice = currentRotation;
                decisionTimeUnity = EyeTrackingMAT_v2.time_stamp;

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    // check return key pressed
                    clock.SetActive(false); //PROVA! *** seems to fix the extra random position frame
                    //positionx.enabled = false;
                    rotationx.enabled = true;
                    waitingForParticipantResponse = false; // escape from while loop
                }

                yield return
                    null; // wait for next frame while allowing rest of program to run (without this the program will hang in an infinite loop)
            }
        }
        else if (trialcomplete == "false" && t<12 && (t != 0 && t != 6 && t != 12 && t != 131 && t != 132 && t != 138 && t != 257))
        {
            bool waitingForParticipantResponse = true;
            Debug.Log("Press the spacebar to end this trial.");
            plsFaster.SetActive(true);
            while (waitingForParticipantResponse)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    plsFaster.SetActive(false);
                    waitingForParticipantResponse = false;
                }

                yield return null;
            }
        }
        else if (trialcomplete == "wrong" && t<12 && (t != 0 && t != 6 && t != 12 && t != 131 && t != 132 && t != 138 && t != 257))
        {
            bool waitingForParticipantResponse = true;
            Debug.Log("Press the spacebar to end this trial.");
            plsSlower.SetActive(true);
            while (waitingForParticipantResponse)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    plsSlower.SetActive(false);
                    waitingForParticipantResponse = false;
                }

                yield return null;
            }
        }
        else if ( t == 0 || t == 6 || t == 12 || t == 131  || t == 132 || t == 138  || t == 257)
        {
            clock.SetActive(false);
            yield return new WaitForSeconds(0.5f);
        }

        clock.SetActive(false);
        clockcenter_slow.SetActive(true);
        fixationcross.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        fixationcross.SetActive(false);
        
    }


    // Optional Post-Trial code. useful for writing data to dependent variables and for resetting everything.
    // Executes in a single frame at the end of each trial
    protected override void PostMethod() 
    {
        clock.SetActive(false);
        // How to write results to dependent variables: 
         Data["W"] = timechoice;
         Data["complete"] = trialcomplete;
         Data["press_time"] = pressTime;
         Data["start_clock_position"] = appearClockTime;
         Data["final_clock_position"] = disappearClockTime;
         //Data["final_clock_position_slow"] = disappearClockTime_Slow;
         Data["random_start_report_position"] = appearChoice;
         Data["decision_time_unity"] = decisionTimeUnity;
         Data["press_time_unity"] = pressTimeUnity;
         Data["disppear_time_unity"] = disappearTimeUnity;
         Data["appear_time_unity"] = appearTimeUnity;
         Data["appear_choice_unity"] = appearChoiceUnity;
         Data["press_Ms"] = pressMs;
         Data["trial_Time_Start_MS"] = trialTimeStartms;
         Data["continuation_length"] = length;
         
        
    }
    protected IEnumerator WaitingResponse(GameObject targetObject)
    {
        targetObject.SetActive(true);
        bool waitingForParticipantResponse = true;
        Debug.Log("Press the spacebar to end this trial.");

        while (waitingForParticipantResponse)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                targetObject.SetActive(false);
                waitingForParticipantResponse = false;
            }
            yield return null;
        }
    }
    
}

