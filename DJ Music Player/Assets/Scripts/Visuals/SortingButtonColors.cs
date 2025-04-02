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

    
    private Color normalColor;
    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    private void Start()
    {
        normalColor = image.color;
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
                        image.color = selectedColor;
                        break;
                    case -1:
                        image.color = negativeSelectedColor;
                        break;
                    default:
                        image.color = normalColor;
                        break;
                }
                break;
            case SortingButtonType.Author:
                switch (SongManager.SortMode)
                {
                    case 2:
                        image.color = selectedColor;
                        break;
                    case -2:
                        image.color = negativeSelectedColor;
                        break;
                    default:
                        image.color = normalColor;
                        break;
                }
                break;
            case SortingButtonType.Tag:
                switch (SongManager.SortMode)
                {
                    case 3:
                        image.color = selectedColor;
                        break;
                    case -3:
                        image.color = negativeSelectedColor;
                        break;
                    default:
                        image.color = normalColor;
                        break;
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}

