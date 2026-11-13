using UnityEngine;
using System.Collections;

public class RotatingPlatform : MonoBehaviour
{
    public GameObject platform;
    public Transform pivot;
    public float degrees;
    public float duration;
    private bool isRotating = false;
    public bool reverseDirection = false;
    private int position = 1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("LeftRotation"))
        {
            if (position >0)
            {
                reverseDirection = true;
                TriggerRotation();
            }   
        }
        if (other.CompareTag("RightRotation"))
        {
            if (position < 2)
            {
                reverseDirection = false;
                TriggerRotation();
            }
        }
    }

    public void TriggerRotation()
    {
        if (!isRotating)
        {
            StartCoroutine(RotateSmoothly());
        }
    }


    IEnumerator RotateSmoothly()
    {
        isRotating = true;
        float rotatedSoFar = 0f;
        float degreesPerSecond = degrees / duration;

        int directionMultiplier = reverseDirection ? -1 : 1;
        position += directionMultiplier;

        while (rotatedSoFar < degrees)
        {
            float step = degreesPerSecond * Time.deltaTime;

            // Prevent overshooting the target angle
            if (rotatedSoFar + step > degrees)
            {
                step = degrees - rotatedSoFar;
            }

            platform.transform.RotateAround(pivot.position, Vector3.up, step * directionMultiplier);
            rotatedSoFar += step;

            yield return null;
        }

        isRotating = false;
    }
}
