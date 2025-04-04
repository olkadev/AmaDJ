using System;
using UnityEngine;

[Serializable]
public class Music
{
    [NonSerialized] private AudioClip _musicClip; // Will be initialized during runtime
    private string _musicClipPath;
    private string _nameText;
    private string _authorText;
    private string _tagText;
    private int _id;
    
    private int _audioTypeId; 
    // Audio Types by ID
    // 1 = MPEG
    // 2 = OGGVORBIS
    // 3 = WAV

    #region Getters and Setters
    
    public AudioClip MusicClip
    {
        get => _musicClip;
        set => _musicClip = value;
    }
    public string MusicClipPath
    {
        get => _musicClipPath;
        set => _musicClipPath = value;
    }
    public string NameText
    {
        get => _nameText;
        set => _nameText = value;
    }
    public string AuthorText
    {
        get => _authorText;
        set => _authorText = value;
    }
    public string TagText
    {
        get => _tagText;
        set => _tagText = value;
    }
    public int AudioTypeId
    {
        get => _audioTypeId;
        set => _audioTypeId = value;
    }
    public int Id
    {
        get => _id;
        set => _id = value;
    }
    
    #endregion
}

[Serializable]
public class MusicData
{
    public Music music;

    public MusicData(Music music)
    {
        this.music = music;
    }
}