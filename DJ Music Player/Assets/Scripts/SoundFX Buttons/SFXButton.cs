using TMPro;
using UnityEngine;

public class SFXButton : MonoBehaviour
{
    [Header("Sound Effect Prefab")]
    [SerializeField] private GameObject soundEffectPrefab;
    [Header("Scriptable Object Reference")]
    [SerializeField] private SoundEffectSO soundEffectSO;
    [Header("Text References")]
    [SerializeField] private TMP_Text nameText;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = soundEffectPrefab.GetComponent<AudioSource>();
    }

    private void Start()
    {
        nameText.text = soundEffectSO.SoundName;
    }

    public void PlayButtonSound()
    {
        audioSource.clip = soundEffectSO.SoundClip;
        Instantiate(soundEffectPrefab);
    }
}
