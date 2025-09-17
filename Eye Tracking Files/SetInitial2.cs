using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetInitial2 : MonoBehaviour
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
            
            GameObject vrCamera = GameObject.Find(vrCameraName);
            
            
            Vector3 cameraOffset = vrCamera.transform.position - transform.position;
            
            Vector3 newRigPosition = targetObject.transform.position - cameraOffset;
            transform.position = newRigPosition;

            // After repositioning, recalculate the direction to look at from the rig's new position
            Vector3 targetDirection = Vector3.zero - transform.position; // Direction from rig to origin
            targetDirection.y = 0; // Keep it flat

            Quaternion newRigRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = newRigRotation;

            setInitialPosition = false;
        }
    }

}