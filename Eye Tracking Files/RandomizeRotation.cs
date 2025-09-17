using UnityEngine;

public class RandomizeChildRotation : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        // Check if the parent is active
        if (transform.parent.gameObject.activeInHierarchy)
        {
            float randomRotationX = Random.Range(-180f, 180f);
            transform.localRotation = Quaternion.Euler(randomRotationX, transform.localEulerAngles.y, transform.localEulerAngles.z);
            // Disable this script to prevent continuous rotation updates
            this.enabled = false;
        }
    }
}