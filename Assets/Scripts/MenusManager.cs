using TMPro;
using UnityEngine;

public class MenusManager : MonoBehaviour
{
    [Header("UI Menu References")]
    [SerializeField] private GameObject importSongMenu;
    [SerializeField] private GameObject searchBar;
    [SerializeField] private GameObject autoplayTagBar;
    [SerializeField] private GameObject escapeMenu;
    [SerializeField] private GameObject goodbyeCanvas;
    [SerializeField] private GameObject welcomeCanvas;
    [SerializeField] private GameObject confirmationCanvas;
    [Header("Input Field References")] 
    [SerializeField] private TMP_InputField searchInputField;
    [SerializeField] private TMP_InputField autoplayInputField;

    private void Start()
    {
        welcomeCanvas.SetActive(true);
    }

    public void ToggleAutoplayTagBar()
    {
        autoplayTagBar.SetActive(!autoplayTagBar.activeSelf);
        SoundButtonManager.IgnoreSoundEffectHotkeys = autoplayTagBar.activeSelf;
        MusicPlayer.IgnorePauseHotkey = autoplayTagBar.activeSelf;
        if (!autoplayTagBar.activeSelf) return;
        autoplayInputField.Select();
    }
    
    public void ToggleSearchBar()
    {
        searchBar.SetActive(!searchBar.activeSelf);
        SoundButtonManager.IgnoreSoundEffectHotkeys = searchBar.activeSelf;
        MusicPlayer.IgnorePauseHotkey = searchBar.activeSelf;
        if (!searchBar.activeSelf) return;
        searchInputField.Select();
    }
    
    public void ToggleImportSongMenu()
    {
        importSongMenu.SetActive(!importSongMenu.activeSelf);
        SoundButtonManager.IgnoreSoundEffectHotkeys = importSongMenu.activeSelf;
        MusicPlayer.IgnorePauseHotkey = importSongMenu.activeSelf;
    }
    
    public void ToggleEscapeMenu()
    {
        if (Goodbye.IsQuitting || Welcome.IsLoading) return;
        escapeMenu.SetActive(!escapeMenu.activeSelf);
        goodbyeCanvas.SetActive(!goodbyeCanvas.activeSelf);
        SoundButtonManager.IgnoreSoundEffectHotkeys = escapeMenu.activeSelf;
        MusicPlayer.IgnorePauseHotkey = escapeMenu.activeSelf;
    }

    public void ToggleConfirmationCanvas()
    {
        ToggleEscapeMenu();
        confirmationCanvas.SetActive(!confirmationCanvas.activeSelf);
    }

    private void Update()
    {
        HotkeyEscapeMenu();
    }

    private void HotkeyEscapeMenu()
    {
        if (confirmationCanvas.activeSelf) return;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (importSongMenu.activeSelf)
            {
                ToggleImportSongMenu();
            }
            else
            {
                ToggleEscapeMenu();
            }
        }
    }
}
