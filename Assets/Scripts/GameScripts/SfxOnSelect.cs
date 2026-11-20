using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

// Suena un clip al presionar el interactable de este mismo objeto.
// Para botones que no tienen script propio, como los arrow buttons.
[DisallowMultipleComponent]
public class SfxOnSelect : MonoBehaviour
{
    [Tooltip("El clip que suena al presionar.")]
    public AudioClip clip;
    [Tooltip("Opcional. Vacio = el sonido se crea en el lugar del boton.")]
    public AudioSource audioSource;

    private XRBaseInteractable interactable;

    private void OnEnable()
    {
        if (interactable == null)
            interactable = GetComponent<XRBaseInteractable>();

        if (interactable != null)
            interactable.selectEntered.AddListener(OnSelectEntered);
    }

    private void OnDisable()
    {
        if (interactable != null)
            interactable.selectEntered.RemoveListener(OnSelectEntered);
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        Sfx.Play(clip, audioSource, transform.position);
    }
}
