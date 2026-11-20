using UnityEngine;

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
        }

        yield return new WaitForSeconds(0.5f);
        socket.socketActive = true;
    }
}
