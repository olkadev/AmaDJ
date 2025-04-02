using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VolumeController : MonoBehaviour
{
    [Header("Slider References")]
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Slider boostSlider;
    [SerializeField] private TMP_Text volumeText;
    [SerializeField] private TMP_Text boostText;
    [SerializeField] private Button boostButton;
    [Header("Color")]
    [SerializeField] private Color boostButtonSelectedColor;
    private Color boostButtonNormalColor;

    private bool isVolumeBoosted;

    private void Start()
    {
        boostButtonNormalColor = boostButton.image.color;
    }

    private void Update()
    {
        ControlVolume();
    }

    public void ToggleBoostedVolume()
    {
        boostSlider.gameObject.SetActive(!boostSlider.gameObject.activeSelf);
        isVolumeBoosted = boostSlider.gameObject.activeSelf;
        if (boostSlider.gameObject.activeSelf)
        {
            boostButton.image.color = boostButtonSelectedColor;
        }
        else
        {
            boostButton.image.color = boostButtonNormalColor;
        }

        boostSlider.value = 0;
        SetVolume();
    }

    public void SetVolume()
    {
        volumeSlider.interactable = boostSlider.value == 0;
        boostSlider.interactable = volumeSlider.value == 100;
        
        float _boostAmount = ((boostSlider.value / 100) * 0.5f) + 1;
        volumeText.text = volumeSlider.value.ToString("N0");
        boostText.text = ((_boostAmount - 1) * 100).ToString("N0")+"%";
        
        AudioListener.volume = volumeSlider.value / 100 * _boostAmount;
    }
    
    private void ControlVolume()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            IncreaseVolume(0.1f);
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            DecreaseVolume(0.1f);
        }
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.UpArrow))
        {
            IncreaseVolume(9999f);
        }
        else if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.DownArrow))
        {
            DecreaseVolume(9999f);
        }
    }

    private void IncreaseVolume(float _amount)
    {
        if (volumeSlider.value < 100)
        {
            volumeSlider.value += _amount;
        }
        else if (isVolumeBoosted)
        {
            boostSlider.value += _amount;
        }
        SetVolume();
    }
    
    private void DecreaseVolume(float _amount)
    {
        if (boostSlider.value <= 0)
        {
            volumeSlider.value -= _amount;
        }
        else if (isVolumeBoosted)
        {
            boostSlider.value -= _amount;
        }
        SetVolume();
    }
}
