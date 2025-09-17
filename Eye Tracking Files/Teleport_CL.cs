using UnityEngine;
using UnityEngine.XR;

public class Teleport_CL : MonoBehaviour
{
    public Transform referenceObject;
    public Transform cameraRig;
    public Transform headset; // Reference to the actual VR headset transform
    public KeyCode teleportKey = KeyCode.C;

    private Vector3 initialHeadsetLocalPosition;
    private Quaternion initialHeadsetLocalRotation;

    void Start()
    {
        // Store the initial local position and rotation of the headset relative to the rig
        initialHeadsetLocalPosition = cameraRig.InverseTransformPoint(headset.position);
        initialHeadsetLocalRotation = Quaternion.Inverse(cameraRig.rotation) * headset.rotation;
    }

    void Update()
    {
        if (Input.GetKeyDown(teleportKey))
        {
            TeleportCameraRig();
        }
    }

    void TeleportCameraRig()
    {
        // Calculate the offset between the headset and the rig
        Vector3 headsetOffset = cameraRig.TransformPoint(initialHeadsetLocalPosition) - cameraRig.position;

        // Set the rig's position, accounting for the headset offset
        cameraRig.position = referenceObject.position - headsetOffset;

        // Calculate the rotation difference
        Quaternion rotationDifference = referenceObject.rotation * Quaternion.Inverse(initialHeadsetLocalRotation);

        // Apply the rotation to the rig
        cameraRig.rotation = rotationDifference;

        // Optionally, you might want to reset the VR tracking origin
        InputTracking.Recenter();
    }
}