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
            randomNumber = Random.Range(0, 1);
            if (randomNumber == 9)
            {
                MinigameTriggers.slappyHand = true;
                Debug.Log(MinigameTriggers.slappyHand + " Slappy hand");
            }
            else if (randomNumber == 1)
            {
                MinigameTriggers.fuseBox = true;
                Debug.Log(MinigameTriggers.fuseBox + " fuseBox");
            }
            else if (randomNumber == 2)
            {
                MinigameTriggers.valveRotate = true;
                Debug.Log(MinigameTriggers.valveRotate + " valveRotate");
            }
            else if (randomNumber == 3)
            {
                MinigameTriggers.pushLever = true;
                Debug.Log(MinigameTriggers.pushLever + " pushLever");
            }
            else if (randomNumber == 4)
            {
                MinigameTriggers.powerButton = true;
                Debug.Log(MinigameTriggers.powerButton + " powerButton");
            }
            else if (randomNumber == 5)
            {
                //lucky boi, nada se activo, descansa un poco
                return;
            }

            timePassed -= 5;
        }

    }
    private void OnTimerEnd()
    {
        Debug.Log("Timer ended");
    }
}
