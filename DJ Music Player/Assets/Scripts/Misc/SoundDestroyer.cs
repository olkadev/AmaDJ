using System;
using Unity.VisualScripting;
using UnityEngine;

public class SoundDestroyer : MonoBehaviour
{
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (!audioSource.isPlaying)
        {
            Destroy(this.gameObject);
        }
    }
}
