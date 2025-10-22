using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    public DoorScript door;

    public int buttonID;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            Debug.Log("Button number " + buttonID + " pressed!");
            GameEvents.Instance.ButtonTrigger(buttonID);
    }
}
