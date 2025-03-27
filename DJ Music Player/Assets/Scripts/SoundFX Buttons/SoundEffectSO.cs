using UnityEngine;

[CreateAssetMenu(fileName = "Sound Effect", menuName = "Scriptable Objects/Sound Effect")]
public class SoundEffectSO : ScriptableObject
{
    [Header("Sound Button Effect Settings")]
    [SerializeField] private AudioClip soundClip;
    [SerializeField] private string soundName;

    public AudioClip SoundClip => soundClip;
    public string SoundName => soundName;
}
