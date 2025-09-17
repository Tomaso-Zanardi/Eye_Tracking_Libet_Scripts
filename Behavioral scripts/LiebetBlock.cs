using System.Collections;
using System.Data;
using bmlTUX;
using UnityEngine;

public class LiebetBlock : Block 
{
    public GameObject clock;

    public GameObject pract1;
    public GameObject pract2;

    public GameObject pract1B;
    public GameObject pract2B;
    public GameObject pract3R;
    public GameObject pract3B;
    public GameObject pause;

    public GameObject expInst,expInstB;
    // // You usually want to store a reference to your Experiment runner
    // YourCustomExperimentRunner myRunner;
    public LiebetRunner lRunner;
    public Behaviour condition;

    public bool attend_Red, attend_Blue, Red_First, Blue_First;
    public bool c;



    // Required Constructor. Good place to set up references to objects in the unity scene
    public LiebetBlock(ExperimentRunner runner, DataTable trialTable, DataRow data, int index) : base(runner, trialTable, data, index) 
    {
        lRunner = (LiebetRunner)runner;

        condition = lRunner.Whithin.GetComponent<within>();
        Red_First = lRunner.Whithin.GetComponent<within>().RedFast;
        Blue_First = lRunner.Whithin.GetComponent<within>().BlueFast;
        attend_Blue = lRunner.Whithin.GetComponent<within>().Attend_Blue;
        attend_Red = lRunner.Whithin.GetComponent<within>().Attend_Red;
        
        clock = lRunner.Clock;
        pract1 = lRunner.Practice1;
        pract2 = lRunner.Practice2;

        pract1B = lRunner.Practice1AB;
        pract2B = lRunner.Practice2AB;
        pract3R = lRunner.Practice3R;
        pract3B = lRunner.Practice3B;
        pause = lRunner.PauseScreen;
            
        expInst = lRunner.ExperInstructions;
        expInstB = lRunner.ExperInstructionsB;
        // myRunner = (YourCustomExperimentRunner)runner;  //cast the generic runner to your custom type.
        // GameObject myGameObject = myRunner.MyGameObject;  // get reference to gameObject stored in your custom runner

    }


    // Optional Pre-Block code. Useful for calibration and setup common to all blocks. Executes in a single frame at the start of the block
    protected override void PreMethod()
    {

        // float thisBlocksDistanceValue = (float)Data["MyDistanceFloatVariableName"]; // Read values of independent variables
        // myGameObject.transform.position = new Vector3(thisBlocksDistanceValue, 0, 0); // set up scene based on value
        string blockstring = (string)Data["type"];

        if (attend_Red)
        {
            if (blockstring == "practice1")
            {
                //pract1.SetActive(true);
                lRunner.StartCoroutine(WaitingResponse(pract1));
            }
        }


        if (attend_Blue)
        {
            if (blockstring == "practice1")
            {
                //pract1.SetActive(true);
                lRunner.StartCoroutine(WaitingResponse(pract1B));
            }
        }
        
    }

    // Optional Pre-Block code spanning multiple frames. Useful for pre-Block instructions.
    // Can execute over multiple frames at the start of a block
    protected override IEnumerator PreCoroutine()
    {
        string blockstring = (string)Data["type"];
 
        
        if (blockstring == "practice2")
        {
            lRunner.StartCoroutine(WaitingResponse(pract2));
        }

        if (attend_Red)
        {
            if (blockstring == "L1")
            {
                lRunner.StartCoroutine(WaitingResponse(expInst));
            }
            if (blockstring == "L21")
            {
                lRunner.StartCoroutine(WaitingResponse(expInstB));
            }
            if (blockstring == "practice3")
            {
                lRunner.StartCoroutine(WaitingResponse(pract3B));
            }

        }

        if (attend_Blue)
        {
            if (blockstring == "L1")
            {
                lRunner.StartCoroutine(WaitingResponse(expInstB));
            }
            if (blockstring == "L21")
            {
                lRunner.StartCoroutine(WaitingResponse(expInst));
            }
            if (blockstring == "practice3")
            {
                lRunner.StartCoroutine(WaitingResponse(pract3R));
            }

        }






        yield return null; // yield return required for coroutine. Waits until next frame
        
        // Other ideas:
        // yield return new WaitForSeconds(5);     Waits for 5 seconds worth of frames;
        // can also wait for user input in a while-loop with a yield return null inside.
    }


    // Optional Post-Block code spanning multiple frames. Useful for Block debrief instructions.
    protected override IEnumerator PostCoroutine() {
        yield return null; //required for coroutine
    }


    // Optional Post-Block code.
    protected override void PostMethod() {
        // cleanup code (happens all in one frame at end of block)
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

