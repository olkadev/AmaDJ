using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable] public static class SaveSystem
{
    private static int _indexCounter;

    public static void EditMusic(Music music, int index)
    {
        string currentFilePath = Path.Combine(Application.persistentDataPath, index.ToString("000") + ".mdat");

        if (File.Exists(currentFilePath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(currentFilePath, FileMode.Create);

            MusicData data = new MusicData(music);
        
            formatter.Serialize(stream, data);
            stream.Close();
        }
        else
        {
            Debug.LogWarning("The File you are trying to Edit doesnt exist.");
        }
    }
    
    public static void SaveMusic(Music music)
    {
        _indexCounter = 0;
        
        bool isEnd = false;

        while (!isEnd)
        {
            string currentFilePath = Path.Combine(Application.persistentDataPath, _indexCounter.ToString("000") + ".mdat");

            if (File.Exists(currentFilePath))
            {
                _indexCounter++;
            }
            else
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(currentFilePath, FileMode.Create);

                MusicData data = new MusicData(music);
        
                formatter.Serialize(stream, data);
                stream.Close();
                
                isEnd = true;
            }
        }
        
    }

    public static MusicData LoadMusic(int index)
    {
        string currentFilePath = Path.Combine(Application.persistentDataPath, index.ToString("000") + ".mdat");
        if (File.Exists(currentFilePath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(currentFilePath, FileMode.Open);

            MusicData data = formatter.Deserialize(stream) as MusicData;
            
            stream.Close();
            
            return data;
        }
        else
        {
            Debug.LogWarning("Save file not found in " + currentFilePath);
            return null;
        }
    }
}
