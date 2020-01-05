using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BGMSoundPlayer))]
public class BGMSoundPlayerInspector : BaseInspector
{
    public override void OnInspectorGUI()
    {
        BGMSoundPlayer player = (BGMSoundPlayer)target;
        GUILayout.Label("Music Setup", EditorStyles.boldLabel);
        this.EventStartSettings(player);
        EditorGUILayout.Separator();

        player.playType = (MusicPlayType)EditorGUILayout.EnumPopup("Play Type", player.playType);
        if(player.playType.Equals(MusicPlayType.FADE_OUT) == false && 
            player.playType.Equals(MusicPlayType.STOP) == false)
        {
            player.musicClip_Index1 = EditorGUILayout.Popup("Music Clip", player.musicClip_Index1,
                DataManager.Sound().GetNameList(true));
        }

        if(player.playType.Equals(MusicPlayType.PLAY) == false && 
            player.playType.Equals(MusicPlayType.STOP) == false)
        {
            player.fadeTime = EditorGUILayout.FloatField("Fade Time (s)", player.fadeTime);
            player.easeType = (Interpolate.EaseType)EditorGUILayout.EnumPopup("Interpolation", player.easeType);
        }

        EditorGUILayout.Separator();
        this.VariableSettings(player);
        EditorGUILayout.Separator();
        if(GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }

}
