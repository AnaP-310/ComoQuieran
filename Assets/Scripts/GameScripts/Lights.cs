using UnityEngine;

public class Lights : MonoBehaviour
{

    public MinigameTriggers triggers;
    public GameObject buttonLight;
    public GameObject fuseBoxLight;
    public GameObject slappLight;
    public GameObject valveLight;
    public GameObject simonLight;
    public GameObject leverLight;

    // Update is called once per frame
    void Update()
    {
        if (triggers.slappyHand)
        {
            slappLight.SetActive(true);
        }
        else
        {
            slappLight.SetActive(false);
        }

        if (triggers.powerButton)
        {
            buttonLight.SetActive(true);
        }
        else
        {
            buttonLight.SetActive(false);
        }

        if (triggers.fuseBox)
        {
            fuseBoxLight.SetActive(true);
        }
        else
        {
            fuseBoxLight.SetActive(false);
        }

        if (triggers.valveRotate)
        {
            valveLight.SetActive(true);
        }
        else
        {
            valveLight.SetActive(false);
        }

        if (triggers.pushLever)
        {
            leverLight.SetActive(true);
        }
        else
        {
            leverLight.SetActive(false);
        }

        if (triggers.simonSays)
        {
            simonLight.SetActive(true);
        }
        else
        {
            simonLight.SetActive(false);
        }
    }
}
