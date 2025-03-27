using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable] public static class SaveSystem
{
    private static int indexCounter;

    public static void EditMusic(Music _music, int _index)
    {
        string _currentFilePath = Path.Combine(Application.persistentDataPath, _index.ToString("000") + ".mdat");

        if (File.Exists(_currentFilePath))
        {
            BinaryFormatter _formatter = new BinaryFormatter();
            FileStream _stream = new FileStream(_currentFilePath, FileMode.Create);

            MusicData _data = new MusicData(_music);
        
            _formatter.Serialize(_stream, _data);
            _stream.Close();
        }
        else
        {
            Debug.LogWarning("The File you are trying to Edit doesnt exist.");
        }
    }
    
    public static void SaveMusic(Music _music)
    {
        indexCounter = 0;
        
        bool _isEnd = false;

        while (!_isEnd)
        {
            string _currentFilePath = Path.Combine(Application.persistentDataPath, indexCounter.ToString("000") + ".mdat");

            if (File.Exists(_currentFilePath))
            {
                indexCounter++;
            }
            else
            {
                BinaryFormatter _formatter = new BinaryFormatter();
                FileStream _stream = new FileStream(_currentFilePath, FileMode.Create);

                MusicData _data = new MusicData(_music);
        
                _formatter.Serialize(_stream, _data);
                _stream.Close();
                
                _isEnd = true;
            }
        }
        
    }

    public static MusicData LoadMusic(int _index)
    {
        string _currentFilePath = Path.Combine(Application.persistentDataPath, _index.ToString("000") + ".mdat");
        if (File.Exists(_currentFilePath))
        {
            BinaryFormatter _formatter = new BinaryFormatter();
            FileStream _stream = new FileStream(_currentFilePath, FileMode.Open);

            MusicData _data = _formatter.Deserialize(_stream) as MusicData;
            
            _stream.Close();
            
            return _data;
        }
        else
        {
            Debug.LogWarning("Save file not found in " + _currentFilePath);
            return null;
        }
    }
}
