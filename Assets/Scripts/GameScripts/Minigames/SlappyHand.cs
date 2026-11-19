using UnityEngine;

public class SlappyHand : MonoBehaviour
{
    public MinigameTriggers Trigger;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("PlayerHand"))
        {
            if (Trigger.slappyHand)
            {
                Trigger.slappyHand = false;
                Debug.Log(Trigger.slappyHand);
            }
        }
    }
}
