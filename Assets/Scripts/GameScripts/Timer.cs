using UnityEngine;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour
{
    public float time;
    private float timePassed = 0;
    private int randomNumber;
    private bool timerRunning = true;
    public MinigameTriggers triggers;
    public float timeBetweenGames = 10;
    private float actualTimeBetweenGames;
    private int gamesLeft = 0;
    public GameObject player;


    [Header("Ambiente")]
    [Tooltip("mechanist music: va en loop y mas fuerte que el ambiente.")]
    public AudioClip music;
    [Range(0f, 1f)] public float musicVolume = 0.7f;
    [Tooltip("ambiance sound: el soundscape de fondo, mas bajito.")]
    public AudioClip ambiance;
    [Range(0f, 1f)] public float ambianceVolume = 0.3f;

    private void Start()
    {
        player.SetActive(true);
        actualTimeBetweenGames = timeBetweenGames;
        // Los AudioSource se crean aqui, no hay que ponerlos a mano en la escena.
        PlayLoop(music, musicVolume);
        PlayLoop(ambiance, ambianceVolume);
    }

    // Loop en 2D: se oye igual en toda la escena, sin importar donde este el jugador.
    private void PlayLoop(AudioClip clip, float volume)
    {
        if (clip == null)
            return;

        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.clip = clip;
        source.loop = true;
        source.volume = volume;
        source.spatialBlend = 0f;
        source.playOnAwake = false;
        source.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (!timerRunning) return;
        timePassed += Time.deltaTime;

        if (time > 0)
        {
            time -= Time.deltaTime;
            //Debug.Log(time);
        }
        else
        {
            time = 0;
            timerRunning = false;
            OnTimerEnd();
        }

        if(timePassed > actualTimeBetweenGames)
        {
            actualTimeBetweenGames = timeBetweenGames;
            //Mala programacion, increiblemente estupido y bien brute force yo lo se. Es importante que el rango sea la cantidad de bools que tenemos en el SO de minigame triggers mas 1
            randomNumber = Random.Range(0, 6);

            switch (randomNumber)
            {
                case 0:
                    if (!triggers.slappyHand)
                    {
                        triggers.slappyHand = true;
                        Debug.Log(triggers.slappyHand + " Slappy hand");
                    }
                    break;
                case 1:
                    if (!triggers.fuseBox)
                    {
                        triggers.fuseBox = true;
                        Debug.Log(triggers.fuseBox + " fuseBox");
                    }
                    break;
                case 2:
                    if (!triggers.valveRotate)
                    {
                        triggers.valveRotate = true;
                        Debug.Log(triggers.valveRotate + " valveRotate");
                    }
                    break;
                case 3:
                    if (!triggers.pushLever)
                    {
                        triggers.pushLever = true;
                        Debug.Log(triggers.pushLever + " pushLever");
                    }
                    break;
                case 4:
                    if (!triggers.powerButton)
                    {
                        triggers.powerButton = true;
                        Debug.Log(triggers.powerButton + " powerButton");
                    }
                    break;
                case 5:
                    if (!triggers.simonSays)
                    {
                        triggers.simonSays = true;
                        Debug.Log(triggers.simonSays + " simonSays");
                        actualTimeBetweenGames = timeBetweenGames * 1.5f;
                    }
                    break;
                default:
                    break;
            }

            timePassed -= actualTimeBetweenGames;
        }

    }
    private void OnTimerEnd()
    {
        if (triggers.slappyHand)
        {
            gamesLeft += 1;
        }
        if (triggers.fuseBox)
        {
            gamesLeft += 1;
        }
        if (triggers.valveRotate)
        {
            gamesLeft += 1;
        }
        if (triggers.powerButton)
        { 
            gamesLeft += 1; 
        }
        if (triggers.pushLever)
        {
            gamesLeft += 1;
        }
        if (triggers.simonSays)
        {
            gamesLeft += 1;
        }
        triggers.slappyHand = false;
        triggers.fuseBox = false;
        triggers.valveRotate = false;
        triggers.pushLever = false;
        triggers.powerButton = false;
        triggers.simonSays = false;
        if (gamesLeft > 3)
        {
            SceneManager.LoadScene("Lose");
        }
        else
        {
            SceneManager.LoadScene("Win");
        }
    }
}
