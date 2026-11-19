using UnityEngine;
using System.Collections;

public class RotatingPlatform : MonoBehaviour
{
    public GameObject platform;
    public Transform pivot;
    public float degrees;
    public float duration;
    private bool isRotating = false;
    private bool reverseDirection = false;
    private int position = 1;

    
    private void OnTriggerEnter(Collider other)
    {
        //imaginense que las posiciones son 0, 1, 2, asi que la posicion del centro es 1, la izquierda es 0 y la derecha es 2

        //chechamos si la paltaforma NO esta en la posicion mas izquierda
        if (other.CompareTag("LeftRotation"))
        {
            if (position >0)
            {
                reverseDirection = true;
                TriggerRotation();
            }   
        }
        //chechamos si la paltaforma NO esta en la posicion mas derecha
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
            StartCoroutine(Rotate());
        }
    }


    private IEnumerator Rotate()
    {
        isRotating = true;
        float rotatedSoFar = 0f;
        float degreesPerSecond = degrees / duration;

        //operador ternario para decidir la direccion y encima ayuda a cambiar la posicion yippe que bonito
        int directionMultiplier = reverseDirection ? -1 : 1;
        position += directionMultiplier;

        while (rotatedSoFar < degrees)
        {
            float step = degreesPerSecond * Time.deltaTime;

            //pa evitar overshooting
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
