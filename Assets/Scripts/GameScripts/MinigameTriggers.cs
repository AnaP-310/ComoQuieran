using UnityEngine;

[CreateAssetMenu(fileName = "MinigameTriggers", menuName = "Scriptable Objects/MinigameTriggers")]
public class MinigameTriggers : ScriptableObject
{
    public bool slapHand = false;
    public bool fuseBox = false;
    public bool valveRotate = false;
    public bool pushLever = false;
    public bool powerButton = false;
}
