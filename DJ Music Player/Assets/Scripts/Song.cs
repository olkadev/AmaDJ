using System.Collections;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Song : MonoBehaviour
{
    [Header("Text References")]
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text authorText;
    [SerializeField] private TMP_Text tagText;
    [SerializeField] private TMP_Text durationText;

    [Header("Input Field References")] 
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private TMP_InputField authorInputField;
    [SerializeField] private TMP_InputField tagInputField;

    [Header("Button References")]
    [SerializeField] private Button editButton;
    [SerializeField] private Button deleteButton;

    private bool isBtnActive = false;
    
    private Music music;
    private MusicPlayer musicPlayer;
    private AudioClip audioClip;
    private SongManager songManager;
    private int songIndex;

    public int GetSongIndex()
    {
        return songIndex;
    }
    public void SetSongIndex(int _index)
    {
        songIndex = _index;
    }
    
    public AudioClip AudioClip
    {
        get => audioClip;
        set => audioClip = value;
    }
    public Music Music
    {
        get => music;
        set => music = value;
    }

    public void SongManagerInit(SongManager _songManager)
    {
        songManager = _songManager;
    }
    
    private void Awake()
    {
        musicPlayer = FindFirstObjectByType<MusicPlayer>();
    }

    private void Start()
    {
        if (!musicPlayer) return;
        StartCoroutine(WaitForMusicClipRoutine());
    }

    #region Buttons
    
    public void PlayNowButton()
    {
        if (musicPlayer.IsFading || musicPlayer.Music == music) return;
        if (musicPlayer.Music == null)
        {
            musicPlayer.InitSongMusic(music);
            musicPlayer.StartMusic(music);
        }
        else
        {
            musicPlayer.InitNextSongMusic(music);
            StartCoroutine(musicPlayer.Crossfade(music.MusicClip));
        }
    }
    
    public void QueueUpButton()
    {
        if (musicPlayer.IsLooping) return;
        musicPlayer.InitNextSongMusic(music);
    }

    public void EditBtn()
    {
        if (isBtnActive)
        {
            ToggleEditMode(false);
            editButton.GetComponentInChildren<TMP_Text>().text = "Edit";
            
            if (!string.IsNullOrEmpty(nameInputField.text))
            {
                music.NameText = nameInputField.text;
            }
            music.AuthorText = authorInputField.text;
            music.TagText = tagInputField.text;

            UpdateSongInformation();
            SaveSystem.EditMusic(music, songIndex);
        }
        else
        {
            ToggleEditMode(true);
            editButton.GetComponentInChildren<TMP_Text>().text = "Confirm";
            nameInputField.text = nameText.text;
            authorInputField.text = authorText.text;
            tagInputField.text = tagText.text;
        }
    }

    public void DeleteBtn()
    {
        if (musicPlayer.Music == music) return;
        string _currentFilePath = Path.Combine(Music.MusicClipPath);
        File.Delete(_currentFilePath);
        _currentFilePath = Path.Combine(Application.persistentDataPath, songIndex.ToString("000") + ".mdat");
        File.Delete(_currentFilePath);

        int _totalMusic = PlayerPrefs.GetInt("TotalMusic");
        PlayerPrefs.SetInt("TotalMusic", _totalMusic - 1);
        
        songManager.RemoveSong(songIndex);
        Destroy(this.gameObject); 
    }
    
    #endregion

    public void DeleteButtonToggle()
    {
        deleteButton.interactable = !deleteButton.interactable;
    }
    
    private void ToggleEditMode(bool _state)
    {
        nameInputField.gameObject.SetActive(_state);
        authorInputField.gameObject.SetActive(_state);
        tagInputField.gameObject.SetActive(_state);
            
        nameText.gameObject.SetActive(!_state);
        authorText.gameObject.SetActive(!_state);
        tagText.gameObject.SetActive(!_state);
        isBtnActive = _state;
    }
    
    private void UpdateSongInformation()
    {
        nameText.text = music.NameText;
        authorText.text = music.AuthorText ?? null;
        tagText.text = music.TagText ?? null;
    }
    
    private IEnumerator WaitForMusicClipRoutine()
    {
        yield return new WaitUntil(() => music.MusicClip);
        UpdateSongInformation();

        float _maxMinute = Mathf.FloorToInt(music.MusicClip.length / 60f);
        float _maxSecond = Mathf.FloorToInt(music.MusicClip.length % 60f);
        durationText.text = $"{_maxMinute}:{_maxSecond.ToString("00")}";
    }
}
