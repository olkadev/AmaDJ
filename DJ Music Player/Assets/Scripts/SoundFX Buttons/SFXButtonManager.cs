using UnityEngine;

public class SFXButtonManager : MonoBehaviour
{
    [Header("Sound Button References")]
    [SerializeField] private GameObject[] soundButtons;

    public static bool IgnoreSoundEffectHotkeys;
    private SFXButton sfxButton;
    
    private void Update()
    {
        PlaySoundOnInput();
    }

    private void PlaySoundOnInput()
    {
        if (IgnoreSoundEffectHotkeys) return;
        switch (Input.inputString)
        {
            case "0":
                PlaySound(0);
                break;
            case "1":
                PlaySound(1);
                break;
            case "2":
                PlaySound(2);
                break;
            case "3":
                PlaySound(3);
                break;
            case "4":
                PlaySound(4);
                break;
            case "5":
                PlaySound(5);
                break;
            case "6":
                PlaySound(6);
                break;
            case "7":
                PlaySound(7);
                break;
            case "8":
                PlaySound(8);
                break;
            case "9":
                PlaySound(9);
                break;
        }
    }

    private void PlaySound(int _index)
    {
        sfxButton = soundButtons[_index].GetComponent<SFXButton>();
        sfxButton.PlayButtonSound();
    }
}
