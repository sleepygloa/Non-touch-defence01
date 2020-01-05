using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;
public class SoundManager : SingletonMonobehaviour<SoundManager>
{
    public const string MasterGroupName = "Master";
    public const string EffectGroupName = "Effect";
    public const string BGMGroupName = "BGM";
    public const string UIGroupName = "UI";
    public const string MixerName = "AudioMixer";
    public const string ContainerName = "SoundContainer";
    public const string FadeA = "FadeA";
    public const string FadeB = "FadeB";
    public const string UI = "UI";
    public const string EffectVolumeParam = "Volume_Effect";
    public const string BGMVolumeParam = "Volume_BGM";
    public const string UIVolumeParam = "Volume_UI";

    public enum MusicPlayingType { None = 0, SourceA = 1, SourceB = 2, AtoB = 3, BtoA = 4}

    public AudioMixer mixer = null;
    public Transform audioRoot = null;
    public AudioSource fadeA_audio = null;
    public AudioSource fadeB_audio = null;
    public AudioSource[] effect_audios = null;
    public AudioSource UI_audio = null;

    public float[] effect_PlayStartTime = null;
    private int EffectChannelCount = 5;
    private MusicPlayingType currentPlayingType = MusicPlayingType.None;
    private bool isTicking = false;
    private SoundClip currentSound = null;
    private SoundClip lastSound = null;
    private bool nowMute = false;
    private float lastEffectVolume = 0.0f;
    private float lastUIVolume = 0.0f;
    private float lastBGMVolume = 0.0f;
    private float minVolume = -80.0f;
    private float maxVolume = 0.0f;


    private AudioListener audioListener = null;

    private void Start()
    {
        if(this.mixer == null)
        {
            this.mixer = ResourceManager.Load(MixerName) as AudioMixer;
        }
        if(this.audioRoot == null)
        {
            this.audioRoot = new GameObject(ContainerName).transform;
            this.audioRoot.SetParent(transform);
            this.audioRoot.localPosition = Vector3.zero;
        }
        if(this.fadeA_audio == null)
        {
            GameObject fadeA_GO = new GameObject(FadeA, typeof(AudioSource));
            fadeA_GO.transform.SetParent(this.audioRoot);
            this.fadeA_audio = fadeA_GO.GetComponent<AudioSource>();
            this.fadeA_audio.playOnAwake = false;
        }
        if(this.fadeB_audio == null)
        {
            GameObject fadeB_GO = new GameObject(FadeB, typeof(AudioSource));
            fadeB_GO.transform.SetParent(this.audioRoot);
            this.fadeB_audio = fadeB_GO.GetComponent<AudioSource>();
            this.fadeB_audio.playOnAwake = false;
        }
        if(this.UI_audio == null)
        {
            GameObject UI_GO = new GameObject(UI, typeof(AudioSource));
            UI_GO.transform.SetParent(this.audioRoot);
            this.UI_audio = UI_GO.GetComponent<AudioSource>();
            this.UI_audio.playOnAwake = false;
        }
        if(this.effect_audios == null)
        {
            this.effect_PlayStartTime = new float[EffectChannelCount];
            this.effect_audios = new AudioSource[EffectChannelCount];
            for(int i = 0; i < EffectChannelCount; i++)
            {
                this.effect_PlayStartTime[i] = 0.0f;
                GameObject Effect_GO = new GameObject("Effect_" + i.ToString(), typeof(AudioSource));
                Effect_GO.transform.SetParent(this.audioRoot);
                this.effect_audios[i] = Effect_GO.GetComponent<AudioSource>();
                this.effect_audios[i].playOnAwake = false;
            }
        }
        if(this.mixer != null)
        {
            //
            this.fadeA_audio.outputAudioMixerGroup = mixer.FindMatchingGroups(BGMGroupName)[0];
            this.fadeB_audio.outputAudioMixerGroup = mixer.FindMatchingGroups(BGMGroupName)[0];

            this.UI_audio.outputAudioMixerGroup = mixer.FindMatchingGroups(UIGroupName)[0];
            for(int i = 0; i < this.effect_audios.Length;i++)
            {
                this.effect_audios[i].outputAudioMixerGroup = mixer.FindMatchingGroups(EffectGroupName)[0];
            }
        }
        if(this.audioListener == null)
        {
            this.audioListener = gameObject.AddComponent<AudioListener>();
        }

        //볼륨초기화.
        this.VolumeInit();
    }

    public void SetBGMVolume(float currentRatio)//ui Slider 0 ~ 1.
    {
        currentRatio = Mathf.Clamp01(currentRatio);//0~1로 세팅.

        float volume = Mathf.Lerp(minVolume, maxVolume, currentRatio);
        this.mixer.SetFloat(BGMVolumeParam, volume);
        PlayerPrefs.SetFloat(BGMVolumeParam, volume);
    
    }
    public float GetBGMVolume()
    {
        if(PlayerPrefs.HasKey(BGMVolumeParam) == true)
        {
            return PlayerPrefs.GetFloat(BGMVolumeParam);
        }
        else
        {
            return maxVolume;
        }
    }

    public void SetEffectVolume(float currentRatio)
    {
        currentRatio = Mathf.Clamp01(currentRatio);
        float volume = Mathf.Lerp(minVolume, maxVolume, currentRatio);
        this.mixer.SetFloat(EffectVolumeParam, volume);
        PlayerPrefs.SetFloat(EffectVolumeParam, volume);
    }
    public float GetEffectVolume()
    {
        if(PlayerPrefs.HasKey(EffectVolumeParam) == true)
        {
            return PlayerPrefs.GetFloat(EffectVolumeParam);
        }else
        {
            return maxVolume;
        }
    }

    public void SetUIVolume(float currentRatio)
    {
        currentRatio = Mathf.Clamp01(currentRatio);
        float volume = Mathf.Lerp(minVolume, maxVolume, currentRatio);
        this.mixer.SetFloat(UIVolumeParam, volume);
        PlayerPrefs.SetFloat(UIVolumeParam, volume);
    }
    public float GetUIVolume()
    {
        if(PlayerPrefs.HasKey(UIVolumeParam) == true)
        {
            return PlayerPrefs.GetFloat(UIVolumeParam);
        }
        else
        {
            return maxVolume;
        }

    }
    /// <summary>
    /// 볼륨을 초기화합니다.
    /// </summary>
    void VolumeInit()
    {
        if(this.mixer != null)
        {
            this.mixer.SetFloat(BGMVolumeParam, GetBGMVolume());
            this.mixer.SetFloat(EffectVolumeParam, GetEffectVolume());
            this.mixer.SetFloat(UIVolumeParam, GetUIVolume());
        }
    }

    void PlayAudioSource(AudioSource source, SoundClip clip, float volume)
    {
        if(source == null || clip == null)
        {
            Debug.LogWarning("Plz check this PlayAudioSource");
            return;
        }
        source.Stop();
        source.clip = clip.GetClip();
        source.volume = volume;
        source.loop = clip.IsLoop;
        source.pitch = clip.Pitch;
        source.dopplerLevel = clip.DopplerLevel;
        source.rolloffMode = clip.audioRolloffMode;
        source.minDistance = clip.MinDistance;
        source.maxDistance = clip.MaxDistance;
        source.spatialBlend = clip.SpatialBlend;
        source.Play();
    }
    /// <summary>
    /// 현재 사운드를 재생중입니까?
    /// </summary>
    public bool IsPlaying()
    {
        return (int)this.currentPlayingType > 0;
    }
    public bool IsDifferentSound(SoundClip clip)
    {
        if(clip == null)
        {
            return false;
        }
        if(this.currentSound != null && this.currentSound.RealID == clip.RealID && 
            IsPlaying() && this.currentSound.IsFadeOut == false)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private IEnumerator CheckProcess()
    {
        while(this.isTicking == true && IsPlaying() == true)
        {
            yield return new WaitForSeconds(0.05f);
            if(this.currentSound.HasLoop())
            {
                switch(this.currentPlayingType)
                {
                    case MusicPlayingType.SourceA:
                        this.currentSound.CheckLoop(this.fadeA_audio);
                        break;
                    case MusicPlayingType.SourceB:
                        this.currentSound.CheckLoop(this.fadeB_audio);
                        break;
                    case MusicPlayingType.AtoB:
                        this.lastSound.CheckLoop(this.fadeA_audio);
                        this.currentSound.CheckLoop(this.fadeB_audio);
                        break;
                    case MusicPlayingType.BtoA:
                        this.lastSound.CheckLoop(this.fadeB_audio);
                        this.currentSound.CheckLoop(this.fadeA_audio);
                        break;
                }
            }
        }
    }
    public void DoCheck()
    {
        StartCoroutine(CheckProcess());
    }

    public void FadeIn(SoundClip clip , float time, Interpolate.EaseType ease)
    {
        if(this.IsDifferentSound(clip) == true)
        {
            this.fadeA_audio.Stop();
            this.fadeB_audio.Stop();
            this.lastSound = this.currentSound;
            this.currentSound = clip;

            this.PlayAudioSource(this.fadeA_audio, this.currentSound, 0.0f);

            this.currentSound.FadeIn(time, ease);
            this.currentPlayingType = MusicPlayingType.SourceA;
            if(this.currentSound.HasLoop() == true)
            {
                this.isTicking = true;
                DoCheck();
            }
        }
    }

    public void FadeIn(int index, float time, Interpolate.EaseType ease)
    {
        this.FadeIn(DataManager.Sound().GetCopy(index), time, ease);
    }

    public void FadeOut(float time, Interpolate.EaseType ease)
    {
        if(this.currentSound != null)
        {
            this.currentSound.FadeOut(time, ease);
        }
    }

    private void Update()
    {
        if(this.currentSound == null)
        {
            return;
        }
        
        switch(this.currentPlayingType)
        {
            case MusicPlayingType.SourceA:
                this.currentSound.DoFade(Time.deltaTime, this.fadeA_audio);
                break;
            case MusicPlayingType.SourceB:
                this.currentSound.DoFade(Time.deltaTime, this.fadeB_audio);
                break;
            case MusicPlayingType.AtoB:
                this.lastSound.DoFade(Time.deltaTime, this.fadeA_audio);
                this.currentSound.DoFade(Time.deltaTime, this.fadeB_audio);
                break;
            case MusicPlayingType.BtoA:
                this.lastSound.DoFade(Time.deltaTime, this.fadeB_audio);
                this.currentSound.DoFade(Time.deltaTime, this.fadeA_audio);
                break;

        }

        if(this.fadeA_audio.isPlaying == true && this.fadeB_audio.isPlaying == false)
        {
            this.currentPlayingType = MusicPlayingType.SourceA;
        }else if(this.fadeB_audio.isPlaying == true && this.fadeA_audio.isPlaying == false)
        {
            this.currentPlayingType = MusicPlayingType.SourceB;
        }else if(this.fadeA_audio.isPlaying == false && this.fadeB_audio.isPlaying == false)
        {
            this.currentPlayingType = MusicPlayingType.None;
        }

    }

    public void FadeTo(SoundClip clip , float time, Interpolate.EaseType ease)
    {
        if(this.currentPlayingType == MusicPlayingType.None)
        {
            this.FadeIn(clip, time, ease);
        }else if(this.IsDifferentSound(clip) == true)
        {
            if(this.currentPlayingType == MusicPlayingType.AtoB)
            {
                this.fadeA_audio.Stop();
                this.currentPlayingType = MusicPlayingType.SourceB;
            }else if(this.currentPlayingType == MusicPlayingType.BtoA)
            {
                this.fadeB_audio.Stop();
                this.currentPlayingType = MusicPlayingType.SourceA;
            }

            //fade to
            this.lastSound = this.currentSound;
            this.currentSound = clip;
            this.lastSound.FadeOut(time, ease);
            this.currentSound.FadeIn(time, ease);
            if(this.currentPlayingType == MusicPlayingType.SourceA)
            {
                PlayAudioSource(fadeB_audio, this.currentSound, 0.0f);
                this.currentPlayingType = MusicPlayingType.AtoB;
            }else if(this.currentPlayingType == MusicPlayingType.SourceB)
            {
                PlayAudioSource(fadeA_audio, this.currentSound, 0.0f);
                this.currentPlayingType = MusicPlayingType.BtoA;
            }
            if(this.currentSound.HasLoop())
            {
                this.isTicking = true;
                DoCheck();
            }

        }
    }

    public void FadeTo(int index, float time, Interpolate.EaseType ease)
    {
        this.FadeTo(DataManager.Sound().GetCopy(index), time, ease);
    }

    //client api
    public void PlayBGM(SoundClip clip)
    {
        if(this.IsDifferentSound(clip) == true)
        {
            this.fadeB_audio.Stop();
            this.lastSound = this.currentSound;
            this.currentSound = clip;
            PlayAudioSource(this.fadeA_audio, clip, 1.0f);
            if(this.currentSound.HasLoop() == true)
            {
                this.isTicking = true;
                DoCheck();
            }
        }
    }
    public void PlayBGM(int index)
    {
        PlayBGM(DataManager.Sound().GetCopy(index));
    }

    public void PlayUISound(SoundClip clip)
    {
        PlayAudioSource(this.UI_audio, clip, 1.0f);
    }

    public void PlayEffectSound(SoundClip clip)
    {
        bool isPlaySuccess = false;

        for(int i = 0; i < this.EffectChannelCount;i++)
        {
            //빈 사운드 채널이 있따면.
            if(this.effect_audios[i].isPlaying == false)
            {
                PlayAudioSource(this.effect_audios[i], clip, 0.0f);
                this.effect_PlayStartTime[i] = Time.realtimeSinceStartup;
                isPlaySuccess = true;
                break;
            }
            else if(this.effect_audios[i].clip == clip.GetClip())
            {
                this.effect_audios[i].Stop();
                PlayAudioSource(this.effect_audios[i], clip, 0.0f);
                this.effect_PlayStartTime[i] = Time.realtimeSinceStartup;
                isPlaySuccess = true;
                break;
            }
        }

        if(isPlaySuccess == false)
        {
            float maxTime = 0.0f;
            int selectIndex = 0;
            for(int i =0; i < this.EffectChannelCount;i++)
            {
                if(this.effect_PlayStartTime[i] > maxTime)
                {
                    maxTime = this.effect_PlayStartTime[i];
                    selectIndex = i;
                }
            }
            PlayAudioSource(this.effect_audios[selectIndex], clip, 0.0f);
        }
    }

    public void PlayOneShot(SoundClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("Play One shot param clip is null!!");
            return;
        }
        switch(clip.PlayType)
        {
            case SoundPlayType.EFFECT:
                PlayEffectSound(clip);
                break;
            case SoundPlayType.BGM:
                PlayBGM(clip);
                break;
            case SoundPlayType.UI:
                PlayUISound(clip);
                break;
        }
    }

    public void PlayOneShot(int index)
    {
        if(index == (int)SoundList.None)
        {
            return;
        }
        this.PlayOneShot(DataManager.Sound().GetCopy(index));
    }
    //BGM STOP
    public void BGMStop(bool allStop = false)
    {
        if(allStop == true)
        {
            this.fadeA_audio.Stop();
            this.fadeB_audio.Stop();
        }
        this.FadeOut(0.5f, Interpolate.EaseType.Linear);
        this.currentPlayingType = MusicPlayingType.None;
        this.isTicking = false;
        StopAllCoroutines();
    }

}
