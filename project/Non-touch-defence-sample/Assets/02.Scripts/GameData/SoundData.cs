using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml;
using System.IO;

public class SoundData : BaseData
{
    public SoundClip[] soundClips = new SoundClip[0];

    public string ClipPath = "Sound/";

    private string xmlFilePath = string.Empty;
    private string xmlFileName = "soundData.xml";
    private string dataPath = "Data/soundData";

    private const string SOUND = "sound";
    private const string CLIP = "clip";
    private const string LOOPS = "loops";
    private const string MAXVOL = "maxvol";
    private const string PITCH = "pitch";
    private const string DOPPLERLEVEL = "dopplerLevel";
    private const string ROLLOFFMODE = "rolloffmode";
    private const string MINDISTANCE = "mindistance";
    private const string MAXDISTANCE = "maxdistance";
    private const string SPATIALBLEND = "spatialBlend";
    private const string LOOP = "loop";
    private const string CLIPPATH = "clipPath";
    private const string CLIPNAME = "clipName";
    private const string CHECKTIMECOUNT = "checktimecount";
    private const string CHECKTIME = "checktime";
    private const string SETTIMECOUNT = "settimecount";
    private const string SETTIME = "settime";
    private const string TYPE = "type";

    public void SaveData()
    {

        using (XmlTextWriter xml = new XmlTextWriter(xmlFilePath + xmlFileName, System.Text.Encoding.Unicode))
        {
            xml.WriteStartDocument();
            xml.WriteStartElement(SOUND);
            xml.WriteElementString(LENGTH, this.names.Length.ToString());
            xml.WriteWhitespace(NEWLINE);

            for (int i = 0; i < this.names.Length; i++)
            {
                SoundClip clip = this.soundClips[i];
                xml.WriteStartElement(CLIP);

                xml.WriteElementString(ID, i.ToString());
                xml.WriteElementString(NAME, this.names[i]);
                xml.WriteElementString(LOOPS, clip.CheckTime.Length.ToString());
                xml.WriteElementString(MAXVOL, clip.MaxVolume.ToString());
                xml.WriteElementString(PITCH, clip.Pitch.ToString());
                xml.WriteElementString(DOPPLERLEVEL, clip.DopplerLevel.ToString());
                xml.WriteElementString(ROLLOFFMODE, clip.audioRolloffMode.ToString());
                xml.WriteElementString(MINDISTANCE, clip.MinDistance.ToString());
                xml.WriteElementString(MAXDISTANCE, clip.MaxDistance.ToString());
                xml.WriteElementString(SPATIALBLEND, clip.SpatialBlend.ToString());
                if (clip.IsLoop == true)
                {
                    xml.WriteElementString(LOOP, "true");
                }
                xml.WriteElementString(CLIPPATH, clip.ClipPath);
                xml.WriteElementString(CLIPNAME, clip.ClipName);
                xml.WriteElementString(CHECKTIMECOUNT, clip.CheckTime.Length.ToString());

                string times = string.Empty;
                foreach (float t in clip.CheckTime)
                {
                    times += t.ToString() + "/";
                }
                xml.WriteElementString(CHECKTIME, times);

                times = string.Empty;
                foreach (float t in clip.SetTime)
                {
                    times += t.ToString() + "/";
                }
                xml.WriteElementString(SETTIME, times);

                xml.WriteElementString(TYPE, clip.PlayType.ToString());

                xml.WriteEndElement();
            }

            xml.WriteEndElement();
            xml.WriteEndDocument();
        }
    }

    void SetLoopTime(bool isCheck, SoundClip clip, string times)
    {
        if (times == string.Empty)
        {
            return;
        }
        string timeString = times; //    3.0f/10.0f/13.0f
        string[] time = timeString.Split('/');
        for (int i = 0; i < time.Length; i++)
        {
            if (isCheck == true)
            {
                clip.CheckTime[i] = float.Parse(time[i]);
            }
            else
            {
                clip.SetTime[i] = float.Parse(time[i]);
            }
        }
    }

    public void AddSound(string name, string clipPath = "", string clipName = "")
    {
        if (this.names == null)
        {
            this.names = new string[] { name };
            this.soundClips = new SoundClip[] { new SoundClip(clipPath, clipName) };
        }
        else
        {
            this.names = ArrayHelper.Add(name, this.names);
            this.soundClips = ArrayHelper.Add<SoundClip>(new SoundClip(), this.soundClips);
        }
    }

    public void LoadData()
    {

        this.xmlFilePath = Application.dataPath + Define.DataDirectory;

        TextAsset asset = (TextAsset)Resources.Load(this.dataPath, typeof(TextAsset));

        Debug.Log(this.xmlFilePath + this.xmlFileName);


        if (asset == null || asset.text == null)
        {
            AddSound("NewSound");
            return;
        }

        using (XmlTextReader reader = new XmlTextReader(new StringReader(asset.text)))
        {
            int currentID = 0;
            while (reader.Read())
            {
                if (reader.IsStartElement())
                {
                    switch (reader.Name)
                    {
                        case LENGTH:
                            int length = int.Parse(reader.ReadString());
                            this.names = new string[length];
                            this.soundClips = new SoundClip[length];
                            break;
                        case ID:
                            currentID = int.Parse(reader.ReadString());
                            this.soundClips[currentID] = new SoundClip
                            {
                                RealID = currentID
                            };
                            break;
                        case NAME:
                            this.names[currentID] = reader.ReadString();
                            break;
                        case LOOPS:
                            int count = int.Parse(reader.ReadString());
                            this.soundClips[currentID].CheckTime = new float[count];
                            this.soundClips[currentID].SetTime = new float[count];
                            break;
                        case MAXVOL:
                            this.soundClips[currentID].MaxVolume = float.Parse(reader.ReadString());
                            break;
                        case PITCH:
                            this.soundClips[currentID].Pitch = float.Parse(reader.ReadString());
                            break;
                        case DOPPLERLEVEL:
                            this.soundClips[currentID].DopplerLevel = float.Parse(reader.ReadString());
                            break;
                        case ROLLOFFMODE:
                            this.soundClips[currentID].audioRolloffMode = (AudioRolloffMode)Enum.Parse(typeof(AudioRolloffMode), reader.ReadString());
                            break;
                        case MINDISTANCE:
                            this.soundClips[currentID].MinDistance = float.Parse(reader.ReadString());
                            break;
                        case MAXDISTANCE:
                            this.soundClips[currentID].MaxDistance = float.Parse(reader.ReadString());
                            break;
                        case SPATIALBLEND:
                            this.soundClips[currentID].SpatialBlend = float.Parse(reader.ReadString());
                            break;
                        case LOOP:
                            this.soundClips[currentID].IsLoop = true;
                            break;
                        case CLIPPATH:
                            this.soundClips[currentID].ClipPath = reader.ReadString();
                            break;
                        case CLIPNAME:
                            this.soundClips[currentID].ClipName = reader.ReadString();
                            break;
                        case TYPE:
                            this.soundClips[currentID].PlayType = (SoundPlayType)Enum.Parse(typeof(SoundPlayType), reader.ReadString());
                            break;
                        case CHECKTIME:
                            SetLoopTime(true, this.soundClips[currentID], reader.ReadString());
                            break;
                        case SETTIME:
                            SetLoopTime(false, this.soundClips[currentID], reader.ReadString());
                            break;
                    }
                }
            }
        }

        foreach(SoundClip clip in this.soundClips)
        {
            clip.PreLoad();
        }

    }

    public override void RemoveData(int index)
    {
        this.names = ArrayHelper.Remove(index, this.names);
        if(this.names.Length == 0)
        {
            this.names = null;
        }
        this.soundClips = ArrayHelper.Remove<SoundClip>(index, this.soundClips);
    }

    public void ClearData()
    {
        foreach(SoundClip clip in this.soundClips)
        {
            if(clip.GetClip() != null)
            {
                clip.ReleaseClip();
            }
        }
        this.soundClips = new SoundClip[0];
        this.names = null;
    }

    public SoundClip GetCopy(int index)
    {
        if(index < 0 || index >= this.soundClips.Length)
        {
            return null;
        }

        SoundClip newClip = new SoundClip();
        newClip.RealID = index;
        newClip.ClipPath = this.soundClips[index].ClipPath;
        newClip.ClipName = this.soundClips[index].ClipName;
        newClip.MaxVolume = this.soundClips[index].MaxVolume;
        newClip.Pitch = this.soundClips[index].Pitch;
        newClip.DopplerLevel = this.soundClips[index].DopplerLevel;
        newClip.audioRolloffMode = this.soundClips[index].audioRolloffMode;
        newClip.MinDistance = this.soundClips[index].MinDistance;
        newClip.MaxDistance = this.soundClips[index].MaxDistance;
        newClip.SpatialBlend = this.soundClips[index].SpatialBlend;
        newClip.IsLoop = this.soundClips[index].IsLoop;
        newClip.CheckTime = new float[this.soundClips[index].CheckTime.Length];
        newClip.SetTime = new float[this.soundClips[index].SetTime.Length];
        for(int i = 0; i < newClip.CheckTime.Length;i++)
        {
            newClip.CheckTime[i] = this.soundClips[index].CheckTime[i];
            newClip.SetTime[i] = this.soundClips[index].SetTime[i];
        }
        newClip.PreLoad();
        return newClip;
    }

    public override void CopyData(int index)
    {
        this.names = ArrayHelper.Add(this.names[index], this.names);
        this.soundClips = ArrayHelper.Add<SoundClip>(GetCopy(index), this.soundClips);
    }


}
