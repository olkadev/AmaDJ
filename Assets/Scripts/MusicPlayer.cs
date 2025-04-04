using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MusicPlayer : MonoBehaviour
{
    [Header("Audio Source Reference")]
    [SerializeField] private AudioSource currentMusicSource;
    [SerializeField] private AudioSource nextMusicSource;
    [Header("UI References")]
    [SerializeField] private TMP_Text musicNameText;
    [SerializeField] private TMP_Text nextMusicNameText;
    [SerializeField] private TMP_Text musicTimeText;
    [SerializeField] private Button loopButton;
    [SerializeField] private Button autoplayButton;
    [SerializeField] private TMP_InputField autoplayInputField;
    [Header("Slider References")]
    [SerializeField] private Slider musicBar;
    [SerializeField] private Slider fadeBar;
    [Header("Customization")]
    public static float CrossFadeTime;
    public static float FadeTime;

    private Music _music;
    private Music _nextMusic;
    private Music _cachedMusic;
    private SongManager _songManager;

    private List<Song> _autoplaySongs = new List<Song>();
    
    public static bool IgnorePauseHotkey;
    private bool _isFading;
    private bool _isPaused;
    private bool _isLooping;
    private bool _isAutoplaying;
    private bool _hasAutoplaySong;
    
    private float _currentSecond;
    private float _currentMinute;
    private float _maxSecond;
    private float _maxMinute;


    #region Getters & Setters
    
    public bool IsPaused => _isPaused;
    public bool IsFading => _isFading;
    public bool IsLooping => _isLooping;
    public Music Music => _music;

    #endregion
    
    #region Inits

    public void InitSongMusic(Music music)
    {
        this._music = music;
    }
    public void InitNextSongMusic(Music nextMusic)
    {
        this._nextMusic = nextMusic;
    }

    #endregion

    private void Awake()
    {
        _songManager = FindFirstObjectByType<SongManager>();
    }

    private void Start()
    {
        CrossFadeTime = PlayerPrefs.GetFloat("CrossFadeTime");
        FadeTime = PlayerPrefs.GetFloat("FadeTime");
        
        if (PlayerPrefs.GetFloat("CrossFadeTime") == 0)
        {
            CrossFadeTime = 6;
        }
        if (PlayerPrefs.GetFloat("FadeTime") == 0)
        {
            FadeTime = 3;
        }
    }

    private void Update()
    {
        HotkeyHandler();
        UpdateQueueText();
        UpdateMusicDuration();
        ChooseAutoplaySong();
        HandleQueued();
    }
    
    public void SearchTag()
    {
        foreach (var song in _songManager.Songs)
        {
            if (autoplayInputField.text.Trim() != string.Empty && string.Equals(song.Music.TagText, autoplayInputField.text.Trim(), StringComparison.InvariantCultureIgnoreCase))
            {
                _autoplaySongs.Add(song);
                _hasAutoplaySong = true;
                Debug.Log($"{song.GetSongIndex()} id added to autoplaySongs");
            }
        }

        if (!_hasAutoplaySong)
        {
            _autoplaySongs.Clear();
        }
        _hasAutoplaySong = false;
    }
    
    public void StartMusic(Music music)
    {
        if (music == null) return;
        this._music = music;
        currentMusicSource.clip = this._music.MusicClip;
        StartCoroutine(StartFadeIn());
    }

    #region Buttons

    public void ToggleMusic()
    {
        if (_isFading || _music == null) return;
        _isPaused = !_isPaused;
        StartCoroutine(currentMusicSource.isPlaying ? FadeOut() : FadeIn());
    }

    public void SkipToQueued()
    {
        if (_nextMusic == null || _isFading) return;
        StartCoroutine(Crossfade(_nextMusic.MusicClip));
    }
    
    public void ToggleAutoplay()
    {
        _isAutoplaying = !_isAutoplaying;
    }
    
    public void ToggleLoop()
    {
        _isLooping = !_isLooping;
        autoplayButton.interactable = !_isLooping;
        
        if (_isLooping)
        {
            if (_isAutoplaying)
            {
                _isAutoplaying = false;
            }
            _nextMusic = null;
        }
    }

    #endregion

    #region Hotkeys
    
    private void HotkeyHandler()
    {
        if (IgnorePauseHotkey) return;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ToggleMusic();
        }

        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.RightArrow))
        {
            SkipToQueued();
        }
    }
    
    #endregion
    
    private void ChooseAutoplaySong()
    {
        if (!_isAutoplaying) return;
        if (_autoplaySongs.Count == 0 || _nextMusic != null || _isLooping) return;
        int randomIndex = Random.Range(0, _autoplaySongs.Count);
        if (_autoplaySongs.Count <= 1)
        {
            _nextMusic = _autoplaySongs[randomIndex].Music;
            return;
        }
        if (_autoplaySongs[randomIndex].Music == _music || (_autoplaySongs[randomIndex].Music == _cachedMusic && _autoplaySongs.Count > 2)) return;

        
        _cachedMusic = _music;
        
        _nextMusic = _autoplaySongs[randomIndex].Music;
    }
    
    private void HandleQueued()
    {
        if (!currentMusicSource.clip && !_isFading)
        {
            StartMusic(_nextMusic);
            return;
        }
        if (currentMusicSource.time > currentMusicSource.clip.length - CrossFadeTime)
        {
            if (_nextMusic == null && !currentMusicSource.isPlaying)
            {
                if (_isLooping)
                {
                    StartMusic(_music);
                }
                else
                {
                    _music = null;
                    currentMusicSource.clip = null;
                }
            }
            else if (_nextMusic != null && currentMusicSource.isPlaying && !_isFading)
            {
                StartCoroutine(Crossfade(_nextMusic.MusicClip));
            }
        }
    }
    
    private void UpdateQueueText()
    {
        if (_music == null)
        {
            musicNameText.text = $"Currently Playing: ";
        }
        else
        {
            musicNameText.text = $"Currently Playing: {_music.NameText}";
        }
        
        if (_isLooping && currentMusicSource.clip)
        {
            nextMusicNameText.text = $"Song is Looping";
        }
        else if (_nextMusic == null)
        {
            nextMusicNameText.text = $"Queue is Empty";
        }
        else
        {
            nextMusicNameText.text = $"Next in Queue: {_nextMusic.NameText}";
        }
    }

    private void UpdateMusicDuration()
    {
        if (currentMusicSource.clip)
        {
            _currentMinute = Mathf.FloorToInt(currentMusicSource.time / 60f);
            _currentSecond = Mathf.FloorToInt(currentMusicSource.time % 60f);
            _maxMinute = Mathf.FloorToInt(currentMusicSource.clip.length / 60f);
            _maxSecond = Mathf.FloorToInt(currentMusicSource.clip.length % 60f);
        }
        else
        {
            _currentMinute = 0;
            _currentSecond = 0;
            _maxMinute = 0;
            _maxSecond = 0;
        }
        
        if (currentMusicSource.clip)
        {
            musicBar.value = currentMusicSource.time / currentMusicSource.clip.length;
        }
        else
        {
            musicBar.value = 0;
        }
        musicTimeText.text = $"{_currentMinute}:{_currentSecond:00} / {_maxMinute}:{_maxSecond:00}";
    }

    #region FadeRoutines
    
    public IEnumerator Crossfade(AudioClip nextClip)
    {
        _isFading = true;
        nextMusicSource.clip = nextClip;
        nextMusicSource.Play();
        
        float timer = 0;
        while (timer < CrossFadeTime)
        {
            timer += Time.deltaTime;
            float t = timer / CrossFadeTime;

            fadeBar.value = Mathf.Lerp(0, 1, t);
            currentMusicSource.volume = Mathf.Lerp(1, 0, t);
            nextMusicSource.volume = Mathf.Lerp(0, 1, t);
            yield return null;
        }
        fadeBar.value = 0;
        
        _music = _nextMusic;
        
        float nextMusicTimestamp = nextMusicSource.time;
        
        currentMusicSource.volume = 0;
        nextMusicSource.volume = 1;
        currentMusicSource.Stop();
        nextMusicSource.Stop();
        
        currentMusicSource.clip = nextClip;
        currentMusicSource.Play();
        currentMusicSource.time += nextMusicTimestamp;
        currentMusicSource.volume = 1;
        
        _isFading = false;
        _nextMusic = null;
    }
    
    private IEnumerator FadeOut()
    {
        _isFading = true;
        float timer = 0;
        while (timer < FadeTime)
        {
            timer += Time.deltaTime;
            float t = timer / FadeTime;

            fadeBar.value = Mathf.Lerp(0, 1, t);
            currentMusicSource.volume = Mathf.Lerp(1, 0, t);

            yield return null;
        }
        fadeBar.value = 0;
        currentMusicSource.volume = 0;
        currentMusicSource.Pause();
        _isFading = false;
    }
    
    private IEnumerator FadeIn()
    {
        UpdateQueueText();
        _isFading = true;
        currentMusicSource.Play();
        float timer = 0;
        while (timer < FadeTime)
        {
            timer += Time.deltaTime;
            float t = timer / FadeTime;

            fadeBar.value = Mathf.Lerp(0, 1, t);
            currentMusicSource.volume = Mathf.Lerp(0, 1, t);

            yield return null;
        }
        fadeBar.value = 0;
        currentMusicSource.volume = 1;
        _isFading = false;
    }
    
    private IEnumerator StartFadeIn()
    {
        UpdateQueueText();
        _isFading = true;
        currentMusicSource.Play();
        float timer = 0;
        while (timer < FadeTime)
        {
            timer += Time.deltaTime;
            float t = timer / FadeTime;

            fadeBar.value = Mathf.Lerp(0, 1, t);
            currentMusicSource.volume = Mathf.Lerp(0, 1, t);

            yield return null;
        }

        fadeBar.value = 0;
        currentMusicSource.volume = 1;
        _isFading = false;
        _nextMusic = null;
    }
    
    #endregion
    
}
