using System.Collections;
using TMPro;
using UnityEngine;

public class EscapeManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TMP_Text goodbyeText;
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
        
        canvasGroup.alpha = 0;
        goodbyeText.alpha = 0;
    }

    #region Buttons

    public void DeleteAllSongsBtn()
    {
        songManager.RemoveAllSongs();
    }

    public void QuitApplicationBtn()
    {
        StartCoroutine(GoodbyeRoutine());
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

    private IEnumerator GoodbyeRoutine()
    {
        const float _MAX_TIME = 1.5f;
        float _timer = 0;

        while (_timer < _MAX_TIME)
        {
            _timer += Time.deltaTime;
            float _t = _timer / _MAX_TIME;

            canvasGroup.alpha = Mathf.Lerp(0, 1, _t);
            yield return null;
        }

        canvasGroup.alpha = 1;
        
        yield return new WaitForSeconds(_MAX_TIME/2);

        _timer = 0;

        while (_timer < _MAX_TIME)
        {
            _timer += Time.deltaTime;
            float _t = _timer / _MAX_TIME;

            goodbyeText.alpha = Mathf.Lerp(0, 1, _t);
            yield return null;
        }
        goodbyeText.alpha = 1;
        yield return new WaitForSeconds(_MAX_TIME/2);
        
        Debug.LogWarning("If you are not in the Editor the application should have closed.");
        Application.Quit();

        canvasGroup.alpha = 0;
        goodbyeText.alpha = 0;
        yield return null;
    }
}
