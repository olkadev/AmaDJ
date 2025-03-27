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

    [SerializeField] private TMP_InputField searchBarText;
    
    private List<Song> displaySongs = new List<Song>();
    private List<Song> songs = new List<Song>();
    private AudioClip audioClip;
    private static bool isNameSorted;
    private static int sortMode;                                                                                        // 1 for Name A-Z with minus being Z-A, 2 for Author, 3 for Tag
    private int indexCounter;
    private int emptyIndex;
    private int totalMusic;

    public List<Song> Songs => songs;
    
    private void Start()
    {
        sortMode = 1;
        HandleSorting();
        
        totalMusic = PlayerPrefs.GetInt("TotalMusic");
        Debug.Log("Total Music: " + totalMusic);
        StartCoroutine(StartRoutine());
    }

    #region Buttons
    
    public void SortByNameBtn()
    {
        if (!(sortMode == 1 || sortMode == -1))
        {
            sortMode = -1;
        }
        sortMode = -sortMode ;
        HandleSorting();
    }
    
    public void SortByAuthorBtn()
    {
        if (!(sortMode == 2 || sortMode == -2))
        {
            sortMode = -2;
        }
        sortMode = -sortMode;
        HandleSorting();
    }
    
    public void SortByTagBtn()
    {
        if (!(sortMode == 3 || sortMode == -3))
        {
            sortMode = -3;
        }
        sortMode = -sortMode;
        HandleSorting();
    }
    
    #endregion
    
    public void SearchAll()
    {
        foreach (var _song in displaySongs)
        {
            if (_song.Music.NameText.ToLower().Contains(searchBarText.text.ToLower()) 
                || _song.Music.AuthorText.ToLower().Contains(searchBarText.text.ToLower()) 
                || _song.Music.TagText.ToLower().Contains(searchBarText.text.ToLower()))
            {
                _song.gameObject.SetActive(true);
            }
            else
            {
                _song.gameObject.SetActive(false);
            }
        }
    }

    public void RemoveAllSongs()
    {
        StartCoroutine(RemoveAllSongsRoutine());
    }
    
    public void RemoveSong(int _index)
    {
        songs.RemoveAt(_index);
        songs.Sort((_a, _b) => _a.GetSongIndex().CompareTo(_b.GetSongIndex()));
        HandleSorting();
    }
    
    public void AddSong(string _currentFilePath, int _index)
    {
        songs.Add(Instantiate(songPrefab, songParent.transform).GetComponent<Song>()); //adds to the end
        songs[songs.Count - 1].SetSongIndex(_index);
        songs[songs.Count - 1].SongManagerInit(gameObject.GetComponent<SongManager>());
        songs[songs.Count - 1].Music = SaveSystem.LoadMusic(_index).Music;
        StartCoroutine(ImportMusicClipRoutine(_currentFilePath, songs.Count - 1));
        HandleSorting();
    }

    private void HandleSorting()
    {
        displaySongs = songs;
        switch (sortMode)
        {
            case 1: // Name A-Z
                displaySongs = displaySongs.OrderBy(_song => _song.Music.NameText, StringComparer.InvariantCultureIgnoreCase).ToList();
                for (int i = 0; i < displaySongs.Count; i++)
                {
                    displaySongs[i].transform.SetSiblingIndex(i);
                }
                break;
            case 2: // Genre A-Z
                displaySongs = displaySongs.OrderBy(_song => _song.Music.AuthorText, StringComparer.InvariantCultureIgnoreCase).ToList();
                for (int i = 0; i < displaySongs.Count; i++)
                {
                    displaySongs[i].transform.SetSiblingIndex(i);
                }
                break;
            case 3: // Tag A-Z
                displaySongs = displaySongs.OrderBy(_song => _song.Music.TagText, StringComparer.InvariantCultureIgnoreCase).ToList();
                for (int i = 0; i < displaySongs.Count; i++)
                {
                    displaySongs[i].transform.SetSiblingIndex(i);
                }
                break;
            
            case -1: // Name Z-A
                displaySongs = displaySongs.OrderBy(_song => _song.Music.NameText, StringComparer.InvariantCultureIgnoreCase).ToList();
                displaySongs.Reverse();
                for (int i = 0; i < displaySongs.Count; i++)
                {
                    displaySongs[i].transform.SetSiblingIndex(i);
                }
                break;
            case -2: // Author Z-A
                displaySongs = displaySongs.OrderBy(_song => _song.Music.AuthorText, StringComparer.InvariantCultureIgnoreCase).ToList();
                displaySongs.Reverse();
                for (int i = 0; i < displaySongs.Count; i++)
                {
                    displaySongs[i].transform.SetSiblingIndex(i);
                }
                break;
            case -3: // Tag Z-A
                displaySongs = displaySongs.OrderBy(_song => _song.Music.TagText, StringComparer.InvariantCultureIgnoreCase).ToList();
                displaySongs.Reverse();
                for (int i = 0; i < displaySongs.Count; i++)
                {
                    displaySongs[i].transform.SetSiblingIndex(i);
                }
                break;
            
            default:
                sortMode = 1;
                break;
        }
    }

    private IEnumerator RemoveAllSongsRoutine()
    {
        songs.Sort((_a, _b) => _a.GetSongIndex().CompareTo(_b.GetSongIndex()));

        indexCounter = 0;
        totalMusic = PlayerPrefs.GetInt("TotalMusic");
        
        while (totalMusic > 0)
        {
            string _currentFilePath = Path.Combine(Application.persistentDataPath, songs[indexCounter].GetSongIndex().ToString("000") + ".mp3");
            File.Delete(_currentFilePath);
            _currentFilePath = Path.Combine(Application.persistentDataPath, songs[indexCounter].GetSongIndex().ToString("000") + ".mdat");
            File.Delete(_currentFilePath);
            Destroy(songs[indexCounter].gameObject);

            totalMusic--;
            Debug.Log(totalMusic);
            indexCounter++;
        }
        songs.Clear();
        PlayerPrefs.SetInt("TotalMusic", 0);
        yield return null;
    }
    
    private IEnumerator StartRoutine()
    {
        indexCounter = 0;
        emptyIndex = 0;
        
        while (totalMusic > 0)
        {
            string _currentFilePath = Path.Combine(Application.persistentDataPath, indexCounter.ToString("000") + ".mdat");
            if (File.Exists(_currentFilePath))
            {
                songs.Add(Instantiate(songPrefab, songParent.transform).GetComponent<Song>());
                songs[indexCounter - emptyIndex].SetSongIndex(indexCounter);
                songs[indexCounter - emptyIndex].SongManagerInit(gameObject.GetComponent<SongManager>());
                songs[indexCounter - emptyIndex].Music = SaveSystem.LoadMusic(indexCounter).Music;
                yield return StartCoroutine(ImportMusicClipRoutine(songs[indexCounter - emptyIndex].Music.MusicClipPath, indexCounter - emptyIndex));

                totalMusic--;
            }
            else
            {
                emptyIndex++;
            }
            indexCounter++;
        }
        HandleSorting();
    }
    
    private IEnumerator ImportMusicClipRoutine(string _path, int _index)
    {
        int _audioTypeId = songs[_index].Music.AudioTypeId;
        switch (_audioTypeId)
        {
            case 1:
                StartCoroutine(HandleAudioClipRoutine(_path, _index, AudioType.MPEG));
                break;
            case 2:
                StartCoroutine(HandleAudioClipRoutine(_path, _index, AudioType.OGGVORBIS));
                break;
            case 3:
                StartCoroutine(HandleAudioClipRoutine(_path, _index, AudioType.WAV));
                break;
            default:
                Debug.LogError("Clip has unknown AudioType on Start");
                break;
        }

        yield return null;
    }

    private IEnumerator HandleAudioClipRoutine(string _path, int _index, AudioType _audioType)
    {
        using UnityWebRequest _www = UnityWebRequestMultimedia.GetAudioClip("file://" + _path, _audioType);
        yield return _www.SendWebRequest();
        
        if (_www.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.LogWarning(_www.error);
        }
        else
        {
            audioClip = DownloadHandlerAudioClip.GetContent(_www);
            songs[_index].Music.MusicClip = audioClip;
        }
    }
}
