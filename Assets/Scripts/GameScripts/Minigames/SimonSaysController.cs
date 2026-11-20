using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Simon Says: reproduce una secuencia y el jugador la repite. La secuencia crece una nota por
// ronda y conserva las anteriores. Si se equivoca se repite la MISMA ronda, nunca regresa a la
// ronda 1 ni hay derrota. Mientras el jugador no toque nada, la secuencia se repite sola.
// El GameManager lo prende y lo apaga con SetPuzzleActive(bool).
[DisallowMultipleComponent]
public class SimonSaysController : MonoBehaviour
{

    public MinigameTriggers trigger;
    public enum PuzzleState
    {
        Inactive,
        Idle,
        PlayingSequence,
        WaitingInput,
        Solved,
    }

    [System.Serializable] public class IntEvent : UnityEvent<int> { }

    [Header("Activacion (GameManager)")]
    [Tooltip("El GameManager lo mueve con SetPuzzleActive(bool). En true para probar sin GameManager.")]
    [SerializeField] private bool puzzleActive = false;
    [Tooltip("Al activarse arranca solo. Si se apaga, hay que llamar StartPuzzle().")]
    [SerializeField] private bool autoStartWhenActivated = true;

    [Header("Botones")]
    [Tooltip("El orden define el indice de cada boton.")]
    [SerializeField] private List<SimonSaysbuttons> buttons = new List<SimonSaysbuttons>();
    [Tooltip("Si la lista esta vacia, los busca en los hijos.")]
    [SerializeField] private bool autoFindButtonsInChildren = true;

    [Header("Secuencia")]
    [Tooltip("ON: usa la secuencia de abajo. OFF: aleatoria.")]
    [SerializeField] private bool useManualSequence;
    [Tooltip("Indices de boton, 0 = el primero de la lista.")]
    [SerializeField] private List<int> manualSequence = new List<int> { 0, 1, 2, 3 };
    [Tooltip("Notas de la ronda 1.")]
    [SerializeField, Min(1)] private int firstRoundLength = 1;
    [Tooltip("Notas que se agregan por ronda.")]
    [SerializeField, Min(1)] private int notesPerRound = 1;
    [Tooltip("Rondas para ganar (en modo manual lo define el largo de la secuencia).")]
    [SerializeField, Min(1)] private int roundsToWin = 4;
    [Tooltip("OFF: no repite la misma nota seguida.")]
    [SerializeField] private bool allowRepeatedNotes = true;
    [Tooltip("0 = aleatorio. Otro valor = siempre la misma secuencia.")]
    [SerializeField] private int randomSeed;

    [Header("Tiempos (segundos)")]
    [Tooltip("Espera antes de reproducir.")]
    [SerializeField, Min(0f)] private float startDelay = 1f;
    [Tooltip("Cuanto dura prendida cada nota.")]
    [SerializeField, Min(0.05f)] private float litDuration = 0.45f;
    [Tooltip("Silencio entre notas.")]
    [SerializeField, Min(0f)] private float gapDuration = 0.2f;
    [Tooltip("Flash de acierto/error del panel.")]
    [SerializeField, Min(0f)] private float feedbackDuration = 0.6f;
    [Tooltip("Flash del boton que presiona el jugador.")]
    [SerializeField, Min(0.05f)] private float pressFlashDuration = 0.25f;

    [Header("Repeticion mientras espera")]
    [Tooltip("ON: si el jugador no toca nada, la secuencia se repite sola.")]
    [SerializeField] private bool repeatWhileWaiting = true;
    [Tooltip("Segundos sin tocar nada antes de repetirla.")]
    [SerializeField, Min(0.5f)] private float repeatAfter = 3f;

    [Header("Sonido")]
    [Tooltip("SScorrect: al completar cada ronda.")]
    public AudioClip correctClip;
    [Tooltip("ButtonSSFail: al equivocarse.")]
    public AudioClip errorClip;
    [Tooltip("Opcional. Vacio = el sonido se crea en el lugar del panel.")]
    public AudioSource audioSource;

    [Header("Debug / pruebas sin visor")]
    [Tooltip("Imprime la secuencia, los aciertos y los errores.")]
    [SerializeField] private bool debugLogs = true;
    [Tooltip("Teclas 1..9 presionan botones, R reinicia.")]
    [SerializeField] private bool debugKeyboardInput = true;

    [Header("Eventos")]
    public UnityEvent onPuzzleStarted;
    public IntEvent onRoundStarted;
    public IntEvent onRoundCompleted;
    public UnityEvent onMistakeMade;
    public UnityEvent onPuzzleSolved;

    private readonly List<int> m_Sequence = new List<int>();
    private System.Random m_Random;
    private int m_Round;
    private int m_InputIndex;

    public bool PuzzleActive => puzzleActive;

    public PuzzleState State { get; private set; } = PuzzleState.Inactive;

    public bool IsSolved => State == PuzzleState.Solved;

    // Ronda actual empezando en 1; 0 si no ha arrancado.
    public int CurrentRound => m_Round;

    // En modo manual las rondas salen del largo de la secuencia escrita.
    public int TotalRounds
    {
        get
        {
            if (!useManualSequence)
                return roundsToWin;

            var extra = Mathf.Max(0, manualSequence.Count - firstRoundLength);
            return 1 + Mathf.CeilToInt(extra / (float)notesPerRound);
        }
    }

    private void Awake()
    {
        AutoFindButtons();

        // Cada boton recibe su indice segun su posicion en la lista.
        for (var i = 0; i < buttons.Count; i++)
        {
            if (buttons[i] == null)
            {
                Debug.LogWarning($"[{nameof(SimonSaysController)}] El boton en el indice {i} esta vacio.", this);
                continue;
            }

            buttons[i].Initialize(this, i);
        }

#if UNITY_EDITOR
        m_LastPuzzleActive = puzzleActive;
#endif
    }

    private void Start()
    {
        if (buttons.Count == 0)
        {
            Debug.LogWarning($"[{nameof(SimonSaysController)}] No hay botones asignados: el puzzle no va a correr.", this);
            State = PuzzleState.Inactive;
            return;
        }

        ApplyActiveState();
    }

    private void Update()
    {
        if (trigger.simonSays)
        {
            puzzleActive = true;
        }

#if UNITY_EDITOR
        CheckPuzzleActiveToggle();
#endif

        if (debugKeyboardInput)
            CheckDebugKeyboard();
    }

    // ---- API publica (GameManager) ----

    // Prende o apaga el puzzle. Sirve directo en un UnityEvent con bool.
    public void SetPuzzleActive(bool value)
    {
        if (puzzleActive == value)
            return;

        puzzleActive = value;
        ApplyActiveState();
    }

    public void ActivatePuzzle() => SetPuzzleActive(true);

    public void DeactivatePuzzle() => SetPuzzleActive(false);

    // Arranca desde la ronda 1 con una secuencia nueva.
    public void StartPuzzle()
    {
        if (!puzzleActive)
        {
            Debug.LogWarning($"[{nameof(SimonSaysController)}] StartPuzzle() con el puzzle inactivo: usa SetPuzzleActive(true) primero.", this);
            return;
        }

        if (buttons.Count == 0)
            return;

        StopPuzzle();

        // El GetInstanceID() evita que dos copias arranquen con la misma semilla.
        m_Random = randomSeed == 0
            ? new System.Random(System.Environment.TickCount ^ (GetInstanceID() * 397))
            : new System.Random(randomSeed);
        m_Sequence.Clear();
        m_Round = 1;

        Log($"Arrancando: {buttons.Count} botones, {TotalRounds} rondas.");
        onPuzzleStarted?.Invoke();
        StartCoroutine(PlayRoundRoutine());
    }

    // Detiene y apaga los botones, sin tocar el bool de activacion.
    public void StopPuzzle()
    {
        StopAllCoroutines();
        SetInputEnabled(false);
        TurnOffAllButtons();
        m_InputIndex = 0;
        State = puzzleActive ? PuzzleState.Idle : PuzzleState.Inactive;
    }

    // Lo deja como si nunca se hubiera jugado.
    public void ResetPuzzle()
    {
        StopPuzzle();
        m_Sequence.Clear();
        m_Round = 0;
    }

    // Simula un press sin VR: lo usan las teclas 1..9, pero sirve desde un UnityEvent.
    public void DebugPressButton(int index)
    {
        var button = GetButton(index);
        if (button == null)
        {
            Log($"No hay boton en el indice {index} (la lista tiene {buttons.Count}).");
            return;
        }

        if (State != PuzzleState.WaitingInput)
        {
            // Se prende igual, para ver que el boton responde.
            button.Play(pressFlashDuration);
            Log($"Press del boton {index} ignorado: el estado es {State}.");
            return;
        }

        OnButtonPressed(button);
    }

    // ---- Input de los botones ----

    // Lo llama SimonSaysbuttons al ser presionado.
    public void OnButtonPressed(SimonSaysbuttons button)
    {
        if (State != PuzzleState.WaitingInput || button == null)
            return;

        if (button.Index < 0 || m_InputIndex >= m_Sequence.Count)
            return;

        button.Play(pressFlashDuration);

        var expected = m_Sequence[m_InputIndex];

        // Error: se repite la misma ronda, con la misma secuencia.
        if (button.Index != expected)
        {
            Log($"Error: se presiono el boton {button.Index} y se esperaba el {expected}. Se repite la ronda {m_Round}.");
            m_InputIndex = 0;
            SetInputEnabled(false);
            onMistakeMade?.Invoke();
            Sfx.Play(errorClip, audioSource, transform.position);
            StopAllCoroutines();
            StartCoroutine(FeedbackAndReplayRoutine(success: false));
            return;
        }

        m_InputIndex++;
        Log($"Boton {button.Index} correcto ({m_InputIndex}/{m_Sequence.Count}).");

        if (m_InputIndex >= m_Sequence.Count)
            CompleteRound();
    }

    // ---- Flujo interno ----

    private void ApplyActiveState()
    {
        if (!puzzleActive)
        {
            StopPuzzle();
            return;
        }

        State = PuzzleState.Idle;

        if (autoStartWhenActivated)
            StartPuzzle();
    }

    // Reproduce la ronda, espera input, y la repite mientras el jugador no toque nada.
    private IEnumerator PlayRoundRoutine()
    {
        State = PuzzleState.PlayingSequence;
        SetInputEnabled(false);
        BuildSequenceForCurrentRound();
        Log($"Ronda {m_Round}/{TotalRounds} -> secuencia: {string.Join(", ", m_Sequence)}");
        onRoundStarted?.Invoke(m_Round);

        if (startDelay > 0f)
            yield return new WaitForSeconds(startDelay);

        do
        {
            yield return PlayLightsRoutine();

            m_InputIndex = 0;
            State = PuzzleState.WaitingInput;
            SetInputEnabled(true);
            Log("Esperando que el jugador repita la secuencia.");

            if (!repeatWhileWaiting)
                yield break;

            yield return WaitForFirstPressRoutine();
        }
        while (State == PuzzleState.WaitingInput && m_InputIndex == 0);
    }

    // Prende las notas una por una.
    private IEnumerator PlayLightsRoutine()
    {
        State = PuzzleState.PlayingSequence;
        SetInputEnabled(false);

        for (var i = 0; i < m_Sequence.Count; i++)
        {
            var button = GetButton(m_Sequence[i]);
            if (button != null)
                button.Play(litDuration);

            yield return new WaitForSeconds(litDuration + gapDuration);
        }
    }

    // Sale cuando el jugador toca el primer boton, o cuando toca repetir la secuencia.
    private IEnumerator WaitForFirstPressRoutine()
    {
        var elapsed = 0f;

        while (m_InputIndex == 0 && State == PuzzleState.WaitingInput && elapsed < repeatAfter)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (m_InputIndex == 0 && State == PuzzleState.WaitingInput)
            Log("Nadie toco nada: se repite la secuencia.");
    }

    // Flash de acierto o error, y a reproducir la ronda que toque.
    private IEnumerator FeedbackAndReplayRoutine(bool success)
    {
        FlashAll(success);
        yield return new WaitForSeconds(feedbackDuration);
        yield return PlayRoundRoutine();
    }

    private IEnumerator SolveRoutine()
    {
        State = PuzzleState.Solved;
        FlashAll(success: true);
        yield return new WaitForSeconds(feedbackDuration);
        TurnOffAllButtons();
        Log("Puzzle resuelto.");
        trigger.simonSays = false;
        puzzleActive = false;
        onPuzzleSolved?.Invoke();
    }

    // Ronda bien repetida: pasa a la siguiente o lo resuelve.
    private void CompleteRound()
    {
        SetInputEnabled(false);
        Log($"Ronda {m_Round} completada.");
        onRoundCompleted?.Invoke(m_Round);
        Sfx.Play(correctClip, audioSource, transform.position);
        StopAllCoroutines();

        if (m_Round >= TotalRounds)
        {
            StartCoroutine(SolveRoutine());
            return;
        }

        m_Round++;
        StartCoroutine(FeedbackAndReplayRoutine(success: true));
    }

    // La ronda N conserva las notas de la anterior, asi repetir la ronda da la misma secuencia.
    private void BuildSequenceForCurrentRound()
    {
        var targetLength = firstRoundLength + (m_Round - 1) * notesPerRound;

        if (useManualSequence)
        {
            m_Sequence.Clear();

            var count = Mathf.Min(targetLength, manualSequence.Count);
            for (var i = 0; i < count; i++)
                m_Sequence.Add(Mathf.Clamp(manualSequence[i], 0, buttons.Count - 1));

            if (m_Sequence.Count == 0)
                Debug.LogWarning($"[{nameof(SimonSaysController)}] La secuencia manual esta vacia.", this);

            return;
        }

        if (m_Sequence.Count > targetLength)
            m_Sequence.RemoveRange(targetLength, m_Sequence.Count - targetLength);

        while (m_Sequence.Count < targetLength)
            m_Sequence.Add(NextRandomNote());
    }

    private int NextRandomNote()
    {
        m_Random ??= new System.Random();

        var note = m_Random.Next(buttons.Count);

        if (!allowRepeatedNotes && buttons.Count > 1 && m_Sequence.Count > 0)
        {
            var last = m_Sequence[m_Sequence.Count - 1];
            while (note == last)
                note = m_Random.Next(buttons.Count);
        }

        return note;
    }

    private SimonSaysbuttons GetButton(int index)
    {
        return index >= 0 && index < buttons.Count ? buttons[index] : null;
    }

    // Abre o cierra la ventana de input de todos los botones.
    private void SetInputEnabled(bool value)
    {
        foreach (var button in buttons)
        {
            if (button != null)
                button.SetInputEnabled(value);
        }
    }

    private void TurnOffAllButtons()
    {
        foreach (var button in buttons)
        {
            if (button != null)
                button.TurnOff();
        }
    }

    private void FlashAll(bool success)
    {
        foreach (var button in buttons)
        {
            if (button == null)
                continue;

            if (success)
                button.FlashSuccess(feedbackDuration);
            else
                button.FlashError(feedbackDuration);
        }
    }

    private void AutoFindButtons()
    {
        if (!autoFindButtonsInChildren)
            return;

        // Limpia huecos de la lista del Inspector.
        buttons.RemoveAll(b => b == null);

        if (buttons.Count > 0)
            return;

        buttons.AddRange(GetComponentsInChildren<SimonSaysbuttons>(true));
    }

    private void Log(string message)
    {
        if (debugLogs)
            Debug.Log($"[SimonSays] {message}", this);
    }

    // ---- Debug ----

    private void CheckDebugKeyboard()
    {
#if ENABLE_INPUT_SYSTEM
        var keyboard = UnityEngine.InputSystem.Keyboard.current;
        if (keyboard == null)
            return;

        var count = Mathf.Min(buttons.Count, 9);
        for (var i = 0; i < count; i++)
        {
            var key = (UnityEngine.InputSystem.Key)((int)UnityEngine.InputSystem.Key.Digit1 + i);
            if (keyboard[key].wasPressedThisFrame)
                DebugPressButton(i);
        }

        if (keyboard.rKey.wasPressedThisFrame)
        {
            Log("Tecla R: reiniciando el puzzle.");
            StartPuzzle();
        }
#endif
    }

#if UNITY_EDITOR
    private bool m_LastPuzzleActive;

    private void OnValidate()
    {
        firstRoundLength = Mathf.Max(1, firstRoundLength);
        notesPerRound = Mathf.Max(1, notesPerRound);
        roundsToWin = Mathf.Max(1, roundsToWin);
        m_LastPuzzleActive = puzzleActive;

        // Llena la lista en el Inspector para poder reordenarla a mano.
        if (!Application.isPlaying)
            AutoFindButtons();
    }

    // Permite mover el bool a mano en Play Mode.
    private void CheckPuzzleActiveToggle()
    {
        if (!Application.isPlaying || m_LastPuzzleActive == puzzleActive)
            return;

        var value = puzzleActive;
        puzzleActive = m_LastPuzzleActive;
        m_LastPuzzleActive = value;
        SetPuzzleActive(value);
    }
#endif
}
