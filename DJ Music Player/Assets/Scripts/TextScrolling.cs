using System.Collections;
using UnityEngine;

public class TextScrolling : MonoBehaviour
{
    private const float SCROLL_SPEED = 7f; //Higher is Slower
    private const float TIME_BETWEEN_SCROLLS = 1f;
    
    private RectTransform textTransform;
    private Song song;
    
    private float parentWidth;
    private float offset;
    private Vector2 targetPos;
    private float targetPosX;

    private bool isScrolling;

    private void Awake()
    {
        parentWidth = transform.parent.GetComponent<RectTransform>().rect.width;
        textTransform = GetComponent<RectTransform>();
    }


    private void Update()
    {
        if (!isScrolling && textTransform.rect.width > parentWidth)
        {
            StartCoroutine(HandleScrolling());
        }
    }
    
    private void OnDisable()
    {
        isScrolling = false;
    }

    private IEnumerator HandleScrolling()
    {
        isScrolling = true;
        Debug.Log("Coroutine Activated");
        var _startingPos = textTransform.anchoredPosition;
        while (true)
        {
            offset = textTransform.rect.width - parentWidth;
            float _timer = 0f;
            float _timeToScroll = (offset / parentWidth) * SCROLL_SPEED;
            while (_timer < _timeToScroll)
            {
                _timer += Time.deltaTime;
                float _t = _timer / _timeToScroll;
                targetPosX = Mathf.Lerp(_startingPos.x, -offset, _t);
                targetPos = new Vector2(targetPosX, textTransform.anchoredPosition.y);
                textTransform.anchoredPosition = targetPos;
                yield return null;
            }
            yield return new WaitForSeconds(TIME_BETWEEN_SCROLLS);
            
            targetPosX = offset;
            targetPos = new Vector2(targetPosX, textTransform.anchoredPosition.y);
            textTransform.anchoredPosition = targetPos;
            _timer = 0f;
            
            while (_timer < _timeToScroll)
            {
                _timer += Time.deltaTime;
                float _t = _timer / _timeToScroll;
                targetPosX = Mathf.Lerp(-offset, _startingPos.x, _t);
                targetPos = new Vector2(targetPosX, textTransform.anchoredPosition.y);
                textTransform.anchoredPosition = targetPos;
                yield return null;
            }
            yield return new WaitForSeconds(TIME_BETWEEN_SCROLLS);
            
            targetPosX = _startingPos.x;
            targetPos = new Vector2(targetPosX, textTransform.anchoredPosition.y);
            textTransform.anchoredPosition = targetPos;
            yield return null;
        }
    }
}
