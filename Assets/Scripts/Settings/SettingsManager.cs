using TMPro;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField crossFadeInputField;
    [SerializeField] private TMP_InputField fadeInputField;
    
    private SongManager _songManager;

    private void Awake()
    {
        _songManager = FindFirstObjectByType<SongManager>();
    }

    private void Start()
    {
        crossFadeInputField.text = MusicPlayer.CrossFadeTime.ToString();
        fadeInputField.text = MusicPlayer.FadeTime.ToString();
    }

    #region Buttons

    public void DeleteAllSongsBtn()
    {
        _songManager.RemoveAllSongs();
    }

    #endregion
    
    #region Input Fields
    
    public void ModifyCrossFadeTime()
    {
        if (string.IsNullOrEmpty(crossFadeInputField.text)) return;
        if (float.TryParse(crossFadeInputField.text, out float crossFadeTime) && crossFadeTime < 16)
        {
            MusicPlayer.CrossFadeTime = crossFadeTime;
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
        if (float.TryParse(fadeInputField.text, out float fadeTime) && fadeTime < 16)
        {
            MusicPlayer.FadeTime = fadeTime;
        }
        else
        {
            Debug.LogError("Invalid input for FadeTime. Using default value.");
            fadeInputField.text = "3";
            MusicPlayer.FadeTime = 3f;
        }
        PlayerPrefs.SetFloat("FadeTime", MusicPlayer.FadeTime);
    }
    
    #endregion
}
