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

    private ExtensionFilter[] extensions = new []
    {
        new ExtensionFilter("Music Files", "mp3", "ogg", "wav")
    };
    private AudioClip audioClip;
    private SongManager songManager;

    private string sourcePath;
    private string destinationPath;
    private int audioTypeId;

    private static int totalMusic;
    
    private int index;

    private void Awake()
    {
        songManager = FindFirstObjectByType<SongManager>();
    }

    private void Start()
    {
        totalMusic = PlayerPrefs.GetInt("TotalMusic");
    }

    private void Update()
    {
        
        if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.O))
        {
            totalMusic--;
            Debug.Log($"Decreased totalMusic to: {totalMusic}");
            PlayerPrefs.SetInt("TotalMusic", totalMusic);                                                            //Fuck you if I have to use this one more time
        }
        else if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.P))
        {
            totalMusic++;
            Debug.Log($"Increased totalMusic to: {totalMusic}");
            PlayerPrefs.SetInt("TotalMusic", totalMusic);   
        }
    }

    public void ImportMusicFileBtn()
    {
        
        StartCoroutine(CountNumberOfFilesRoutine());
    }
    
    public void SaveMusicBtn()
    {
        if (!songManager)
        {
            Debug.LogError("No SongManager in Scene!");
            return;
        }
        if (string.IsNullOrEmpty(sourcePath))
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
        
        Music _music = new Music();
        
        CopyMusicFileToDestination();
        _music.MusicClipPath = destinationPath;
        _music.AudioTypeId = audioTypeId;
        
        _music.NameText = nameTextInputField.text;
        _music.AuthorText = !authorTextInputField ? null : authorTextInputField.text;;
        _music.TagText = !tagTextInputField ? null : tagTextInputField.text;
        SaveSystem.SaveMusic(_music);
        
        songManager.AddSong(destinationPath, index);
        totalMusic = PlayerPrefs.GetInt("TotalMusic");
        PlayerPrefs.SetInt("TotalMusic", totalMusic + 1);
        nameTextInputField.text = string.Empty;
        authorTextInputField.text = string.Empty;
        tagTextInputField.text = string.Empty;
        sourcePath = string.Empty;
        songPathText.text = "File Path is Empty";
    }

    private void CopyMusicFileToDestination()
    {
        try
        {
            File.Copy(sourcePath, destinationPath, false);
            Debug.Log("Music file saved to: " + destinationPath);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to save Music file: " + e.Message);
        }
    }
    
    private IEnumerator CountNumberOfFilesRoutine()
    {
        var _path = StandaloneFileBrowser.OpenFilePanel("", "", extensions, false);
        if (_path.Length == 0 || string.IsNullOrEmpty(_path[0]))
        {
            errorText.text = ErrorMessages.NoFileSelected;
            songPathText.text = "File Path is Empty";
            sourcePath = null;
            yield break;
        }

        var _sourcePath = _path[0];

        if (_sourcePath.Contains(".mp3"))
        {
            StartCoroutine(HandleAudioClipRoutine(_sourcePath, AudioType.MPEG));
            audioTypeId = 1;
        }
        else if (_sourcePath.Contains(".ogg") || _sourcePath.Contains(".flac"))
        {
            StartCoroutine(HandleAudioClipRoutine(_sourcePath, AudioType.OGGVORBIS));
            audioTypeId = 2;
        }
        else if (_sourcePath.Contains(".wav"))
        {
            StartCoroutine(HandleAudioClipRoutine(_sourcePath, AudioType.WAV));
            audioTypeId = 3;
        }
        else
        {
            Debug.LogWarning("You have imported the wrong AudioType");
            yield break;
        }
        
        int _indexCounter = 0;
        bool _isEnd = false;

        while (!_isEnd)
        {
            string _currentFilePath = Path.Combine(Application.persistentDataPath, _indexCounter.ToString("000") + ".mdat");

            if (File.Exists(_currentFilePath))
            {
                _indexCounter++;
            }
            else
            {
                _isEnd = true;
            }
        }

        destinationPath = Path.Combine(Application.persistentDataPath, _indexCounter.ToString("000") + ".mclp");
        sourcePath = _path[0];
        songPathText.text = sourcePath;
        index = _indexCounter;
        
        errorText.text = string.Empty;
    }

    private IEnumerator HandleAudioClipRoutine(string _sourcePath, AudioType _audioType)
    {
        using UnityWebRequest _www = UnityWebRequestMultimedia.GetAudioClip("file://" + _sourcePath, _audioType);
        yield return _www.SendWebRequest();
    
        if (_www.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.LogWarning(_www.error);
        }
        if (DownloadHandlerAudioClip.GetContent(_www).length < 20f)
        {
            errorText.text = ErrorMessages.MinMusicFileLength;
            songPathText.text = "File Path is Empty";
            sourcePath = null;
        }
    }
}
