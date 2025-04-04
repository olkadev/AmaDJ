using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SoundButton : MonoBehaviour
{
    [Header("Sound Effect Prefab")]
    [SerializeField] private GameObject soundEffectPrefab;
    [Header("Scriptable Object Reference")]
    [SerializeField] private SoundEffectSO soundEffectSO;
    [SerializeField] private TMP_Text nameText;

    private AudioSource _audioSource;
    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _audioSource = soundEffectPrefab.GetComponent<AudioSource>();
    }

    private void Start()
    {
        nameText.text = soundEffectSO.SoundName;
        _button.onClick.AddListener(PlaySound);
    }

    public void PlaySound()
    {
        _audioSource.clip = soundEffectSO.SoundClip;
        Instantiate(soundEffectPrefab);
    }
}
