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
    [SerializeField] private GameObject confirmationCanvas;
    [Header("Input Field References")] 
    [SerializeField] private TMP_InputField searchInputField;
    [SerializeField] private TMP_InputField autoplayInputField;

    public void ToggleAutoplayTagBar()
    {
        autoplayTagBar.SetActive(!autoplayTagBar.activeSelf);
        SFXButtonManager.IgnoreSoundEffectHotkeys = autoplayTagBar.activeSelf;
        if (!autoplayTagBar.activeSelf) return;
        autoplayInputField.Select();
    }
    
    public void ToggleSearchBar()
    {
        searchBar.SetActive(!searchBar.activeSelf);
        SFXButtonManager.IgnoreSoundEffectHotkeys = searchBar.activeSelf;
        if (!searchBar.activeSelf) return;
        searchInputField.Select();
    }
    
    public void ToggleImportSongMenu()
    {
        importSongMenu.SetActive(!importSongMenu.activeSelf);
        SFXButtonManager.IgnoreSoundEffectHotkeys = importSongMenu.activeSelf;
    }
    
    public void ToggleEscapeMenu()
    {
        escapeMenu.SetActive(!escapeMenu.activeSelf);
        goodbyeCanvas.SetActive(!goodbyeCanvas.activeSelf);
        SFXButtonManager.IgnoreSoundEffectHotkeys = escapeMenu.activeSelf;
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
