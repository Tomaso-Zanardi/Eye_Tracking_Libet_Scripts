using UnityEngine;
using ViveSR.anipal.Eye; // Importing the namespace for the Vive Eye Tracker SDK.
using System.Text.RegularExpressions; // For using regular expressions.
using System.Collections; // For using coroutines.

using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;
using System.IO;
using UnityEngine.XR;
public class EyeGazeRaycast : MonoBehaviour // Defining a new class EyeGazeRaycast, inheriting from MonoBehaviour.
{
    //DA MATTIA************************************************************************************************
    public static EyeData_v2 eyeData = new EyeData_v2();
    public static float   pupil_diameter_L, pupil_diameter_R;               // Diameter of pupil dilation in mm.
    public static Vector2 pupil_position_L, pupil_position_R;
    public static bool eye_callback_registered = false;

    //*********************************************************************************************************
    public static Vector3? CurrentGazeHitPoint { get; private set; } = null; // A nullable static property to store the current gaze hit point.
    public float rayLength = 100f; // Public variable to set the length of the ray.
    public Color rayColor = Color.red; // Public variable to set the color of the ray.

    private LineRenderer lineRenderer; // Private variable to hold a reference to the LineRenderer component.

    private void Awake() // Awake is called when the script instance is being loaded.
    {
        lineRenderer = GetComponent<LineRenderer>(); // Getting the LineRenderer component attached to this GameObject.

        if (lineRenderer == null) // Checking if the LineRenderer component is not found.
        {
            Debug.LogError("No LineRenderer component found on this game object. Please add one."); // Logging an error message.
            return; // Exiting the method.
        }

        lineRenderer.startColor = rayColor; // Setting the start color of the line.
        lineRenderer.endColor = rayColor; // Setting the end color of the line.
        lineRenderer.positionCount = 2; // Setting the number of positions in the LineRenderer to 2 (start and end points).
    }

    private void Update() // Update is called once per frame.
    {
        PerformEyeGazeRaycast(); // Calling the method to perform the eye gaze raycast.
        //Measurement();
        //UpdateEyeTrackingData();
        //RetrievePupilData();

    }

    private void PerformEyeGazeRaycast() // Method to perform the eye gaze raycast.
    {
        Ray gazeRay; // Declaring a Ray variable to store the gaze direction.

        // Obtaining the combined gaze direction for both eyes and storing it in gazeRay.
        if (SRanipal_Eye_v2.GetGazeRay(GazeIndex.COMBINE, out gazeRay))
        {
            gazeRay.origin = transform.position; // Setting the origin of the gazeRay to the current GameObject's position.

            RaycastHit hitInfo; // Declaring a RaycastHit variable to store information about what the ray hits.

            lineRenderer.positionCount = 2; // Ensuring the LineRenderer has two positions.

            // Performing a raycast in the direction of gazeRay.
            if (Physics.Raycast(gazeRay, out hitInfo))
            {
                CurrentGazeHitPoint = hitInfo.point; // Updating the current gaze hit point with the point of collision.
                GameObject hitObject = hitInfo.collider.gameObject; // Getting the GameObject that was hit.

                // Checking if the hit object's name matches a specific pattern.
                if (Regex.IsMatch(hitObject.name, @"Cube \(\d+\)"))
                {
                    hitObject.SetActive(false); // Disabling the hit GameObject.
                    StartCoroutine(ActivateAfterDelay(hitObject, 2.0f)); // Starting a coroutine to reactivate the object after a delay.
                }

                // Logging the name and position of the hit object.
                // Logging the name and position of the hit object with coordinates formatted to 4 decimal places.
                Debug.Log("Gazed at: " + hitObject.name + " at position: " + 
                          "X: " + hitInfo.point.x.ToString("F4") + 
                          ", Y: " + hitInfo.point.y.ToString("F4") + 
                          ", Z: " + hitInfo.point.z.ToString("F4"));


                // Setting the start and end positions of the LineRenderer to visualize the ray.
                lineRenderer.SetPosition(0, gazeRay.origin);
                lineRenderer.SetPosition(1, hitInfo.point);
            }
            else
            {
                CurrentGazeHitPoint = null; // Resetting the current gaze hit point if nothing was hit.
                // Drawing the ray for the specified length if no collision is detected.
                lineRenderer.SetPosition(0, gazeRay.origin);
                lineRenderer.SetPosition(1, gazeRay.origin + gazeRay.direction * rayLength);
            }
        }
        else
        {
            // Hiding the LineRenderer if no valid gaze data is obtained.
            lineRenderer.positionCount = 0;
        }
    }

    private IEnumerator ActivateAfterDelay(GameObject obj, float delay) // Coroutine to activate a GameObject after a specified delay.
    {
        yield return new WaitForSeconds(delay); // Waiting for the specified amount of time.
        obj.SetActive(true); // Reactivating the GameObject.
    }
    
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
        Debug.Log("Pupil diameter LEFT: " + pupil_diameter_L + "Pupil diameter RIGHT: " + pupil_diameter_R);
    }
    


     public static void EyeCallback(ref EyeData_v2 eye_data)
        {
            // Gets data from anipal's Eye module.
            eyeData = eye_data;

            //  Measure eye movements at the frequency of 120Hz.
            ViveSR.Error error = SRanipal_Eye_API.GetEyeData_v2(ref eyeData);
            if (error == ViveSR.Error.WORK)
            {

                pupil_diameter_L = eyeData.verbose_data.left.pupil_diameter_mm;
                pupil_diameter_R = eyeData.verbose_data.right.pupil_diameter_mm;

                pupil_position_L = eyeData.verbose_data.left.pupil_position_in_sensor_area;
                pupil_position_R = eyeData.verbose_data.right.pupil_position_in_sensor_area;



                //  Print data in txt file if button "s" is pressed. 
                /*if (start_printing == true)
                {
                    string value =
                        

                        pupil_diameter_L.ToString() + "\t" +
                        pupil_diameter_R.ToString() + "\t" +

                        pupil_position_L.x.ToString() + "\t" +
                        pupil_position_L.y.ToString() + "\t" +
                        pupil_position_R.x.ToString() + "\t" +
                        pupil_position_R.y.ToString() + "\t" +

                       
                    Environment.NewLine;
                    File.AppendAllText(file_name + file_number + ".txt", value);
                }
            */}
        }
     
     private void UpdateEyeTrackingData()
     {
         if (SRanipal_Eye_Framework.Status == SRanipal_Eye_Framework.FrameworkStatus.WORKING)
         {
             SRanipal_Eye_API.GetEyeData_v2(ref eyeData);
         }
     }

     private void RetrievePupilData()
     {
         if (SRanipal_Eye_Framework.Status == SRanipal_Eye_Framework.FrameworkStatus.WORKING)
         {
             pupil_diameter_L = eyeData.verbose_data.left.pupil_diameter_mm;
             pupil_diameter_R = eyeData.verbose_data.right.pupil_diameter_mm;

             Debug.Log("Pupil diameter LEFT: " + pupil_diameter_L + ", Pupil diameter RIGHT: " + pupil_diameter_R);
         }
     }

}