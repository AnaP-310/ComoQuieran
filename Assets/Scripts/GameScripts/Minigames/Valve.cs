using UnityEngine;

public class Valve : MonoBehaviour
{
    public MinigameTriggers trigger;
    public HingeJoint joint;
    public float rotationValue;

    [Header("Sonido")]
    [Tooltip("valve")]
    public AudioClip valveClip;
    [Tooltip("Opcional. Vacio = el sonido se crea en el lugar de la valvula.")]
    public AudioSource audioSource;

    private bool soundPlayed;

    void Update()
    {
        // Track the current rotation of the valve on the axis it can rotate on
        rotationValue = joint.angle;

        // If the rotation value is within a certain range, run the event
        if (rotationValue >= 150 && rotationValue <= 180)
        {
            trigger.valveRotate = false;

            // Una sola vez al llegar al rango, no cada frame.
            if (!soundPlayed)
            {
                soundPlayed = true;
                Sfx.Play(valveClip, audioSource, transform.position);
            }
        }
        else
        {
            soundPlayed = false;
        }
    }
}
