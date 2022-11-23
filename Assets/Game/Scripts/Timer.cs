using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public float time;
    public float duration;
    public bool isRunning;
    public bool isCompleted;

    // Update is called once per frame
    void Update()
    {
        if (isRunning && time < duration)
        {
            time += Time.deltaTime;
        }
        else
        {
            if (isRunning)
            {
                isCompleted = true;
            }
            StopTimer();
        }
    }

    public void StartTimer(float duration)
    {
        this.duration = duration;
        if (!isRunning)
        {
            isRunning = true;
        }
    }

    public void StopTimer()
    {
        isRunning = false;
        time = 0;
    }
}
