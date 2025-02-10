using System;
using UnityEngine;

public class DevTools : MonoBehaviour
{
    [SerializeField] float timeScale = 1f;
    public float TimeScale
    {
        get => timeScale;
        set
        {
            if (value < 0) value = 0;
            if (value > 5) value = 5f;
            Time.timeScale = value;
        }
    }

    void Awake()
    {
        Time.timeScale = timeScale;
    }
}
