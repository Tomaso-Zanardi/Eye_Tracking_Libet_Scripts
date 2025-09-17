using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetInitialPosition : MonoBehaviour
{
    public GameObject targetObject; // Reference to the object representing the desired initial position
    public string vrCameraName = "Camera"; // Name of the VR camera object in the scene
    public KeyCode setInitialPositionKey = KeyCode.C; // Key to press to set the initial position

    private bool setInitialPosition = false;

    private void Update()
    {
        if (Input.GetKeyDown(setInitialPositionKey))
        {
            setInitialPosition = true;
        }
    }

    private void LateUpdate()
    {
        if (setInitialPosition)
        {
            // Get the VR camera object
            GameObject vrCamera = GameObject.Find(vrCameraName);

            // Calculate the offset between the VR camera position and the CameraRig position
            Vector3 cameraOffset = vrCamera.transform.position - transform.position;

            // Calculate the new position for the CameraRig
            Vector3 newRigPosition = targetObject.transform.position - cameraOffset;

            // Set the new position for the CameraRig
            transform.position = newRigPosition;

            // Calculate the direction from the camera to the origin
            Vector3 directionToOrigin = Vector3.zero - vrCamera.transform.position; // Assuming the origin is the target point
            directionToOrigin.y = 0; // Ensure the direction is flat relative to the ground plane
            //tentativo
            directionToOrigin.x = 0;

            // Calculate the new rotation for the CameraRig to look at the origin
            Quaternion newRigRotation = Quaternion.LookRotation(directionToOrigin);

            // Apply the new rotation to the CameraRig
            transform.rotation = newRigRotation;

            // Reset the flag
            setInitialPosition = false;
        }
    }
}