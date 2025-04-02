using System.Collections;
using TMPro;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField crossFadeInputField;
    [SerializeField] private TMP_InputField fadeInputField;
    
    private SongManager songManager;

    private void Awake()
    {
        songManager = FindFirstObjectByType<SongManager>();
    }

    private void Start()
    {
        crossFadeInputField.text = MusicPlayer.CrossFadeTime.ToString();
        fadeInputField.text = MusicPlayer.FadeTime.ToString();
        
        
    }

    #region Buttons

    public void DeleteAllSongsBtn()
    {
        songManager.RemoveAllSongs();
    }

    #endregion
    
    public void ModifyCrossFadeTime()
    {
        if (string.IsNullOrEmpty(crossFadeInputField.text)) return;
        if (float.TryParse(crossFadeInputField.text, out float _crossFadeTime) && _crossFadeTime < 16)
        {
            MusicPlayer.CrossFadeTime = _crossFadeTime;
        }
        else
        {
            Debug.LogError("Invalid input for CrossFadeTime. Using default value.");
            crossFadeInputField.text = "6";
            MusicPlayer.CrossFadeTime = 6f;
        }
        
        PlayerPrefs.SetFloat("CrossFadeTime", MusicPlayer.CrossFadeTime);
    }

    public void ModifyFadeTime()
    {
        if (string.IsNullOrEmpty(fadeInputField.text)) return;
        if (float.TryParse(fadeInputField.text, out float _fadeTime) && _fadeTime < 16)
        {
            MusicPlayer.FadeTime = _fadeTime;
        }
        else
        {
            Debug.LogError("Invalid input for FadeTime. Using default value.");
            fadeInputField.text = "3";
            MusicPlayer.FadeTime = 3f;
        }
        
        PlayerPrefs.SetFloat("FadeTime", MusicPlayer.FadeTime);
    }
}
