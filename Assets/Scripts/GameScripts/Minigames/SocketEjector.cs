using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class SocketEjector : MonoBehaviour
{
    public XRSocketInteractor socket;
    public Transform SpawnPoint;
    public GameObject fuse;

    [Header("Sonido")]
    [Tooltip("Opcional: al salir expulsado el fusible.")]
    public AudioClip ejectClip;
    [Tooltip("Opcional. Vacio = el sonido se crea en el lugar del socket.")]
    public AudioSource audioSource;

    private Vector3 originalScale;

    private void Start()
    {
        originalScale = fuse.transform.localScale;
    }

    public void EjectObject()
    {
        if (socket.hasSelection)
        {
            StartCoroutine(ForceEjectRoutine());
        }
    }

    private IEnumerator ForceEjectRoutine()
    {
        IXRSelectInteractable interactable = socket.firstInteractableSelected;
        socket.socketActive = false;
        if (interactable != null && socket.interactionManager != null)
        {
            socket.interactionManager.SelectExit(socket, interactable);
            fuse.transform.position = SpawnPoint.position;
            fuse.transform.localScale = originalScale;
            Sfx.Play(ejectClip, audioSource, transform.position);
        }

        yield return new WaitForSeconds(0.5f);
        socket.socketActive = true;
    }
}
