using UnityEngine;

public class Timer : MonoBehaviour
{
    public float time;

    private bool timerRunning;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //if (!timerRunning) return;

        if (time > 0)
        {
            time -= Time.deltaTime;
            Debug.Log(time);
        }
        else
        {
            time = 0;
            timerRunning = false;
            OnTimerEnd();
        }

    }
    private void OnTimerEnd()
    {
        Debug.Log("Timer ended");
    }
}
