using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class SongManager : MonoBehaviour
{
    [Header("Song References")]
    [SerializeField] private GameObject songPrefab;
    [SerializeField] private GameObject songParent;
    [Header("Search Bar Reference")]
    [SerializeField] private TMP_InputField searchBarText;
    
    // 1 for Name A-Z with minus being Z-A, 2 for Author, 3 for Tag
    public static int SortMode; 
    
    private List<Song> _displaySongs = new List<Song>();
    private List<Song> _songs = new List<Song>();
    private AudioClip _audioClip;
    private static bool _isNameSorted;
    private int _indexCounter;
    private int _emptyIndex;
    private int _totalMusic;
    public int TotalMusic => _totalMusic;

    public List<Song> Songs => _songs;
    
    private void Start()
    {
        SortMode = 1;
        HandleSorting();
        
        _totalMusic = PlayerPrefs.GetInt("TotalMusic");
        Debug.Log("Total Music: " + _totalMusic);
        StartCoroutine(StartRoutine());
    }

    #region Buttons
    
    public void SortByNameBtn()
    {
        if (!(SortMode == 1 || SortMode == -1))
        {
            SortMode = -1;
        }
        SortMode = -SortMode ;
        HandleSorting();
    }
    
    public void SortByAuthorBtn()
    {
        if (!(SortMode == 2 || SortMode == -2))
        {
            SortMode = -2;
        }
        SortMode = -SortMode;
        HandleSorting();
    }
    
    public void SortByTagBtn()
    {
        if (!(SortMode == 3 || SortMode == -3))
        {
            SortMode = -3;
        }
        SortMode = -SortMode;
        HandleSorting();
    }
    
    #endregion
    
    // Used in searching for songs
    public void SearchAll()
    {
        foreach (var song in _displaySongs)
        {
            if (song.Music.NameText.ToLower().Contains(searchBarText.text.ToLower().Trim()) ||
                song.Music.AuthorText.ToLower().Contains(searchBarText.text.ToLower().Trim()) ||
                song.Music.TagText.ToLower().Contains(searchBarText.text.ToLower().Trim()))
            {
                song.gameObject.SetActive(true);
            }
            else
            {
                song.gameObject.SetActive(false);
            }
        }
    }

    public void RemoveAllSongs()
    {
        StartCoroutine(RemoveAllSongsRoutine());
    }
    
    public void RemoveSong(int index)
    {
        _songs.RemoveAt(index);
        _songs.Sort((a, b) => a.GetSongIndex().CompareTo(b.GetSongIndex()));
        HandleSorting();
    }
    
    public void AddSong(string currentFilePath, int index)
    {
        _songs.Add(Instantiate(songPrefab, songParent.transform).GetComponent<Song>()); //adds to the end
        _songs[_songs.Count - 1].SetSongIndex(index);
        _songs[_songs.Count - 1].SongManagerInit(gameObject.GetComponent<SongManager>());
        _songs[_songs.Count - 1].Music = SaveSystem.LoadMusic(index).music;
        StartCoroutine(ImportMusicClipRoutine(currentFilePath, _songs.Count - 1));
        HandleSorting();
    }

    private void HandleSorting()
    {
        _displaySongs = _songs;
        switch (SortMode)
        {
            case 1: // Name A-Z
                _displaySongs = _displaySongs.OrderBy(song => song.Music.NameText, StringComparer.InvariantCultureIgnoreCase).ToList();
                for (int i = 0; i < _displaySongs.Count; i++)
                {
                    _displaySongs[i].transform.SetSiblingIndex(i);
                }
                break;
            case 2: // Genre A-Z
                _displaySongs = _displaySongs.OrderBy(song => song.Music.AuthorText, StringComparer.InvariantCultureIgnoreCase).ToList();
                for (int i = 0; i < _displaySongs.Count; i++)
                {
                    _displaySongs[i].transform.SetSiblingIndex(i);
                }
                break;
            case 3: // Tag A-Z
                _displaySongs = _displaySongs.OrderBy(song => song.Music.TagText, StringComparer.InvariantCultureIgnoreCase).ToList();
                for (int i = 0; i < _displaySongs.Count; i++)
                {
                    _displaySongs[i].transform.SetSiblingIndex(i);
                }
                break;
            
            case -1: // Name Z-A
                _displaySongs = _displaySongs.OrderBy(song => song.Music.NameText, StringComparer.InvariantCultureIgnoreCase).ToList();
                _displaySongs.Reverse();
                for (int i = 0; i < _displaySongs.Count; i++)
                {
                    _displaySongs[i].transform.SetSiblingIndex(i);
                }
                break;
            case -2: // Author Z-A
                _displaySongs = _displaySongs.OrderBy(song => song.Music.AuthorText, StringComparer.InvariantCultureIgnoreCase).ToList();
                _displaySongs.Reverse();
                for (int i = 0; i < _displaySongs.Count; i++)
                {
                    _displaySongs[i].transform.SetSiblingIndex(i);
                }
                break;
            case -3: // Tag Z-A
                _displaySongs = _displaySongs.OrderBy(song => song.Music.TagText, StringComparer.InvariantCultureIgnoreCase).ToList();
                _displaySongs.Reverse();
                for (int i = 0; i < _displaySongs.Count; i++)
                {
                    _displaySongs[i].transform.SetSiblingIndex(i);
                }
                break;
            
            default:
                SortMode = 1;
                break;
        }
    }

    private IEnumerator StartRoutine()
    {
        _indexCounter = 0;
        _emptyIndex = 0;
        
        // this makes sure that if the save system makes a mistake, or if songs are deleted manually, it can return to being stable
        const int stopAfterXAttempts = 1000;
        int attemptsRemaining = stopAfterXAttempts;
        
        while (_totalMusic > 0)
        {
            string currentFilePath = Path.Combine(Application.persistentDataPath, _indexCounter.ToString("000") + ".mdat");
            if (File.Exists(currentFilePath))
            {
                _songs.Add(Instantiate(songPrefab, songParent.transform).GetComponent<Song>());
                _songs[_indexCounter - _emptyIndex].SetSongIndex(_indexCounter);
                _songs[_indexCounter - _emptyIndex].SongManagerInit(gameObject.GetComponent<SongManager>());
                _songs[_indexCounter - _emptyIndex].Music = SaveSystem.LoadMusic(_indexCounter).music;
                yield return StartCoroutine(ImportMusicClipRoutine(_songs[_indexCounter - _emptyIndex].Music.MusicClipPath, _indexCounter - _emptyIndex));

                _totalMusic--;
                attemptsRemaining = stopAfterXAttempts;
            }
            else
            {
                _emptyIndex++;
                attemptsRemaining--;
                if (attemptsRemaining <= 0) 
                {
                    Debug.LogError("No more songs have been found after 1000 attempts.\nSetting TotalMusic to " + _songs.Count);
                    PlayerPrefs.SetInt("TotalMusic", _songs.Count);
                    break;
                }
            }
            _indexCounter++;
        }
        HandleSorting();
    }
    
    private IEnumerator RemoveAllSongsRoutine()
    {
        _songs.Sort((a, b) => a.GetSongIndex().CompareTo(b.GetSongIndex()));

        _indexCounter = 0;
        _totalMusic = PlayerPrefs.GetInt("TotalMusic");
        
        while (_totalMusic > 0)
        {
            string currentFilePath = Path.Combine(Application.persistentDataPath, _songs[_indexCounter].GetSongIndex().ToString("000") + ".mclp");
            File.Delete(currentFilePath);
            currentFilePath = Path.Combine(Application.persistentDataPath, _songs[_indexCounter].GetSongIndex().ToString("000") + ".mdat");
            File.Delete(currentFilePath);
            Destroy(_songs[_indexCounter].gameObject);

            _totalMusic--;
            Debug.Log(_totalMusic);
            _indexCounter++;
        }
        _songs.Clear();
        PlayerPrefs.SetInt("TotalMusic", 0);
        yield return null;
    }
    
    private IEnumerator ImportMusicClipRoutine(string path, int index)
    {
        int audioTypeId = _songs[index].Music.AudioTypeId;
        switch (audioTypeId)
        {
            case 1:
                StartCoroutine(HandleAudioClipRoutine(path, index, AudioType.MPEG));
                break;
            case 2:
                StartCoroutine(HandleAudioClipRoutine(path, index, AudioType.OGGVORBIS));
                break;
            case 3:
                StartCoroutine(HandleAudioClipRoutine(path, index, AudioType.WAV));
                break;
            default:
                Debug.LogError("Clip has unknown AudioType on Start");
                break;
        }

        yield return null;
    }

    private IEnumerator HandleAudioClipRoutine(string path, int index, AudioType audioType)
    {
        using UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + path, audioType);
        yield return www.SendWebRequest();
        
        if (www.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.LogWarning(www.error);
        }
        else
        {
            _audioClip = DownloadHandlerAudioClip.GetContent(www);
            _songs[index].Music.MusicClip = _audioClip;
        }
    }
}
