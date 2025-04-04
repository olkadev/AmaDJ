using System;
using System.Collections;
using System.IO;
using SFB;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class SaveSong : MonoBehaviour
{
    [Header("Text References")] 
    [SerializeField] private TMP_Text songPathText;
    [SerializeField] private TMP_Text errorText;
    [Header("Input Field References")]
    [SerializeField] private TMP_InputField nameTextInputField;
    [SerializeField] private TMP_InputField authorTextInputField;
    [SerializeField] private TMP_InputField tagTextInputField;

    private readonly ExtensionFilter[] _extensions = new []
    {
        new ExtensionFilter("Music Files", "mp3", "ogg", "wav")
    };
    private AudioClip _audioClip;
    private SongManager _songManager;

    private static int _totalMusic;
    
    private string _sourcePath;
    private string _destinationPath;
    private int _audioTypeId;
    
    private int _index;

    private void Awake()
    {
        _songManager = FindFirstObjectByType<SongManager>();
    }

    private void Start()
    {
        _totalMusic = PlayerPrefs.GetInt("TotalMusic");
    }

    #region Buttons
    
    public void ImportMusicFileBtn()
    {
        StartCoroutine(CountNumberOfFilesRoutine());
    }
    
    public void SaveMusicBtn()
    {
        if (!_songManager)
        {
            Debug.LogError("No SongManager in Scene!");
            return;
        }
        if (string.IsNullOrEmpty(_sourcePath))
        {
            errorText.text = ErrorMessages.NoMusicFileSelected;
            return;
        }
        if (string.IsNullOrEmpty(nameTextInputField.text))
        {
            errorText.text = ErrorMessages.NameRequired;
            return;
        }
        
        errorText.text = string.Empty;
        
        SaveMusic();
    }
    
    #endregion

    // Triggered by the button
    private void SaveMusic()
    {
        Music music = new Music();
        
        CopyMusicFileToDestination();
        music.MusicClipPath = _destinationPath;
        music.AudioTypeId = _audioTypeId;
        
        music.NameText = nameTextInputField.text.Trim();
        music.AuthorText = !authorTextInputField ? null : authorTextInputField.text.Trim();
        music.TagText = !tagTextInputField ? null : tagTextInputField.text.Trim();
        SaveSystem.SaveMusic(music);
        
        _songManager.AddSong(_destinationPath, _index);
        _totalMusic = PlayerPrefs.GetInt("TotalMusic");
        PlayerPrefs.SetInt("TotalMusic", _totalMusic + 1);
        nameTextInputField.text = string.Empty;
        authorTextInputField.text = string.Empty;
        tagTextInputField.text = string.Empty;
        _sourcePath = string.Empty;
        songPathText.text = "File Path is Empty";
    }
    
    private void CopyMusicFileToDestination()
    {
        try
        {
            File.Copy(_sourcePath, _destinationPath, false);
            Debug.Log("Music file saved to: " + _destinationPath);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to save Music file: " + e.Message);
        }
    }
    
    private IEnumerator CountNumberOfFilesRoutine()
    {
        // Opens File Browser
        var path = StandaloneFileBrowser.OpenFilePanel("", "", _extensions, false);
        if (path.Length == 0 || string.IsNullOrEmpty(path[0]))
        {
            errorText.text = ErrorMessages.NoFileSelected;
            songPathText.text = "File Path is Empty";
            this._sourcePath = null;
            yield break;
        }

        var sourcePath = path[0];

        // Assigns _audioTypeID
        if (sourcePath.Contains(".mp3"))
        {
            StartCoroutine(HandleAudioClipRoutine(sourcePath, AudioType.MPEG));
            _audioTypeId = 1;
        }
        else if (sourcePath.Contains(".ogg"))
        {
            StartCoroutine(HandleAudioClipRoutine(sourcePath, AudioType.OGGVORBIS));
            _audioTypeId = 2;
        }
        else if (sourcePath.Contains(".wav"))
        {
            StartCoroutine(HandleAudioClipRoutine(sourcePath, AudioType.WAV));
            _audioTypeId = 3;
        }
        else
        {
            Debug.LogWarning("You have imported the wrong AudioType");
            yield break;
        }
        
        int indexCounter = 0;
        bool isEnd = false;

        while (!isEnd)
        {
            string currentFilePath = Path.Combine(Application.persistentDataPath, indexCounter.ToString("000") + ".mdat");

            if (File.Exists(currentFilePath))
            {
                indexCounter++;
            }
            else
            {
                isEnd = true;
            }
        }
        _destinationPath = Path.Combine(Application.persistentDataPath, indexCounter.ToString("000") + ".mclp");
        this._sourcePath = path[0];
        songPathText.text = this._sourcePath;
        _index = indexCounter;
        
        errorText.text = string.Empty;
    }

    private IEnumerator HandleAudioClipRoutine(string sourcePath, AudioType audioType)
    {
        using UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + sourcePath, audioType);
        yield return www.SendWebRequest();
    
        if (www.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.LogWarning(www.error);
        }
        if (DownloadHandlerAudioClip.GetContent(www).length < 20f)
        {
            errorText.text = ErrorMessages.MinMusicFileLength;
            songPathText.text = "File Path is Empty";
            this._sourcePath = null;
        }
    }
}
