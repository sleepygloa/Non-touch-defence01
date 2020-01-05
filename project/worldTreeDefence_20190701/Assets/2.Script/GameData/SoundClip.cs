using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundClip
{
    public SoundPlayType PlayType = SoundPlayType.None;
    public int RealID = 0;
    //audio file... ogg. wav.
    private AudioClip clip = null;
    public string ClipName = string.Empty;
    public string ClipPath = string.Empty;

    public float MaxVolume = 1.0f;

    //구간 반복을 위한 변수입니다.
    public bool IsLoop = false;
    public float[] CheckTime = new float[0];
    public float[] SetTime = new float[0];
    public int CurrentLoop = 0;

    public float Pitch = 1.0f;
    public float DopplerLevel = 1.0f;
    public AudioRolloffMode audioRolloffMode = AudioRolloffMode.Logarithmic;
    public float MinDistance = 10000.0f;
    public float MaxDistance = 50000.0f;
    public float SpatialBlend = 1.0f;

    public float FadeTime1 = 0.0f;
    public float FadeTime2 = 0.0f;
    public Interpolate.Function Interpolatefunction;
    public bool IsFadeIn = false;
    public bool IsFadeOut = false;

    public SoundClip() { }
    public SoundClip(string clipPath, string clipName)
    {
        this.ClipPath = clipPath;
        this.ClipName = clipName;

        string pathLower = this.ClipPath.ToLower();

        if(pathLower.Contains("bgm") == true)
        {
            this.PlayType = SoundPlayType.BGM;
        }else if(pathLower.Contains("effect") == true)
        {
            this.PlayType = SoundPlayType.EFFECT;
        }else if(pathLower.Contains("ui") == true)
        {
            this.PlayType = SoundPlayType.UI;
        }else
        {
            Debug.LogWarning("Can not Find Type :" + ClipPath);
            this.PlayType = SoundPlayType.None;
        }
    }

    public void PreLoad()
    {
        if(this.clip == null)
        {
            string fullPath = this.ClipPath + this.ClipName;
            this.clip = ResourceManager.Load(fullPath) as AudioClip;
            if(this.clip == null)
            {
                Debug.LogWarning("Preload AudioClip Load Failed : " + fullPath);
            }
        }
    }

    public AudioClip GetClip()
    {
        if(this.clip == null)
        {
            PreLoad();
        }
        return this.clip;
    }

    public void ReleaseClip()
    {
        if(this.clip != null)
        {
            this.clip = null;
        }
    }

    //반복기능.
    public void AddLoop()
    {
        this.CheckTime = ArrayHelper.Add(0.0f, this.CheckTime);
        this.SetTime = ArrayHelper.Add(0.0f, this.SetTime);
    }
    public void RemoveLoop(int index)
    {
        this.CheckTime = ArrayHelper.Remove(index, this.CheckTime);
        this.SetTime = ArrayHelper.Remove(index, this.SetTime);
    }
    //반복구간을 가지고 있는가?.
    public bool HasLoop()
    {
        return this.CheckTime.Length > 0;
    }
    //다음 반복구간으로 이동합니다.
    public void NextLoop()
    {
        this.CurrentLoop++;
        if(this.CurrentLoop >= this.CheckTime.Length)
        {
            this.CurrentLoop = 0;
        }
    }
    public void CheckLoop(AudioSource source)
    {
        //반복구간을 가지고 있고, 체크 타임보다 더 재생이 되었다면 세팅타임으로 시간을 돌립니다.
        if(HasLoop() == true && source.time >= this.CheckTime[this.CurrentLoop])
        {
            source.time = this.SetTime[CurrentLoop];
            this.NextLoop();
        }
    }
    //fade 기능.
    public void FadeIn(float time, Interpolate.EaseType easeType)
    {
        this.IsFadeOut = false;
        this.IsFadeIn = true;
        this.FadeTime1 = 0.0f;
        this.FadeTime2 = time;
        this.Interpolatefunction = Interpolate.Ease(easeType);
    }
    public void FadeOut(float time, Interpolate.EaseType easeType)
    {
        this.IsFadeOut = true;
        this.IsFadeIn = false;
        this.FadeTime1 = 0.0f;
        this.FadeTime2 = time;
        this.Interpolatefunction = Interpolate.Ease(easeType);
    }
    public void DoFade(float time, AudioSource audio)
    {
        if(this.IsFadeIn == true)
        {
            this.FadeTime1 += time;
            audio.volume = Interpolate.Ease(this.Interpolatefunction, 0, this.MaxVolume, this.FadeTime1, this.FadeTime2);
            if(this.FadeTime1 >= this.FadeTime2)
            {
                this.IsFadeIn = false;
            }
        }else if(this.IsFadeOut == true)
        {
            this.FadeTime1 += time;
            audio.volume = Interpolate.Ease(this.Interpolatefunction, this.MaxVolume, 0 - this.MaxVolume, this.FadeTime1,
                this.FadeTime2);
            if(this.FadeTime1 >= this.FadeTime2)
            {
                this.IsFadeOut = false;
                audio.Stop();
            }
        }
    }


}
