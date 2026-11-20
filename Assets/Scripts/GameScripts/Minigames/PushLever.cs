using UnityEngine;

public class PushLever : MonoBehaviour
{
    public MinigameTriggers Trigger;

    [Header("Sonido")]
    [Tooltip("Lever1 o LeverSlide, el que le quede a esta palanca.")]
    public AudioClip leverClip;
    [Tooltip("Opcional. Vacio = el sonido se crea en el lugar de la palanca.")]
    public AudioSource audioSource;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Lever"))
        {
            Sfx.Play(leverClip, audioSource, transform.position);

            if (Trigger.pushLever)
            {
                Trigger.pushLever = false;
                Debug.Log(Trigger.pushLever + "pushLever");
            }
        }
    }
}
