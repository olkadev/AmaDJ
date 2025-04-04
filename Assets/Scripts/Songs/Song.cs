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

    private bool _isBtnActive;

    private MusicPlayer _musicPlayer;
    private SongManager _songManager;
    private int _songIndex;

    public int GetSongIndex()
    {
        return _songIndex;
    }
    public void SetSongIndex(int index)
    {
        _songIndex = index;
    }
    
    public AudioClip AudioClip { get; set; }

    public Music Music { get; set; }

    public void SongManagerInit(SongManager songManager)
    {
        this._songManager = songManager;
    }
    
    private void Awake()
    {
        _musicPlayer = FindFirstObjectByType<MusicPlayer>();
    }

    private void Start()
    {
        if (!_musicPlayer) return;
        StartCoroutine(WaitForMusicClipRoutine());
    }

    #region Buttons
    
    public void PlayNowButton()
    {
        if (_musicPlayer.IsFading) return;
        if (_musicPlayer.Music == null)
        {
            _musicPlayer.InitSongMusic(Music);
            _musicPlayer.StartMusic(Music);
        }
        else
        {
            _musicPlayer.InitNextSongMusic(Music);
            StartCoroutine(_musicPlayer.Crossfade(Music.MusicClip));
        }
    }
    
    public void QueueUpButton()
    {
        if (_musicPlayer.IsLooping || _musicPlayer.IsFading) return;
        _musicPlayer.InitNextSongMusic(Music);
    }

    public void EditBtn()
    {
        if (_isBtnActive)
        {
            ToggleEditMode(false);
            
            if (!string.IsNullOrEmpty(nameInputField.text))
            {
                Music.NameText = nameInputField.text;
            }
            Music.AuthorText = authorInputField.text;
            Music.TagText = tagInputField.text;

            UpdateSongInformation();
            SaveSystem.EditMusic(Music, _songIndex);
        }
        else
        {
            ToggleEditMode(true);
            nameInputField.text = nameText.text.Trim();
            authorInputField.text = authorText.text.Trim();
            tagInputField.text = tagText.text.Trim();
        }
    }

    public void DeleteBtn()
    {
        if (_musicPlayer.Music == Music) return;
        string currentFilePath = Path.Combine(Music.MusicClipPath);
        File.Delete(currentFilePath);
        currentFilePath = Path.Combine(Application.persistentDataPath, _songIndex.ToString("000") + ".mdat");
        File.Delete(currentFilePath);

        int totalMusic = PlayerPrefs.GetInt("TotalMusic");
        PlayerPrefs.SetInt("TotalMusic", totalMusic - 1);
        
        _songManager.RemoveSong(_songIndex);
        Destroy(this.gameObject); 
    }
    
    #endregion

    public void DeleteButtonToggle()
    {
        deleteButton.interactable = !deleteButton.interactable;
    }
    
    private void ToggleEditMode(bool state)
    {
        SoundButtonManager.IgnoreSoundEffectHotkeys = state;
        MusicPlayer.IgnorePauseHotkey = state;
        nameInputField.gameObject.SetActive(state);
        authorInputField.gameObject.SetActive(state);
        tagInputField.gameObject.SetActive(state);
            
        nameText.gameObject.SetActive(!state);
        authorText.gameObject.SetActive(!state);
        tagText.gameObject.SetActive(!state);
        _isBtnActive = state;
    }
    
    private void UpdateSongInformation()
    {
        nameText.text = Music.NameText;
        authorText.text = Music.AuthorText;
        tagText.text = Music.TagText;
    }
    
    private IEnumerator WaitForMusicClipRoutine()
    {
        yield return new WaitUntil(() => Music.MusicClip);
        UpdateSongInformation();

        float maxMinute = Mathf.FloorToInt(Music.MusicClip.length / 60f);
        float maxSecond = Mathf.FloorToInt(Music.MusicClip.length % 60f);
        durationText.text = $"{maxMinute}:{maxSecond.ToString("00")}";
    }
}
