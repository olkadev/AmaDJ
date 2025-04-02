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

    private Music music;
    private Music nextMusic;
    private Music cachedMusic;
    private SongManager songManager;

    private List<Song> autoplaySongs = new List<Song>();
    
    private bool isFading;
    private bool isPaused;
    private bool isLooping;
    private bool isAutoplaying;
    private bool hasAutoplaySong;
    
    private float currentSecond;
    private float currentMinute;
    private float maxSecond;
    private float maxMinute;

    public bool IsPaused => isPaused;
    public bool IsFading => isFading;
    public bool IsLooping => isLooping;
    public Music Music => music;

    #region Inits

    public void InitSongMusic(Music _music)
    {
        music = _music;
    }
    public void InitNextSongMusic(Music _nextMusic)
    {
        nextMusic = _nextMusic;
    }

    #endregion

    private void Awake()
    {
        songManager = FindFirstObjectByType<SongManager>();
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
        // DEBUG
        if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.RightArrow))
        { 
            currentMusicSource.time = currentMusicSource.clip.length - 8f;
        }
        // DEBUG

        HotkeyHandler();
        
        UpdateQueueText();
        UpdateMusicDuration();
        ChooseAutoplaySong();
        HandleQueued();
    }
    
    public void SearchTag()
    {
        foreach (var _song in songManager.Songs)
        {
            if (string.Equals(_song.Music.TagText, autoplayInputField.text.Trim(), StringComparison.InvariantCultureIgnoreCase))
            {
                autoplaySongs.Add(_song);
                hasAutoplaySong = true;
                Debug.Log($"{_song.GetSongIndex()} id added to autoplaySongs");
            }
        }

        if (!hasAutoplaySong)
        {
            autoplaySongs.Clear();
        }
        hasAutoplaySong = false;
    }
    
    public void StartMusic(Music _music)
    {
        if (_music == null) return;
        music = _music;
        currentMusicSource.clip = music.MusicClip;
        StartCoroutine(StartFadeIn());
    }

    #region Buttons

    public void ToggleMusic()
    {
        if (isFading || music == null) return;
        isPaused = !isPaused;
        StartCoroutine(currentMusicSource.isPlaying ? FadeOut() : FadeIn());
    }

    public void SkipToQueued()
    {
        if (nextMusic == null || isFading) return;
        StartCoroutine(Crossfade(nextMusic.MusicClip));
    }
    
    public void ToggleAutoplay()
    {
        isAutoplaying = !isAutoplaying;
        if (isAutoplaying)
        {
            autoplayButton.Select();
        }
        else
        {
            autoplayButton.OnDeselect(null);
        }
    }
    
    public void ToggleLoop()
    {
        isLooping = !isLooping;
        autoplayButton.interactable = !isLooping;
        
        if (isLooping)
        {
            loopButton.Select();
            if (isAutoplaying)
            {
                isAutoplaying = false;
                nextMusic = null;
                autoplayButton.OnDeselect(null);
            }
        }
        else
        {
            loopButton.OnDeselect(null);
        }
    }

    #endregion

    #region Hotkeys
    
    private void HotkeyHandler()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ToggleMusic();
        }

        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.P))
        {
            SkipToQueued();
        }
    }
    
    #endregion
    
    private void ChooseAutoplaySong()
    {
        if (!isAutoplaying || autoplaySongs.Count == 0 || nextMusic != null || isLooping) return;
        int _randomIndex = Random.Range(0, autoplaySongs.Count);
        if (autoplaySongs.Count <= 1)
        {
            nextMusic = autoplaySongs[_randomIndex].Music;
            return;
        }
        if (autoplaySongs[_randomIndex].Music == music || (autoplaySongs[_randomIndex].Music == cachedMusic && autoplaySongs.Count > 2)) return;

        
        cachedMusic = music;
        
        nextMusic = autoplaySongs[_randomIndex].Music;
    }
    
    private void HandleQueued()
    {
        if (!currentMusicSource.clip && !isFading)
        {
            StartMusic(nextMusic);
            return;
        }
        if (currentMusicSource.time > currentMusicSource.clip.length - CrossFadeTime)
        {
            if (nextMusic == null && !currentMusicSource.isPlaying)
            {
                if (isLooping)
                {
                    StartMusic(music);
                }
                else
                {
                    music = null;
                    currentMusicSource.clip = null;
                }
            }
            else if (nextMusic != null && currentMusicSource.isPlaying && !isFading)
            {
                StartCoroutine(Crossfade(nextMusic.MusicClip));
            }
        }
    }
    
    private void UpdateQueueText()
    {
        if (music == null)
        {
            musicNameText.text = $"Currently Playing: ";
        }
        else
        {
            musicNameText.text = $"Currently Playing: {music.NameText}";
        }
        if (isLooping && currentMusicSource.clip)
        {
            nextMusicNameText.text = $"Song is Looping";
        }
        else if (nextMusic == null)
        {
            nextMusicNameText.text = $"Queue is Empty";
        }
        else
        {
            nextMusicNameText.text = $"Next in Queue: {nextMusic.NameText}";
        }
    }

    private void UpdateMusicDuration()
    {
        if (currentMusicSource.clip)
        {
            currentMinute = Mathf.FloorToInt(currentMusicSource.time / 60f);
            currentSecond = Mathf.FloorToInt(currentMusicSource.time % 60f);
            maxMinute = Mathf.FloorToInt(currentMusicSource.clip.length / 60f);
            maxSecond = Mathf.FloorToInt(currentMusicSource.clip.length % 60f);
        }
        else
        {
            currentMinute = 0;
            currentSecond = 0;
            maxMinute = 0;
            maxSecond = 0;
        }
        
        if (currentMusicSource.clip)
        {
            musicBar.value = currentMusicSource.time / currentMusicSource.clip.length;
        }
        else
        {
            musicBar.value = 0;
        }
        
        musicTimeText.text = $"{currentMinute}:{currentSecond:00} / {maxMinute}:{maxSecond:00}";
    }

    #region FadeRoutines
    
    public IEnumerator Crossfade(AudioClip _nextClip)
    {
        isFading = true;
        nextMusicSource.clip = _nextClip;
        nextMusicSource.Play();
        
        float _timer = 0;
        while (_timer < CrossFadeTime)
        {
            _timer += Time.deltaTime;
            float _t = _timer / CrossFadeTime;

            fadeBar.value = Mathf.Lerp(0, 1, _t);
            currentMusicSource.volume = Mathf.Lerp(1, 0, _t);
            nextMusicSource.volume = Mathf.Lerp(0, 1, _t);
            yield return null;
        }
        fadeBar.value = 0;
        
        music = nextMusic;
        
        float _nextMusicTimestamp = nextMusicSource.time;
        
        currentMusicSource.volume = 0;
        nextMusicSource.volume = 1;
        currentMusicSource.Stop();
        nextMusicSource.Stop();
        
        currentMusicSource.clip = _nextClip;
        currentMusicSource.Play();
        currentMusicSource.time += _nextMusicTimestamp;
        currentMusicSource.volume = 1;
        
        isFading = false;
        nextMusic = null;
    }
    
    private IEnumerator FadeOut()
    {
        isFading = true;
        float _timer = 0;
        while (_timer < FadeTime)
        {
            _timer += Time.deltaTime;
            float _t = _timer / FadeTime;

            fadeBar.value = Mathf.Lerp(0, 1, _t);
            currentMusicSource.volume = Mathf.Lerp(1, 0, _t);

            yield return null;
        }
        fadeBar.value = 0;
        currentMusicSource.volume = 0;
        currentMusicSource.Pause();
        isFading = false;
    }
    
    private IEnumerator FadeIn()
    {
        UpdateQueueText();
        isFading = true;
        currentMusicSource.Play();
        float _timer = 0;
        while (_timer < FadeTime)
        {
            _timer += Time.deltaTime;
            float _t = _timer / FadeTime;

            fadeBar.value = Mathf.Lerp(0, 1, _t);
            currentMusicSource.volume = Mathf.Lerp(0, 1, _t);

            yield return null;
        }
        fadeBar.value = 0;
        currentMusicSource.volume = 1;
        isFading = false;
    }
    
    private IEnumerator StartFadeIn()
    {
        UpdateQueueText();
        isFading = true;
        currentMusicSource.Play();
        float _timer = 0;
        while (_timer < FadeTime)
        {
            _timer += Time.deltaTime;
            float _t = _timer / FadeTime;

            fadeBar.value = Mathf.Lerp(0, 1, _t);
            currentMusicSource.volume = Mathf.Lerp(0, 1, _t);

            yield return null;
        }

        fadeBar.value = 0;
        currentMusicSource.volume = 1;
        isFading = false;
        nextMusic = null;
    }
    
    #endregion
    
}
