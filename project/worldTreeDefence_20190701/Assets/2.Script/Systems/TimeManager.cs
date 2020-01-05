using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : SingletonMonobehaviour<TimeManager>
{
    public Dictionary<string, Timer> timerList = new Dictionary<string, Timer>();
    public float AccumTime = 0.0f;
    public int AccumFrame = 0;

    public void AddTimer(Timer timer)
    {
        if(timerList.ContainsKey(timer.ID) == false)
        {
            timerList.Add(timer.ID, timer);
        }
    }
    public void RemoveTimer(string ID)
    {
        if(timerList.ContainsKey(ID) == true)
        {
            timerList[ID].Stop();
            timerList.Remove(ID);
        }
    }
    public void RemoveTimer(Timer timer)
    {
        RemoveTimer(timer.ID);
    }
	public void UpdateTime()
    {
        AccumTime += Time.deltaTime;
        AccumFrame++;

        if(timerList.Count > 0)
        {
            List<Timer> timers = new List<Timer>(timerList.Values);
            foreach(Timer timer in timers)
            {
                timer.OnUpdate();
            }
        }
    }

}
