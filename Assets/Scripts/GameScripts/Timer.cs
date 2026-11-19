using UnityEngine;

public class Timer : MonoBehaviour
{
    public float time;
    private float timePassed = 0;
    private int randomNumber;
    private bool timerRunning = true;
    public MinigameTriggers MinigameTriggers;

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

        if(timePassed > 5)
        {
            //Mala programacion, increiblemente estupido y bien brute force yo lo se. Es importante que el rango sea la cantidad de bools que tenemos en el SO de minigame triggers mas 1
            randomNumber = Random.Range(0, 6);

            switch (randomNumber)
            {
                case 0:
                    MinigameTriggers.slappyHand = true;
                    Debug.Log(MinigameTriggers.slappyHand + " Slappy hand");
                    break;
                case 1:
                    MinigameTriggers.fuseBox = true;
                    Debug.Log(MinigameTriggers.fuseBox + " fuseBox");
                    break;
                case 2:
                    MinigameTriggers.valveRotate = true;
                    Debug.Log(MinigameTriggers.valveRotate + " valveRotate");
                    break;
                case 3:
                    MinigameTriggers.pushLever = true;
                    Debug.Log(MinigameTriggers.pushLever + " pushLever");
                    break;
                case 4:
                    MinigameTriggers.powerButton = true;
                    Debug.Log(MinigameTriggers.powerButton + " powerButton");
                    break;
                case 5:
                    //lucky boi, nada se activo, descansa un poco
                    break;
                default:
                    break;
            }

            timePassed -= 5;
        }

    }
    private void OnTimerEnd()
    {
        Debug.Log("Timer ended");
    }
}
