
using UnityEngine;

[CreateAssetMenu(fileName ="SfxName", menuName = "ScriptableObjects/AudioScriptableObject", order = 1)]
public class SfxScriptableObjects : ScriptableObject
{
    public string SfxName;
    public AudioClip sfxClip;
}
