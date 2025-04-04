using System.Collections;
using TMPro;
using UnityEngine;

public class Goodbye : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TMP_Text goodbyeText;
    
    private static bool _isQuitting;
    public static bool IsQuitting => _isQuitting;

    private void Start()
    {
        canvasGroup.alpha = 0;
        goodbyeText.alpha = 0;
    }

    public void QuitApplicationBtn()
    {
        _isQuitting = true;
        StartCoroutine(GoodbyeRoutine());
    }
    
    // Fades in "Goodbye" canvas and quits the application
    private IEnumerator GoodbyeRoutine()
    {
        const float maxTime = 1.5f;
        float timer = 0;

        while (timer < maxTime)
        {
            timer += Time.deltaTime;
            float t = timer / maxTime;

            canvasGroup.alpha = Mathf.Lerp(0, 1, t);
            yield return null;
        }

        canvasGroup.alpha = 1;
        
        yield return new WaitForSeconds(maxTime/2);

        timer = 0;

        while (timer < maxTime)
        {
            timer += Time.deltaTime;
            float t = timer / maxTime;

            goodbyeText.alpha = Mathf.Lerp(0, 1, t);
            yield return null;
        }
        goodbyeText.alpha = 1;
        yield return new WaitForSeconds(maxTime/2);
        
        Debug.LogWarning("If you are not in the Editor the application should have closed.");
        Application.Quit();
        
        yield return null;
    }
}
