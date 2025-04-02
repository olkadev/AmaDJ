using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ButtonSprite : MonoBehaviour
{
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite selectedSprite;

    [SerializeField] private Color disabledColor;
    [SerializeField] private bool isPauseButton;

    [SerializeField] private Button button;
    
    private Image image;
    
    private Color normalColor;
    private bool isSelected;

    private MusicPlayer musicPlayer;

    private void Awake()
    {
        image = GetComponent<Image>();
        musicPlayer = FindFirstObjectByType<MusicPlayer>();
    }

    private void Start()
    {
        normalColor = image.color;
        
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
            image.sprite = musicPlayer.IsPaused ? selectedSprite : normalSprite;
        }
        else
        {
            if (!button.interactable)
            {
                image.sprite = normalSprite;
                isSelected = false;
                image.color = disabledColor;
            }
            else
            {
                image.color = normalColor;
            }
        }
    }

    private void OnClick()
    {
        if (isPauseButton) return;
        isSelected = !isSelected;
        image.sprite = isSelected ? selectedSprite : normalSprite;
    }
}
