using UnityEngine;

public class SoundDestroyer : MonoBehaviour
{
    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (!_audioSource.isPlaying)
        {
            Destroy(this.gameObject);
        }
    }
}
