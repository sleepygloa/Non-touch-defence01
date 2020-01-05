using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer
{
    public string ID = string.Empty;

    public bool isEnd = false;

    System.Action callback = null;

    float targetTime = 0.0f;
    float accumTime = 0.0f;

    float firstDelayTime = 0.0f;

    bool loop = false;

    public Timer()
    {
        ID = System.Guid.NewGuid().ToString();
    }

    public string Once(float targetTime, System.Action callback, float firstDelay = 0.0f)
    {
        this.isEnd = false;
        this.accumTime = 0.0f;
        this.targetTime = targetTime;
        this.callback = callback;
        this.loop = false;
        this.firstDelayTime = firstDelay;

        return ID;
    }

    public string Repeat(float targetTime, System.Action callback, float firstDelay = 0.0f)
    {
        this.isEnd = false;
        this.accumTime = 0.0f;
        this.targetTime = targetTime;
        this.callback = callback;
        this.loop = true;
        this.firstDelayTime = firstDelay;

        return ID;
    }

    public void Stop()
    {
        this.isEnd = true;
        this.accumTime = 0.0f;
        this.targetTime = 0.0f;
        this.callback = null;
        this.firstDelayTime = 0.0f;
    }

    public void OnUpdate()
    {
        if (this.isEnd == true)
        {
            return;
        }

        accumTime += Time.deltaTime;
        if (firstDelayTime > 0.0f)
        {
            if (firstDelayTime <= accumTime)
            {
                firstDelayTime = 0.0f;
                accumTime = 0.0f;
                callback.Invoke();
            }
            else
            {
                return;
            }
        }

        if (targetTime <= accumTime)
        {
            this.isEnd = true;
            if (callback != null)
            {
                callback.Invoke();
            }
            if (this.loop == true)
            {
                isEnd = false;
                accumTime = 0.0f;
            }
        }


    }


}
