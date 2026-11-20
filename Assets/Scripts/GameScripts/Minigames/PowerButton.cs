using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class PowerButton : MonoBehaviour
{
    public MinigameTriggers trigger;
    public Transform visualTarget;
    public Vector3 localAxis;
    public float resetSpeed = 10;
    public float followAngleThreshold = 45;

    [Header("Sonido")]
    [Tooltip("powerbutton, o ButtonArrows si este es el de flechas.")]
    public AudioClip pressClip;
    [Tooltip("Opcional. Vacio = el sonido se crea en el lugar del boton.")]
    public AudioSource audioSource;

    private bool freeze;
    private Vector3 initialLocalPosition;

    private Vector3 offset;
    private Transform pokeAttachTransform;

    private XRBaseInteractable interactable;
    private bool isFollowing = false;

    private void Start()
    {
        initialLocalPosition = visualTarget.localPosition;

        interactable = GetComponent<XRBaseInteractable>();
        interactable.hoverEntered.AddListener(Follow);
        interactable.hoverExited.AddListener(Rset);
        interactable.selectEntered.AddListener(Freeze);
    }

    public void Follow(BaseInteractionEventArgs hover)
    {
        if(hover.interactorObject is XRPokeInteractor)
        {
            XRPokeInteractor interactor = (XRPokeInteractor)hover.interactorObject;

            pokeAttachTransform = interactor.attachTransform;
            offset = visualTarget.position - pokeAttachTransform.position;

            float pokeAngle = Vector3.Angle(offset,visualTarget.TransformDirection(localAxis));

            if(pokeAngle < followAngleThreshold)
            {
                isFollowing = true;
                freeze = false;
            }
        }
    }

    public void Rset(BaseInteractionEventArgs hover)
    {
        if(hover.interactorObject is XRPokeInteractor)
        {
            isFollowing = false;
            freeze = false;
        }
    }

    public void Freeze(BaseInteractionEventArgs hover)
    {
        if (hover.interactorObject is XRPokeInteractor)
        {
            freeze = true;

            // Aqui es cuando el boton quedo hundido: ese es el click.
            Sfx.Play(pressClip, audioSource, transform.position);
        }
    }

    private void Update()
    {
        if (freeze)
            return;


        if(isFollowing)
        {
            Vector3 localTargetPosition = visualTarget.InverseTransformPoint(pokeAttachTransform.position + offset);
            Vector3 constrainedLocalTargerPosition = Vector3.Project(localTargetPosition, localAxis);
            visualTarget.position = visualTarget.TransformPoint(constrainedLocalTargerPosition);
        }
        else
        {
            visualTarget.localPosition = Vector3.Lerp(visualTarget.localPosition, initialLocalPosition, Time.deltaTime * resetSpeed);
        }

    }

    public void ButtonPresed()
    {
        trigger.powerButton = false;
    }
}
