using UnityEngine;

public class ScreenMode : MonoBehaviour
{
    private bool isFullScreen = true;
    
    public void ToggleScreenMode()
    {
        if (isFullScreen)
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
            isFullScreen = false;
            Debug.LogWarning("Outside of the editor you are now on Windowed Mode");
        }

        if (!isFullScreen)
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
            isFullScreen = true;
            Debug.LogWarning("Outside of the editor you are now on FullScreen Mode");
        }
    }
}
