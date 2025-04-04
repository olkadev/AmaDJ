using UnityEngine;
using UnityEngine.UI;

public class ButtonSprite : MonoBehaviour
{
    [SerializeField] private Sprite selectedSprite;

    [SerializeField] private Color disabledColor;
    [SerializeField] private bool isPauseButton;

    [SerializeField] private Button button;

    private Sprite _normalSprite;
    private Image _image;
    
    private Color _normalColor;
    private bool _isSelected;

    private MusicPlayer _musicPlayer;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _musicPlayer = FindFirstObjectByType<MusicPlayer>();
    }

    private void Start()
    {
        _normalSprite = _image.sprite;
        _normalColor = _image.color;
        
        button.onClick.AddListener(OnClick);
    }

    private void Update()
    {
        OnDisable();
    }

    private void OnDisable()
    {
        if (isPauseButton)
        {
            _image.sprite = _musicPlayer.IsPaused ? selectedSprite : _normalSprite;
        }
        else
        {
            if (!button.interactable)
            {
                _image.sprite = _normalSprite;
                _isSelected = false;
                _image.color = disabledColor;
            }
            else
            {
                _image.color = _normalColor;
            }
        }
    }

    private void OnClick()
    {
        if (isPauseButton) return;
        _isSelected = !_isSelected;
        _image.sprite = _isSelected ? selectedSprite : _normalSprite;
    }
}
