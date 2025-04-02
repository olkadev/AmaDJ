using System.Collections;
using TMPro;
using UnityEngine;

public class Goodbye : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TMP_Text goodbyeText;
    
    private static bool isQuitting;
    public static bool IsQuitting => isQuitting;

    private void Start()
    {
        canvasGroup.alpha = 0;
        goodbyeText.alpha = 0;
    }

    public void QuitApplicationBtn()
    {
        isQuitting = true;
        StartCoroutine(GoodbyeRoutine());
    }
    
    private IEnumerator GoodbyeRoutine()
    {
        const float _MAX_TIME = 1.5f;
        float _timer = 0;

        while (_timer < _MAX_TIME)
        {
            _timer += Time.deltaTime;
            float _t = _timer / _MAX_TIME;

            canvasGroup.alpha = Mathf.Lerp(0, 1, _t);
            yield return null;
        }

        canvasGroup.alpha = 1;
        
        yield return new WaitForSeconds(_MAX_TIME/2);

        _timer = 0;

        while (_timer < _MAX_TIME)
        {
            _timer += Time.deltaTime;
            float _t = _timer / _MAX_TIME;

            goodbyeText.alpha = Mathf.Lerp(0, 1, _t);
            yield return null;
        }
        goodbyeText.alpha = 1;
        yield return new WaitForSeconds(_MAX_TIME/2);
        
        Debug.LogWarning("If you are not in the Editor the application should have closed.");
        Application.Quit();

        canvasGroup.alpha = 0;
        goodbyeText.alpha = 0;
        yield return null;
    }
}
