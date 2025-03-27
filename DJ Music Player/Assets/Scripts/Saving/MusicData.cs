using System;
using UnityEngine;

[Serializable]
public class Music
{
    [NonSerialized] private AudioClip musicClip; // Will be loaded after
    private string musicClipPath;
    private string nameText;
    private string authorText;
    private string tagText;
    private int id;
    private int audioTypeId; 
    /* Audio Types by ID
     1 = MPEG
     2 = OGGVORBIS
     3 = WAV
     4 = 
     
     
      
     */
    
    public AudioClip MusicClip
    {
        get => musicClip;
        set => musicClip = value;
    }
    public string MusicClipPath
    {
        get => musicClipPath;
        set => musicClipPath = value;
    }
    public string NameText
    {
        get => nameText;
        set => nameText = value;
    }
    public string AuthorText
    {
        get => authorText;
        set => authorText = value;
    }
    public string TagText
    {
        get => tagText;
        set => tagText = value;
    }
    public int AudioTypeId
    {
        get => audioTypeId;
        set => audioTypeId = value;
    }
    public int Id
    {
        get => id;
        set => id = value;
    }
}

[Serializable]
public class MusicData
{
    public Music Music;

    public MusicData(Music _music)
    {
        Music = _music;
    }
}