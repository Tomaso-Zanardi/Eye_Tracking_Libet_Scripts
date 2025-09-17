using UnityEngine;

public class TeleportViewToReference : MonoBehaviour
{
    public Transform referenceObject; // The reference object to match the position and rotation.
    public Transform cameraRig; // The VR camera rig that represents the player's position and orientation in the scene.
    public KeyCode teleportKey = KeyCode.C; // The key to press to teleport the camera/player.

    void Update()
    {
        if (Input.GetKeyDown(teleportKey))
        {
            TeleportCameraRig();
        }
    }

    void TeleportCameraRig()
    {
        // Match the camera rig's position to the reference object's position.
        cameraRig.transform.position = referenceObject.transform.position;

        // Match the camera rig's rotation to the reference object's rotation.
        cameraRig.transform.rotation = referenceObject.transform.rotation;
    }
}