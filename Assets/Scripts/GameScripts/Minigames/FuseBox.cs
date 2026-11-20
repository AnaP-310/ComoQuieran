using UnityEngine;
using UnityEngine.Events;

public class FuseBox : MonoBehaviour
{
    public MinigameTriggers trigger;
    public GameObject fuse1;
    public Transform fuse1SpawnPoint;
    public GameObject fuse2;
    public Transform fuse2SpawnPoint;

    public UnityEvent EjectFuse;

    [Header("Sonido")]
    [Tooltip("fuse popin: al meter cada fusible.")]
    public AudioClip fuseInClip;
    [Tooltip("Opcional: al botar los fusibles cuando arranca el minijuego.")]
    public AudioClip ejectClip;
    [Tooltip("Opcional. Vacio = el sonido se crea en el lugar de la caja.")]
    public AudioSource audioSource;

    private bool fuse1On;
    private bool fuse2On;
    private bool CanReset;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        fuse1.transform.position = fuse1SpawnPoint.position;
        fuse2.transform.position = fuse2SpawnPoint.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (trigger.fuseBox && CanReset)
        {
            CanReset = false;
            EjectFuse.Invoke();
            Sfx.Play(ejectClip, audioSource, transform.position);
        }

        if (fuse1On && fuse2On)
        {
            fuse1On = false;
            fuse2On = false;
            CanReset = true;
            trigger.fuseBox = false;
        }
    }

    public void Fuse1In()
    {
        fuse1On = true;
        Sfx.Play(fuseInClip, audioSource, transform.position);
    }
    public void Fuse2In()
    {
        fuse2On = true;
        Sfx.Play(fuseInClip, audioSource, transform.position);
    }
}
