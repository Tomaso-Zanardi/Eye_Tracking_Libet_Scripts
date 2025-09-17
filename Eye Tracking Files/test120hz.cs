using UnityEngine;
using ViveSR.anipal.Eye;
using System.Runtime.InteropServices;
using UnityEngine.UI;

using System;
using System.IO;


/// <summary>
/// Example usage for eye tracking callback
/// Note: Callback runs on a separate thread to report at ~120hz.
/// Unity is not threadsafe and cannot call any UnityEngine api from within callback thread.
/// </summary>
public class test120hz : MonoBehaviour
{
    public int numb;
    public static string file_path = Directory.GetCurrentDirectory(); 
    public static int file_number = 0;                                      // Enumarate file (avoid overlapping).    
    public static string file_name = "Eye_Data"; 
    private static EyeData eyeData = new EyeData();
    private static bool eye_callback_registered = false;
    public static float time_unity;                                         // Unity time since game is trigger in seconds.
    public static float time_stamp;                                         // The time when the frame was capturing in millisecond.
    public static int frame;
    public static Vector3 gaze_direct_C;

    public Text uiText;
    private float updateSpeed = 0;
    private static float lastTime, currentTime;
    public static float   pupil_diameter_L, pupil_diameter_R;               // Diameter of pupil dilation in mm.
    
    public static bool start_printing = false;                              // Bool to able/disable printing data.

    
    public static Vector3 gaze_origin_L_world, gaze_origin_R_world;         // gaze origin traslated with head position (eyes position in the head).
    public static Vector3 gaze_direct_L_world, gaze_direct_R_world;         // gaze direction traslated with effective eyes position. 

    public static Vector3 ray;

    void Update()
    {
        if (SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.WORKING) return;


        if (SRanipal_Eye_Framework.Instance.EnableEyeDataCallback == true && eye_callback_registered == false)
        {
            SRanipal_Eye.WrapperRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye.CallbackBasic)EyeCallback));
            eye_callback_registered = true;
        }
        else if (SRanipal_Eye_Framework.Instance.EnableEyeDataCallback == false && eye_callback_registered == true)
        {
            SRanipal_Eye.WrapperUnRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye.CallbackBasic)EyeCallback));
            eye_callback_registered = false;
        }
        time_unity = Time.fixedTime;
        updateSpeed = currentTime - lastTime;
        uiText.text = updateSpeed.ToString() + " ms";
        //ray = FocusInfo.point;
        //Debug.Log("L: " + pupil_diameter_L+ " R: "+ pupil_diameter_R);
        if (Input.GetKeyDown(KeyCode.S))
        {
            StartDataRecord();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StopDataRecord();
        }
    }

    private void OnDisable()
    {
        Release();
    }

    void OnApplicationQuit()
    {
        Release();
    }

    /// <summary>
    /// Release callback thread when disabled or quit
    /// </summary>
    private static void Release()
    {
        if (eye_callback_registered == true)
        {
            SRanipal_Eye.WrapperUnRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye.CallbackBasic)EyeCallback));
            eye_callback_registered = false;
        }
    }

    /// <summary>
    /// Required class for IL2CPP scripting backend support
    /// </summary>
    internal class MonoPInvokeCallbackAttribute : System.Attribute
    {
        public MonoPInvokeCallbackAttribute() { }
    }

    /// <summary>
    /// Eye tracking data callback thread.
    /// Reports data at ~120hz
    /// MonoPInvokeCallback attribute required for IL2CPP scripting backend
    /// </summary>
    /// <param name="eye_data">Reference to latest eye_data</param>
    [MonoPInvokeCallback]
    private static void EyeCallback(ref EyeData eye_data)
    {
        eyeData = eye_data;
        // do stuff with eyeData...

        lastTime = currentTime;
        currentTime = eyeData.timestamp;
        //  Measure eye movements at the frequency of 120Hz.
        ViveSR.Error error = SRanipal_Eye_API.GetEyeData(ref eyeData);
        if (error == ViveSR.Error.WORK)
        {
            //time_unity = Time.fixedTime;
            time_stamp = eyeData.timestamp;
            frame = eyeData.frame_sequence;
            pupil_diameter_L = eyeData.verbose_data.left.pupil_diameter_mm;
            pupil_diameter_R = eyeData.verbose_data.right.pupil_diameter_mm;
            gaze_direct_C = eyeData.verbose_data.combined.eye_data.gaze_direction_normalized;
            if (start_printing == true)
            {
                string value =
                    time_unity.ToString() + "\t" +
                    time_stamp.ToString() + "\t" +
                    frame.ToString() + "\t" +
                    pupil_diameter_L.ToString() + "\t" +
                    pupil_diameter_R.ToString() + "\t" +
                    gaze_direct_C.x.ToString() + "\t" +
                    gaze_direct_C.y.ToString() + "\t" +
                    gaze_direct_C.z.ToString() + "\t" +

                    Environment.NewLine;
                File.AppendAllText(file_name + file_number + ".txt", value);

            }
        }
    }
    
    public static void Data_txt(string filePath)
        {
            string variable =
                "time_unity(ms)" + "\t" +
                "time_stamp(ms)" + "\t" +
                "frame" + "\t" +
                "pupil_diameter_L(mm)" + "\t" +
                "pupil_diameter_R(mm)" + "\t" +
                "gaze_direct_C.x" + "\t" +
                "gaze_direct_C.y" + "\t" +
                "gaze_direct_C.z" + "\t" +
                
                Environment.NewLine;
                File.AppendAllText(file_name + file_number + ".txt", variable);
        }
    
    // ********************************************************************************************************************
    //  Start printing data. 
    // ********************************************************************************************************************
    public static void StartDataRecord()
    {
        Debug.Log("START PRINT");
        start_printing = true;
        file_number += 1;
        Data_txt(file_path);
    }


    // ********************************************************************************************************************
    //  Stop printing data. 
    // ********************************************************************************************************************
    public static void StopDataRecord()
    {
        Debug.Log("STOP PRINT");
        start_printing = false;
    }


    // ********************************************************************************************************************
    //  Start eyes calibration.
    // ********************************************************************************************************************
    public static void EyesCalibration()
    {
        Debug.Log("LAUNCH CALIBRATION");
        SRanipal_Eye_v2.LaunchEyeCalibration();
    }
}