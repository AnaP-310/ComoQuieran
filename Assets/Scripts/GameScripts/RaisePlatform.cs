using UnityEngine;
using System.Collections;

public class RaisePlatform : MonoBehaviour
{
    public GameObject platform;
    public float distance;
    public float duration;
    [Header("Sonido")]
    [Tooltip("ButtonArrows, o el motor de la plataforma.")]
    public AudioClip moveClip;
    [Tooltip("Opcional. Vacio = el sonido se crea en el lugar de la plataforma.")]
    public AudioSource audioSource;

    private bool isMoving = false;
    private bool reverseDirection = false;
    private int position = 0;

    //chechamos si esta en la posicion de abajo, si si cambiamos la posicion e iniciamos la corutina pa mover la plataforma
    public void Raise()
    {
        
        if (position == 0)
        {
            position = 1;
            reverseDirection = false;
            StartCoroutine(Move()); ;
        }
        
    }

    //chechamos si esta en la posicion de arriba, si si cambiamos la posicion e iniciamos la corutina pa mover la plataforma
    public void Lower()
    {
        if (position == 1)
        {
            position = 0;
            reverseDirection = true;
            StartCoroutine(Move());
        }
        
    }

    private IEnumerator Move()
    {
        isMoving = true;
        Sfx.Play(moveClip, audioSource, platform.transform.position);
        int directionMultiplier = reverseDirection ? -1 : 1;

        Vector3 startPosition = platform.transform.position;
        Vector3 targetPosition = startPosition + Vector3.up*distance*directionMultiplier;
        float elapsedTime = 0f;

        

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            //move smoothtly entre la posicion de inicio y la final, elapsed time/duration se usa para obtener un valor entre 1 y 0 ya que el lerp clampea el valor entre 1 y 0
            platform.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime/duration);

            yield return null;
        }

        platform.transform.position = targetPosition;
        isMoving = false;
    }
}
