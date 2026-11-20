using UnityEngine;

public class SlappyHand : MonoBehaviour
{
    public MinigameTriggers Trigger;

    [Header("Sonido")]
    [Tooltip("HandSlap")]
    public AudioClip slapClip;
    [Tooltip("Opcional. Vacio = el sonido se crea en el lugar del golpe.")]
    public AudioSource audioSource;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("PlayerHand"))
        {
            // Suena siempre que le pegan, aunque el minijuego no este activo.
            Sfx.Play(slapClip, audioSource, transform.position);

            if (Trigger.slappyHand)
            {
                Trigger.slappyHand = false;
                Debug.Log(Trigger.slappyHand);
            }
        }
    }
}
