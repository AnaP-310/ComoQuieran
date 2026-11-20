using UnityEngine;

public class Valve : MonoBehaviour
{
    public MinigameTriggers trigger;
    public HingeJoint joint;
    public float rotationValue;


    void Update()
    {
        // Track the current rotation of the valve on the axis it can rotate on
        rotationValue = joint.angle;

        // If the rotation value is within a certain range, run the event
        if (rotationValue >=150  && rotationValue <= 180)
        {
            trigger.valveRotate = false;
        }
    }
}
