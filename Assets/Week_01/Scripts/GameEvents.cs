using UnityEngine;
using System;

public class GameEvents : MonoBehaviour
{
    public static GameEvents Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public event Action<int> OnButtonTrigger;

    

    public void ButtonTrigger(int triggerID)
    {
        OnButtonTrigger?.Invoke(triggerID);
    }

}
