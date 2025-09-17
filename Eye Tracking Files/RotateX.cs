using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class RotateX : MonoBehaviour
{
    private float rotationDuration = 2.56f;
    private float rotationSpeed;
    public float currentRotationX;
    private float previousRotationX;
    public int rotationCount = 0;
    private float totalRotation = 0f; // New variable to track total rotation

    void Start()
    {
        rotationSpeed = 360f / rotationDuration;
        rotationCount = 0;
    }

    void OnEnable()
    {
        ResetRotation();
    }

    private void OnDisable()
    {
        rotationCount = 0;
        ResetRotation();
    }

    private void ResetRotation()
    {
        currentRotationX = Random.Range(0f, 360f);
        previousRotationX = currentRotationX;
        totalRotation = 0f; // Reset total rotation
        transform.rotation = Quaternion.Euler(currentRotationX, 0, -90);
    }

    void Update()
    {
        float rotationThisFrame = rotationSpeed * Time.deltaTime;
        currentRotationX += rotationThisFrame;
        currentRotationX %= 360f;
        transform.rotation = Quaternion.Euler(currentRotationX, 0, -90);

        totalRotation += rotationThisFrame; // Increment total rotation

        // Check if a full rotation (360 degrees) is completed from the start/reset point
        if (totalRotation >= 360f)
        {
            rotationCount++;
            Debug.Log("Full rotation completed. Count: " + rotationCount);
            totalRotation %= 360f; // Reset total rotation for the next cycle
        }

        previousRotationX = currentRotationX;
    }
}