using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.XR;
using ViveSR.anipal.Eye;

namespace Realter
{
    public class EyeTracking: MonoBehaviour
    {
        // *******************************************************************************************************************
        //  Settle file txt information. 
        // *******************************************************************************************************************
        public static string file_path = Directory.GetCurrentDirectory();       // Directory in which put th file. 
        public static int file_number = 0;                                      // Enumarate file (avoid overlapping).    
        public static string file_name = "Data";                         // Name of the file. 


        // ********************************************************************************************************************
        //  Parameters for the head and eyes thorugh XR.Nodes.
        // ********************************************************************************************************************
        public static GameObject XR_head;                                       // User's virtual head. 
        public static Vector3 head_position;                                    // Head position in HMD. 
        public static Quaternion head_rotation;
        

        // ********************************************************************************************************************
        //  Parameters for eyes through SRanipal. 
        // ********************************************************************************************************************
        public static EyeData_v2 eyeData = new EyeData_v2();
        public static bool eye_callback_registered = false;
        public static bool start_printing = false;                              // Bool to able/disable printing data.

        public static float time_unity;                                         // Unity time since game is trigger in seconds.
        public static float time_stamp;                                         // The time when the frame was capturing in millisecond.
        public static int frame;                                                // The number of the frames.

        public static UInt64  eye_valid_L, eye_valid_R;                         // The bits explaining the validity of eye data.
        public static float   openness_L, openness_R;                           // A value representing how open the eye is in [0,1].
        public static float   pupil_diameter_L, pupil_diameter_R;               // Diameter of pupil dilation in mm.
        public static Vector2 pupil_position_L, pupil_position_R;               // The normalized position of a pupil in [0,1].
        public static Vector3 gaze_origin_L, gaze_origin_R, gaze_origin_C;      // The point in the eye from which the gaze ray originates in meter miles.(right-handed coordinate system).
        public static Vector3 gaze_direct_L, gaze_direct_R, gaze_direct_C;      // The normalized gaze direction of the eye in [0,1].(right-handed coordinate system).
 

        // ********************************************************************************************************************
        //  Parameters for gaze-contingency 
        // ********************************************************************************************************************
        public static GameObject XR_left_eye, XR_right_eye;                     // User's virtual eyes. 
        public static Vector3 gaze_origin_L_meters, gaze_origin_R_meters;       // gaze origin in meter from GetGazeRay (local space).
        public static Vector3 gaze_origin_L_world, gaze_origin_R_world;         // gaze origin traslated with head position (eyes position in the head).
        public static Vector3 gaze_direct_L_world, gaze_direct_R_world;         // gaze direction traslated with effective eyes position. 
        public static Vector3 gaze_contingency_L, gaze_contingency_R;           // Parameters for gaze contingnecy (fixations point). 
        
        
        // ********************************************************************************************************************
        //  Parameters for the hands through XR.Node
        // ********************************************************************************************************************         
        public static GameObject XR_left_hand, XR_right_hand;

        public static Vector3 left_hand_position, right_hand_position;
        public static Quaternion left_hand_rotation, right_hand_rotation;


        public void Awake()
        {
            SetNodes();
        }

        public void Start()
        {
            Measurement();
        }

        public void Update()
        {
            Tracking();
        }

        public void LateUpdate()
        {
            SetHeadEyesHandsPosition();
        }


        // ********************************************************************************************************************
        //  Set cameras infromation
        // ********************************************************************************************************************
        public static void SetNodes()
        {
            XR_head         =  GameObject.Find("XR Head");                  // User's virtual head.  
            XR_left_eye     =  GameObject.Find("XR Left Eye");              // User's virtual left eye.
            XR_right_eye    =  GameObject.Find("XR Right Eye");             // User's virtual right eye.
            XR_left_hand    =  GameObject.Find("Controller (left)");        // User's virtual left hand through Vive Controller. 
            XR_right_hand   =  GameObject.Find("Controller (right)");       // User's virtual right hand through Vive Controller. 
        }


        // ********************************************************************************************************************
        //  Tracking head, eyes, and hands movement through XR.Nodes. 
        // ********************************************************************************************************************
        public static void Tracking()
        {
            //  Tracking Unity editor time.
            time_unity = Time.fixedTime;


            // Tracking head movements from XR.Node that represent head center. 
            head_position       = InputTracking.GetLocalPosition(XRNode.CenterEye);     // Center Eye is the standard/correct node. 
            head_rotation       = InputTracking.GetLocalRotation(XRNode.CenterEye);


                                            // GAZE CONTINGENT PARAMETERS
            // Define of the 2 gaze ray for each eye (look at Gaze Ray Sample 2 for further informations). 
            if (eye_callback_registered)
            {
                SRanipal_Eye_v2.GetGazeRay(GazeIndex.LEFT, out gaze_origin_L_meters, out gaze_direct_L, eyeData);
                SRanipal_Eye_v2.GetGazeRay(GazeIndex.RIGHT, out gaze_origin_R_meters, out gaze_direct_R, eyeData);
            }

            else
            {
                SRanipal_Eye_v2.GetGazeRay(GazeIndex.LEFT, out gaze_origin_L_meters, out gaze_direct_L);
                SRanipal_Eye_v2.GetGazeRay(GazeIndex.RIGHT, out gaze_origin_R_meters, out gaze_direct_R);
            }


            // Determine the effective position of user's eyes by traslating the gaze origin toward head position. 
            gaze_origin_L_world = XR_head.transform.TransformPoint(gaze_origin_L_meters);
            gaze_origin_R_world = XR_head.transform.TransformPoint(gaze_origin_R_meters);

            // Determine the effective direction of user's eyes by combining gaze direction from SRanipal and eyes position determine above. 
            gaze_direct_L_world = XR_left_eye.transform.TransformDirection(gaze_direct_L);
            gaze_direct_R_world = XR_right_eye.transform.TransformDirection(gaze_direct_R);

            //Definition of gaze-contingent parameters from each rays.
            gaze_contingency_L = (gaze_origin_L_world + gaze_direct_L_world);
            gaze_contingency_R = (gaze_origin_R_world + gaze_direct_R_world);

            // Tracking hands position from Valve.VR.
            left_hand_position = InputTracking.GetLocalPosition(XRNode.LeftHand);
            left_hand_rotation = InputTracking.GetLocalRotation(XRNode.LeftHand);
            right_hand_position = InputTracking.GetLocalPosition(XRNode.RightHand);
            right_hand_rotation = InputTracking.GetLocalRotation(XRNode.RightHand);
        }


        public static void SetHeadEyesHandsPosition()
        {
            // Regulate Head position according XR.Node.
            XR_head.transform.SetPositionAndRotation(head_position, head_rotation);

            // Regulate eyes position according gaze origin traslated to head position (world coordinates). 
            XR_left_eye.transform.SetPositionAndRotation(gaze_origin_L_world, head_rotation);
            XR_right_eye.transform.SetPositionAndRotation(gaze_origin_R_world, head_rotation); 
            
            // Regulate Hands position according XR.Node.
            XR_left_hand.transform.SetPositionAndRotation(left_hand_position, left_hand_rotation);
            XR_right_hand.transform.SetPositionAndRotation(right_hand_position, right_hand_rotation);
        }


        // ********************************************************************************************************************
        //  Measure eye movements in a callback function that HTC SRanipal provides.
        // ********************************************************************************************************************
        public static void Measurement()
        {
            EyeParameter eye_parameter = new EyeParameter();
            SRanipal_Eye_API.GetEyeParameter(ref eye_parameter);

            if (SRanipal_Eye_Framework.Instance.EnableEyeDataCallback == true && eye_callback_registered == false)
            {
                SRanipal_Eye_v2.WrapperRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
                eye_callback_registered = true;
            }

            else if (SRanipal_Eye_Framework.Instance.EnableEyeDataCallback == false && eye_callback_registered == true)
            {
                SRanipal_Eye_v2.WrapperUnRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
                eye_callback_registered = false;
            }
        }


        // ********************************************************************************************************************
        //  Callback function to record the eye movement data. It works with UnityEngine. 
        // ********************************************************************************************************************
        public static void EyeCallback(ref EyeData_v2 eye_data)
        {
            // Gets data from anipal's Eye module.
            eyeData = eye_data;

            //  Measure eye movements at the frequency of 120Hz.
            ViveSR.Error error = SRanipal_Eye_API.GetEyeData_v2(ref eyeData);
            if (error == ViveSR.Error.WORK)
            {

                time_stamp = eyeData.timestamp;
                frame = eyeData.frame_sequence;

                eye_valid_L = eyeData.verbose_data.left.eye_data_validata_bit_mask;
                eye_valid_R = eyeData.verbose_data.right.eye_data_validata_bit_mask;

                openness_L = eyeData.verbose_data.left.eye_openness;
                openness_R = eyeData.verbose_data.right.eye_openness;

                pupil_diameter_L = eyeData.verbose_data.left.pupil_diameter_mm;
                pupil_diameter_R = eyeData.verbose_data.right.pupil_diameter_mm;

                pupil_position_L = eyeData.verbose_data.left.pupil_position_in_sensor_area;
                pupil_position_R = eyeData.verbose_data.right.pupil_position_in_sensor_area;

                gaze_origin_L = eyeData.verbose_data.left.gaze_origin_mm;
                gaze_origin_R = eyeData.verbose_data.right.gaze_origin_mm;

                gaze_direct_L = eyeData.verbose_data.left.gaze_direction_normalized;
                gaze_direct_R = eyeData.verbose_data.right.gaze_direction_normalized;

                gaze_origin_C = eyeData.verbose_data.combined.eye_data.gaze_origin_mm;
                gaze_direct_C = eyeData.verbose_data.combined.eye_data.gaze_direction_normalized;

                gaze_origin_L.x *= -1;
                gaze_origin_R.x *= -1;
                gaze_direct_L.x *= -1;
                gaze_direct_R.x *= -1;


                //  Print data in txt file if button "s" is pressed. 
                if (start_printing == true)
                {
                    string value =

                        time_unity.ToString("F4") + "\t" +
                        time_stamp.ToString() + "\t" +
                        frame.ToString() + "\t" +

                        eye_valid_L.ToString() + "\t" +
                        eye_valid_R.ToString() + "\t" +

                        openness_L.ToString() + "\t" +
                        openness_R.ToString() + "\t" +

                        pupil_diameter_L.ToString() + "\t" +
                        pupil_diameter_R.ToString() + "\t" +

                        pupil_position_L.x.ToString() + "\t" +
                        pupil_position_L.y.ToString() + "\t" +
                        pupil_position_R.x.ToString() + "\t" +
                        pupil_position_R.y.ToString() + "\t" +

                        gaze_origin_L.x.ToString() + "\t" +
                        gaze_origin_L.y.ToString() + "\t" +
                        gaze_origin_L.z.ToString() + "\t" +
                        gaze_origin_R.x.ToString() + "\t" +
                        gaze_origin_R.y.ToString() + "\t" +
                        gaze_origin_R.z.ToString() + "\t" +
                        gaze_origin_C.x.ToString() + "\t" +
                        gaze_origin_C.y.ToString() + "\t" +
                        gaze_origin_C.z.ToString() + "\t" +

                        gaze_direct_L.x.ToString() + "\t" +
                        gaze_direct_L.y.ToString() + "\t" +
                        gaze_direct_L.z.ToString() + "\t" +
                        gaze_direct_R.x.ToString() + "\t" +
                        gaze_direct_R.y.ToString() + "\t" +
                        gaze_direct_R.z.ToString() + "\t" +
                        gaze_direct_C.x.ToString() + "\t" +
                        gaze_direct_C.y.ToString() + "\t" +
                        gaze_direct_C.z.ToString() + "\t" +


                        head_position.x.ToString() + "\t" +
                        head_position.y.ToString() + "\t" +
                        head_position.z.ToString() + "\t" +

                        head_rotation.x.ToString() + "\t" +
                        head_rotation.y.ToString() + "\t" +
                        head_rotation.z.ToString() + "\t" +
                        head_rotation.w.ToString() + "\t" +


                        gaze_origin_L_world.x.ToString() + "\t" +
                        gaze_origin_L_world.y.ToString() + "\t" +
                        gaze_origin_L_world.z.ToString() + "\t" +

                        gaze_origin_R_world.x.ToString() + "\t" +
                        gaze_origin_R_world.y.ToString() + "\t" +
                        gaze_origin_R_world.z.ToString() + "\t" +

                        gaze_direct_L_world.x.ToString() + "\t" +
                        gaze_direct_L_world.y.ToString() + "\t" +
                        gaze_direct_L_world.z.ToString() + "\t" +

                        gaze_direct_R_world.x.ToString() + "\t" +
                        gaze_direct_R_world.y.ToString() + "\t" +
                        gaze_direct_R_world.z.ToString() + "\t" +

                        gaze_contingency_L.x.ToString() + "\t" +
                        gaze_contingency_L.y.ToString() + "\t" +
                        gaze_contingency_L.z.ToString() + "\t" +

                        gaze_contingency_R.x.ToString() + "\t" +
                        gaze_contingency_R.y.ToString() + "\t" +
                        gaze_contingency_R.z.ToString() + "\t" +


                    Environment.NewLine;
                    File.AppendAllText(file_name + file_number + ".txt", value);
                }
            }
        }


        // ********************************************************************************************************************
        //  Create a text file with labels.  
        // ********************************************************************************************************************
        public static void Data_txt(string filePath)
        {
            string variable =
            "time_unity" + "\t" +
            "time_stamp(ms)" + "\t" +
            "frame" + "\t" +

            "eye_valid_L" + "\t" +
            "eye_valid_R" + "\t" +

            "openness_L" + "\t" +
            "openness_R" + "\t" +

            "pupil_diameter_L(mm)" + "\t" +
            "pupil_diameter_R(mm)" + "\t" +

            "pupil_position_L.x" + "\t" +
            "pupil_position_L.y" + "\t" +
            "pupil_position_R.x" + "\t" +
            "pupil_position_R.y" + "\t" +

            "gaze_origin_L.x(mm)" + "\t" +
            "gaze_origin_L.y(mm)" + "\t" +
            "gaze_origin_L.z(mm)" + "\t" +
            "gaze_origin_R.x(mm)" + "\t" +
            "gaze_origin_R.y(mm)" + "\t" +
            "gaze_origin_R.z(mm)" + "\t" +
            "gaze_origin_C.x(mm)" + "\t" +
            "gaze_origin_C.y(mm)" + "\t" +
            "gaze_origin_C.z(mm)" + "\t" +

            "gaze_direct_L.x" + "\t" +
            "gaze_direct_L.y" + "\t" +
            "gaze_direct_L.z" + "\t" +
            "gaze_direct_R.x" + "\t" +
            "gaze_direct_R.y" + "\t" +
            "gaze_direct_R.z" + "\t" +
            "gaze_direct_C.x" + "\t" +
            "gaze_direct_C.y" + "\t" +
            "gaze_direct_C.z" + "\t" +

            "head.position.x" + "\t" +
            "head.position.y" + "\t" +
            "head.position.z" + "\t" +

            "head.rotation.x" + "\t" +
            "head.rotation.y" + "\t" +
            "head.rotation.z" + "\t" +
            "head.rotation.w" + "\t" +

            "gaze_origin_world_L.x" + "\t" +
            "gaze_origin_world_L.y" + "\t" +
            "gaze_origin_world_L.z" + "\t" +

            "gaze_origin_world_R.x" + "\t" +
            "gaze_origin_world_R.y" + "\t" +
            "gaze_origin_world_R.z" + "\t" +

            "gaze_direction_world_L.x" + "\t" +
            "gaze_direction_world_L.y" + "\t" +
            "gaze_direction_world_L.z" + "\t" +

            "gaze_direction_world_R.x" + "\t" +
            "gaze_direction_world_R.y" + "\t" +
            "gaze_direction_world_R.z" + "\t" +

            "gaze_contingency_L.x" + "\t" +
            "gaze_contingency_L.y" + "\t" +
            "gaze_contingency_L.z" + "\t" +

            "gaze_contingency_R.x" + "\t" +
            "gaze_contingency_R.y" + "\t" +
            "gaze_contingency_R.z" + "\t" +

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
}