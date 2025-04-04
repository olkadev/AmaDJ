using System;
using UnityEngine;
using UnityEngine.UI;

public class SortingButtonColors : MonoBehaviour
{
    private enum SortingButtonType
    {
        Name,
        Author,
        Tag
    }
    [SerializeField] private SortingButtonType sortingButtonType;

    [SerializeField] private Color selectedColor;
    [SerializeField] private Color negativeSelectedColor;

    
    private Color _normalColor;
    private Image _image;

    private void Awake()
    {
        _image = GetComponent<Image>();
    }

    private void Start()
    {
        _normalColor = _image.color;
    }

    private void Update()
    {
        ChooseColor();
    }

    private void ChooseColor()
    {
        switch (sortingButtonType)
        {
            case SortingButtonType.Name:
                switch (SongManager.SortMode)
                {
                    case 1:
                        _image.color = selectedColor;
                        break;
                    case -1:
                        _image.color = negativeSelectedColor;
                        break;
                    default:
                        _image.color = _normalColor;
                        break;
                }
                break;
            case SortingButtonType.Author:
                switch (SongManager.SortMode)
                {
                    case 2:
                        _image.color = selectedColor;
                        break;
                    case -2:
                        _image.color = negativeSelectedColor;
                        break;
                    default:
                        _image.color = _normalColor;
                        break;
                }
                break;
            case SortingButtonType.Tag:
                switch (SongManager.SortMode)
                {
                    case 3:
                        _image.color = selectedColor;
                        break;
                    case -3:
                        _image.color = negativeSelectedColor;
                        break;
                    default:
                        _image.color = _normalColor;
                        break;
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}

