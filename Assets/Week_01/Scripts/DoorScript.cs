using Unity.VisualScripting;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    public int doorID;

    
    private bool doorOpened;
    
    private void OnEnable()
    {
        GameEvents.Instance.OnButtonTrigger += TriggerDoor;
    }
    private void OnDisable()
    {
        GameEvents.Instance.OnButtonTrigger -= TriggerDoor;
    }

    private void TriggerDoor(int triggerID)
    {
        if (doorID == triggerID)
            if (!doorOpened)
                {
                    OpenDoor();
                }
                else
                {
                    CloseDoor();
                } 
        
    }

    private void OpenDoor()
    {
        transform.position += Vector3.up * 3;
        doorOpened = true;
        Debug.Log("Door " + doorID + " was opened.");
    }

    private void CloseDoor()
    {
        transform.position -= Vector3.up * 3;
        doorOpened = false;
        Debug.Log("Door " + doorID + " was closed.");
    }

}
