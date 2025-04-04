using System.Collections;
using UnityEngine;

// You can use this script for any text AS LONG as it is in a "container" which dictates the _parentWidth
public class TextScrolling : MonoBehaviour
{
    private const float ScrollSpeed = 7f; //Higher is Slower
    private const float TimeBetweenScrolls = 1f;
    
    private RectTransform _textTransform;
    
    private bool _isScrolling;
    
    private float _parentWidth;
    private float _offset;
    private Vector2 _targetPos;
    private float _targetPosX;

    private void OnDisable()
    {
        _isScrolling = false;
    }
    
    private void Awake()
    {
        _parentWidth = transform.parent.GetComponent<RectTransform>().rect.width;
        _textTransform = GetComponent<RectTransform>();
    }
    
    private void Update()
    {
        if (!_isScrolling && _textTransform.rect.width > _parentWidth)
        {
            StartCoroutine(HandleScrolling());
        }
    }

    private IEnumerator HandleScrolling()
    {
        _isScrolling = true;
        var startingPos = _textTransform.anchoredPosition;
        while (true)
        {
            _offset = _textTransform.rect.width - _parentWidth;
            float timer = 0f;
            float timeToScroll = (_offset / _parentWidth) * ScrollSpeed;
            while (timer < timeToScroll)
            {
                timer += Time.deltaTime;
                float t = timer / timeToScroll;
                _targetPosX = Mathf.Lerp(startingPos.x, -_offset, t);
                _targetPos = new Vector2(_targetPosX, _textTransform.anchoredPosition.y);
                _textTransform.anchoredPosition = _targetPos;
                yield return null;
            }
            yield return new WaitForSeconds(TimeBetweenScrolls);
            
            _targetPosX = _offset;
            _targetPos = new Vector2(_targetPosX, _textTransform.anchoredPosition.y);
            _textTransform.anchoredPosition = _targetPos;
            timer = 0f;
            
            while (timer < timeToScroll)
            {
                timer += Time.deltaTime;
                float t = timer / timeToScroll;
                _targetPosX = Mathf.Lerp(-_offset, startingPos.x, t);
                _targetPos = new Vector2(_targetPosX, _textTransform.anchoredPosition.y);
                _textTransform.anchoredPosition = _targetPos;
                yield return null;
            }
            yield return new WaitForSeconds(TimeBetweenScrolls);
            
            _targetPosX = startingPos.x;
            _targetPos = new Vector2(_targetPosX, _textTransform.anchoredPosition.y);
            _textTransform.anchoredPosition = _targetPos;
            yield return null;
        }
    }
}
