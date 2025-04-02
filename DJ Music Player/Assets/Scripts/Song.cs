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

    private MusicPlayer musicPlayer;
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
    
    public AudioClip AudioClip { get; set; }

    public Music Music { get; set; }

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
        if (musicPlayer.IsFading) return;
        if (musicPlayer.Music == null)
        {
            musicPlayer.InitSongMusic(Music);
            musicPlayer.StartMusic(Music);
        }
        else
        {
            musicPlayer.InitNextSongMusic(Music);
            StartCoroutine(musicPlayer.Crossfade(Music.MusicClip));
        }
    }
    
    public void QueueUpButton()
    {
        if (musicPlayer.IsLooping || musicPlayer.IsFading) return;
        musicPlayer.InitNextSongMusic(Music);
    }

    public void EditBtn()
    {
        if (isBtnActive)
        {
            ToggleEditMode(false);
            
            if (!string.IsNullOrEmpty(nameInputField.text))
            {
                Music.NameText = nameInputField.text;
            }
            Music.AuthorText = authorInputField.text;
            Music.TagText = tagInputField.text;

            UpdateSongInformation();
            SaveSystem.EditMusic(Music, songIndex);
        }
        else
        {
            ToggleEditMode(true);
            nameInputField.text = nameText.text;
            authorInputField.text = authorText.text;
            tagInputField.text = tagText.text;
        }
    }

    public void DeleteBtn()
    {
        if (musicPlayer.Music == Music) return;
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
        nameText.text = Music.NameText;
        authorText.text = Music.AuthorText ?? null;
        tagText.text = Music.TagText ?? null;
    }
    
    private IEnumerator WaitForMusicClipRoutine()
    {
        yield return new WaitUntil(() => Music.MusicClip);
        UpdateSongInformation();

        float _maxMinute = Mathf.FloorToInt(Music.MusicClip.length / 60f);
        float _maxSecond = Mathf.FloorToInt(Music.MusicClip.length % 60f);
        durationText.text = $"{_maxMinute}:{_maxSecond.ToString("00")}";
    }
}
