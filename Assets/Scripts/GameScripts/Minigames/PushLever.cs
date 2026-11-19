using UnityEngine;

public class PushLever : MonoBehaviour
{
    public MinigameTriggers Trigger;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Lever"))
        {
            if (Trigger.pushLever)
            {
                Trigger.pushLever = false;
                Debug.Log(Trigger.pushLever + "pushLever");
            }
        }
    }
}
