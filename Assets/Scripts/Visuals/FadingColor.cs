using UnityEngine;
using UnityEngine.UI;

public class FadingColor : MonoBehaviour
{
    
    [SerializeField] private Color fadingColor;
    private Color _normalColor;
    
    private MusicPlayer _musicPlayer;

    private Image _image;
    
    private void Awake()
    {
        _image = GetComponent<Image>();
        _musicPlayer = FindFirstObjectByType<MusicPlayer>();
    }

    private void Start()
    {
        _normalColor = _image.color;
    }

    private void Update()
    {
        HandleColor();
    }

    private void HandleColor()
    {
        _image.color = _musicPlayer.IsFading ? fadingColor : _normalColor;
    }
}
