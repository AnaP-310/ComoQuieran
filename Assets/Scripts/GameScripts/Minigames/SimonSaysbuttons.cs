using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

// Un boton del Simon Says: le avisa al SimonSaysController que fue presionado y da el feedback
// (emision + audio). La logica de la secuencia vive en el controller.
// En el GameObject del boton: Collider + XRSimpleInteractable + XRPokeFilter + Renderer.
[DisallowMultipleComponent]
public class SimonSaysbuttons : MonoBehaviour
{
    [Header("Interaccion (XRI)")]
    [Tooltip("XRSimpleInteractable (+ XRPokeFilter para poke). Vacio = se busca aqui mismo.")]
    [SerializeField] private XRBaseInteractable interactable;

    [Header("Feedback - Emision")]
    [Tooltip("Renderer que se prende. Vacio = se busca aqui o en los hijos.")]
    [SerializeField] private Renderer targetRenderer;
    [Tooltip("Material dentro del Renderer (0 si solo tiene uno).")]
    [SerializeField, Min(0)] private int materialIndex;
    [SerializeField] private Color litColor = new Color(1f, 0.85f, 0.2f);
    [SerializeField] private Color errorColor = new Color(1f, 0.15f, 0.1f);
    [SerializeField] private Color successColor = new Color(0.2f, 1f, 0.35f);
    [Tooltip("Multiplicador de la emision. Subelo si no se nota.")]
    [SerializeField, Min(0f)] private float emissionIntensity = 3f;

    [Header("Feedback - Audio (opcional)")]
    [Tooltip("Vacio = se busca un AudioSource aqui mismo.")]
    [SerializeField] private AudioSource audioSource;
    [Tooltip("Tono de este boton. Se puede dejar vacio.")]
    [SerializeField] private AudioClip toneClip;

    private static readonly int k_EmissionColor = Shader.PropertyToID("_EmissionColor");
    private static readonly int k_BaseColor = Shader.PropertyToID("_BaseColor");
    private static readonly int k_LegacyColor = Shader.PropertyToID("_Color");
    private const string k_EmissionKeyword = "_EMISSION";

    private SimonSaysController m_Controller;
    private Material m_RuntimeMaterial;
    private Color m_BaseEmission;
    private bool m_BaseEmissionEnabled;
    private bool m_HasEmission;
    private int m_FallbackColorId = -1;
    private Color m_BaseFallbackColor;
    private Coroutine m_FlashRoutine;
    private bool m_InputEnabled;

    // Posicion en la lista del controller; -1 si no fue registrado.
    public int Index { get; private set; } = -1;

    private void Awake()
    {
        AutoAssignReferences();
        CacheMaterial();
    }

    // Rellena lo que se haya dejado vacio en el Inspector.
    private void AutoAssignReferences()
    {
        if (interactable == null)
            interactable = GetComponent<XRBaseInteractable>();

        if (targetRenderer == null)
            targetRenderer = GetComponentInChildren<Renderer>();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        if (interactable != null)
            interactable.selectEntered.AddListener(OnSelectEntered);
        else
            Debug.LogWarning($"[{nameof(SimonSaysbuttons)}] '{name}' no tiene XRBaseInteractable: no se podra presionar.", this);
    }

    private void OnDisable()
    {
        if (interactable != null)
            interactable.selectEntered.RemoveListener(OnSelectEntered);

        TurnOff();
    }

    // El controller le da su indice al arrancar.
    public void Initialize(SimonSaysController controller, int index)
    {
        m_Controller = controller;
        Index = index;
    }

    // El controller abre o cierra la ventana de input (durante el playback se ignoran los toques).
    public void SetInputEnabled(bool value)
    {
        m_InputEnabled = value;
    }

    // Prende el boton con su color y su tono. duration <= 0 lo deja prendido hasta TurnOff().
    public void Play(float duration)
    {
        PlayClip();
        Flash(litColor, duration);
    }

    public void FlashError(float duration) => Flash(errorColor, duration);

    public void FlashSuccess(float duration) => Flash(successColor, duration);

    // Apaga y corta el flash en curso.
    public void TurnOff()
    {
        if (m_FlashRoutine != null)
        {
            StopCoroutine(m_FlashRoutine);
            m_FlashRoutine = null;
        }

        RestoreEmission();
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (!m_InputEnabled || m_Controller == null)
            return;

        m_Controller.OnButtonPressed(this);
    }

    private void Flash(Color color, float duration)
    {
        if (m_FlashRoutine != null)
            StopCoroutine(m_FlashRoutine);

        m_FlashRoutine = StartCoroutine(FlashRoutine(color, duration));
    }

    private IEnumerator FlashRoutine(Color color, float duration)
    {
        SetEmission(color * emissionIntensity, lit: true);

        if (duration > 0f)
        {
            yield return new WaitForSeconds(duration);
            RestoreEmission();
        }

        m_FlashRoutine = null;
    }

    private void PlayClip()
    {
        // Sin AudioSource el sonido se crea en el lugar del boton.
        Sfx.Play(toneClip, audioSource, transform.position);
    }

    // Guarda una instancia del material y su estado original.
    private void CacheMaterial()
    {
        if (targetRenderer == null)
            return;

        // materials[] ya devuelve instancias; se reasigna para que el Renderer se quede con esas.
        var materials = targetRenderer.materials;
        if (materials.Length == 0)
            return;

        var index = Mathf.Clamp(materialIndex, 0, materials.Length - 1);
        m_RuntimeMaterial = materials[index];
        targetRenderer.materials = materials;

        if (m_RuntimeMaterial == null)
            return;

        m_HasEmission = m_RuntimeMaterial.HasProperty(k_EmissionColor);
        if (m_HasEmission)
        {
            m_BaseEmission = m_RuntimeMaterial.GetColor(k_EmissionColor);
            m_BaseEmissionEnabled = m_RuntimeMaterial.IsKeywordEnabled(k_EmissionKeyword);
            return;
        }

        // Sin emision (Unlit y similares) se tiñe el color base.
        if (m_RuntimeMaterial.HasProperty(k_BaseColor))
            m_FallbackColorId = k_BaseColor;
        else if (m_RuntimeMaterial.HasProperty(k_LegacyColor))
            m_FallbackColorId = k_LegacyColor;

        if (m_FallbackColorId == -1)
        {
            Debug.LogWarning($"[{nameof(SimonSaysbuttons)}] El material de '{name}' no tiene _EmissionColor ni color base: no habra feedback visual.", this);
            return;
        }

        m_BaseFallbackColor = m_RuntimeMaterial.GetColor(m_FallbackColorId);
    }

    private void SetEmission(Color color, bool lit)
    {
        if (m_RuntimeMaterial == null)
            return;

        if (!m_HasEmission)
        {
            if (m_FallbackColorId == -1)
                return;

            // El color base es LDR y 'color' viene multiplicado por la intensidad.
            var tint = lit
                ? new Color(Mathf.Clamp01(color.r), Mathf.Clamp01(color.g), Mathf.Clamp01(color.b), m_BaseFallbackColor.a)
                : m_BaseFallbackColor;

            m_RuntimeMaterial.SetColor(m_FallbackColorId, tint);
            return;
        }

        m_RuntimeMaterial.SetColor(k_EmissionColor, color);

        if (lit)
            m_RuntimeMaterial.EnableKeyword(k_EmissionKeyword);
        else if (!m_BaseEmissionEnabled)
            m_RuntimeMaterial.DisableKeyword(k_EmissionKeyword);
    }

    private void RestoreEmission()
    {
        SetEmission(m_BaseEmission, m_BaseEmissionEnabled);
    }

#if UNITY_EDITOR
    private void OnValidate() => AutoAssignReferences();
#endif
}
