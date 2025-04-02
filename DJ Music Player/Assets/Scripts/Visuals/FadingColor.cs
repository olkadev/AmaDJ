using UnityEngine;
using UnityEngine.UI;

public class FadingColor : MonoBehaviour
{
    
    [SerializeField] private Color fadingColor;
    private Color normalColor;

    private const float LERP_TIME = 2f;

    private MusicPlayer musicPlayer;

    private Image image;
    
    private void Awake()
    {
        image = GetComponent<Image>();
        musicPlayer = FindFirstObjectByType<MusicPlayer>();
    }

    private void Start()
    {
        normalColor = image.color;
    }

    private void Update()
    {
        HandleColor();
    }

    private void HandleColor()
    {
        image.color = musicPlayer.IsFading ? fadingColor : normalColor;
    }
}
