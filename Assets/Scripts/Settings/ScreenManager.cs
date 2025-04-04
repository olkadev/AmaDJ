using System.Linq;
using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    private bool _isFullScreen = true;
    private Resolution[] _resolutions;

    private void Start()
    {
        _resolutions = Screen.resolutions;
        
        // This is to prevent UI strange behaviour
        Screen.SetResolution(_resolutions.Last().width, _resolutions.Last().height, true);
    }

    public void ToggleScreenMode()
    {
        if (_isFullScreen)
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
            _isFullScreen = false;
            Debug.LogWarning("Outside of the editor you are now on Windowed Mode");
        }
        else
        {
            Screen.SetResolution(_resolutions.Last().width, _resolutions.Last().height, true);
            _isFullScreen = true;
            Debug.LogWarning("Outside of the editor you are now on FullScreen Mode");
        }
    }
}
