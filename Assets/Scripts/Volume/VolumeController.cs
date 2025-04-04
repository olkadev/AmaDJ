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
    private Color _boostButtonNormalColor;

    private bool _isVolumeBoosted;
    private const float VolumeAmount = 60f;

    private void Start()
    {
        _boostButtonNormalColor = boostButton.image.color;
    }

    private void Update()
    {
        ControlVolume();
    }

    public void ToggleBoostedVolume()
    {
        boostSlider.gameObject.SetActive(!boostSlider.gameObject.activeSelf);
        _isVolumeBoosted = boostSlider.gameObject.activeSelf;
        boostButton.image.color = boostSlider.gameObject.activeSelf ? boostButtonSelectedColor : _boostButtonNormalColor;

        boostSlider.value = 0;
        SetVolume();
    }

    public void SetVolume()
    {
        volumeSlider.interactable = boostSlider.value == 0;
        boostSlider.interactable = volumeSlider.value == 100;
        
        float boostAmount = ((boostSlider.value / 100) * 0.5f) + 1;
        volumeText.text = volumeSlider.value.ToString("N0");
        boostText.text = ((boostAmount - 1) * 100).ToString("N0")+"%";
        
        AudioListener.volume = volumeSlider.value / 100 * boostAmount;
    }
    
    private void ControlVolume()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            IncreaseVolume(VolumeAmount * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            DecreaseVolume(VolumeAmount * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.UpArrow))
        {
            IncreaseVolume(999999f * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.DownArrow))
        {
            DecreaseVolume(999999f * Time.deltaTime);
        }
    }

    private void IncreaseVolume(float amount)
    {
        if (volumeSlider.value < 100)
        {
            volumeSlider.value += amount;
        }
        else if (_isVolumeBoosted)
        {
            boostSlider.value += amount;
        }
        SetVolume();
    }
    
    private void DecreaseVolume(float amount)
    {
        if (boostSlider.value <= 0)
        {
            volumeSlider.value -= amount;
        }
        else if (_isVolumeBoosted)
        {
            boostSlider.value -= amount;
        }
        SetVolume();
    }
}
