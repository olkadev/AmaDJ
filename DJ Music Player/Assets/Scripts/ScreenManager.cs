using System;
using System.Linq;
using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    private bool isFullScreen = true;
    private Resolution[] resolutions;

    private void Start()
    {
        resolutions = Screen.resolutions;
        Screen.SetResolution(resolutions.Last().width, resolutions.Last().height, true);

        
        Debug.Log($"Max Screen Resolution width: {resolutions.Last().width} height: {resolutions.Last().height}");
    }

    public void ToggleScreenMode()
    {
        if (isFullScreen)
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
            isFullScreen = false;
            Debug.LogWarning("Outside of the editor you are now on Windowed Mode");
        }
        else
        {
            Screen.SetResolution(resolutions.Last().width, resolutions.Last().height, true);
            isFullScreen = true;
            Debug.LogWarning("Outside of the editor you are now on FullScreen Mode");
        }
    }
}
