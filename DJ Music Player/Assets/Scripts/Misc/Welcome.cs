using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Welcome : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private CanvasGroup contentCanvasGroup;
    [SerializeField] private Slider loadingBar;

    private SongManager songManager;
    
    private static bool isLoading;
    public static bool IsLoading => isLoading;

    float totalMusic;

    private void Awake()
    {
        songManager = FindFirstObjectByType<SongManager>();
    }

    private void Start()
    {
        totalMusic = songManager.TotalMusic;

        loadingBar.maxValue = totalMusic;
        contentCanvasGroup.alpha = 1f;
        canvasGroup.alpha = 1f;
        StartCoroutine(WelcomeRoutine());
    }

    private IEnumerator WelcomeRoutine()
    {
        const float _MAX_TIME = 1f;
        float _timer = 0;
        float _maxTotalMusic = totalMusic;

        while (_timer < _MAX_TIME)
        {
            _timer += Time.deltaTime;
            float _t = _timer / _MAX_TIME;

            contentCanvasGroup.alpha = Mathf.Lerp(0, 1, _t);
            yield return null;
        }
        _timer = 0;
        contentCanvasGroup.alpha = 1f;

        while (totalMusic > 0)
        {
            totalMusic = songManager.TotalMusic - _maxTotalMusic;
            loadingBar.value = Mathf.Lerp(0, _maxTotalMusic, -totalMusic);
            yield return null;
        }
        loadingBar.value = _maxTotalMusic;

        yield return new WaitForSeconds(_MAX_TIME/2);
        
        while (_timer < _MAX_TIME*2)
        {
            _timer += Time.deltaTime;
            float _t = _timer / _MAX_TIME;

            canvasGroup.alpha = Mathf.Lerp(1, 0, _t);
            yield return null;
        }
        canvasGroup.alpha = 0;
        
        Destroy(this.gameObject);
        yield return null;
    }
}
