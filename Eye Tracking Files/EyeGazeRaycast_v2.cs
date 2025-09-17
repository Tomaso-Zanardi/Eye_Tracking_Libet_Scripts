using UnityEngine;
using ViveSR.anipal.Eye;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;
using System.IO;
using UnityEngine.XR;

public class EyeGazeRaycast_v2 : MonoBehaviour
{
    public static EyeData_v2 eyeData = new EyeData_v2();
    public static float pupil_diameter_L, pupil_diameter_R;
    public static Vector2 pupil_position_L, pupil_position_R;
    public static bool eye_callback_registered = false;
    public static Vector3? CurrentGazeHitPoint { get; private set; } = null;

    public float rayLength = 100f;
    public Color rayColor = Color.red;

    private LineRenderer lineRenderer;
    private GameObject hitObject; // The object that was hit by the gaze.
    private string csvFilePath = "GazeData.csv";
    private bool fileInitialized = false;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();

        if (lineRenderer == null)
        {
            Debug.LogError("No LineRenderer component found on this game object. Please add one.");
            return;
        }

        lineRenderer.startColor = rayColor;
        lineRenderer.endColor = rayColor;
        lineRenderer.positionCount = 2;

        //InitializeCSVFile();
    }

    private void Update()
    {
        //PerformEyeGazeRaycast();
        UpdateEyeTrackingData();
        RetrievePupilData();
        //WriteDataToCSV();
    }

    private void PerformEyeGazeRaycast()
    {
        Ray gazeRay;

        if (SRanipal_Eye_v2.GetGazeRay(GazeIndex.COMBINE, out gazeRay))
        {
            gazeRay.origin = transform.position;
            RaycastHit hitInfo;

            lineRenderer.positionCount = 2;

            if (Physics.Raycast(gazeRay, out hitInfo))
            {
                CurrentGazeHitPoint = hitInfo.point;
                hitObject = hitInfo.collider.gameObject;

                if (Regex.IsMatch(hitObject.name, @"Cube \(\d+\)"))
                {
                    hitObject.SetActive(false);
                    StartCoroutine(ActivateAfterDelay(hitObject, 2.0f));
                }

                Debug.Log("Gazed at: " + hitObject.name + " at position: " +
                          "X: " + hitInfo.point.x.ToString("F4") +
                          ", Y: " + hitInfo.point.y.ToString("F4") +
                          ", Z: " + hitInfo.point.z.ToString("F4"));

                lineRenderer.SetPosition(0, gazeRay.origin);
                lineRenderer.SetPosition(1, hitInfo.point);
            }
            else
            {
                CurrentGazeHitPoint = null;
                lineRenderer.SetPosition(0, gazeRay.origin);
                lineRenderer.SetPosition(1, gazeRay.origin + gazeRay.direction * rayLength);
            }
        }
        else
        {
            lineRenderer.positionCount = 0;
        }
    }

    private IEnumerator ActivateAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        obj.SetActive(true);
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
        }
    }

    private void InitializeCSVFile()
    {
        if (!fileInitialized)
        {
            using (StreamWriter sw = new StreamWriter(csvFilePath, false))
            {
                sw.WriteLine("Time,Frame,HitObjectName,HitPointX,HitPointY,HitPointZ,PupilDiameterLeft,PupilDiameterRight");
            }
            fileInitialized = true;
        }
    }

    private void WriteDataToCSV()
    {
        using (StreamWriter sw = new StreamWriter(csvFilePath, true))
        {
            string hitObjectName = CurrentGazeHitPoint.HasValue ? hitObject.name : "None";
            Vector3 hitPoint = CurrentGazeHitPoint.HasValue ? hitObject.transform.position : Vector3.zero;

            string line = string.Format("{0},{1},{2},{3:F4},{4:F4},{5:F4},{6:F4},{7:F4}",
                Time.time,
                Time.frameCount,
                hitObjectName,
                hitPoint.x,
                hitPoint.y,
                hitPoint.z,
                pupil_diameter_L,
                pupil_diameter_R);

            sw.WriteLine(line);
        }
    }
}
