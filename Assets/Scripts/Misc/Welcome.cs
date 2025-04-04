using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Welcome : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private CanvasGroup contentCanvasGroup;
    [SerializeField] private Slider loadingBar;

    private SongManager _songManager;
    
    private static bool _isLoading;
    private float _totalMusic;
    
    public static bool IsLoading => _isLoading;


    private void Awake()
    {
        _songManager = FindFirstObjectByType<SongManager>();
    }

    private void Start()
    {
        _totalMusic = _songManager.TotalMusic;

        loadingBar.maxValue = _totalMusic;
        contentCanvasGroup.alpha = 1f;
        canvasGroup.alpha = 1f;
        StartCoroutine(WelcomeRoutine());
    }

    private IEnumerator WelcomeRoutine()
    {
        const float maxTime = 1f;
        float timer = 0;
        float maxTotalMusic = _totalMusic;

        while (timer < maxTime)
        {
            timer += Time.deltaTime;
            float t = timer / maxTime;

            contentCanvasGroup.alpha = Mathf.Lerp(0, 1, t);
            yield return null;
        }
        timer = 0;
        contentCanvasGroup.alpha = 1f;

        while (_totalMusic > 0)
        {
            _totalMusic = _songManager.TotalMusic - maxTotalMusic;
            loadingBar.value = Mathf.Lerp(0, maxTotalMusic, -_totalMusic);
            yield return null;
        }
        loadingBar.value = maxTotalMusic;

        yield return new WaitForSeconds(maxTime/2);
        
        while (timer < maxTime*2)
        {
            timer += Time.deltaTime;
            float t = timer / maxTime;

            canvasGroup.alpha = Mathf.Lerp(1, 0, t);
            yield return null;
        }
        canvasGroup.alpha = 0;
        
        Destroy(this.gameObject);
        yield return null;
    }
}
