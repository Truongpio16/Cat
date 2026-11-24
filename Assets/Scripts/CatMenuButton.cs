using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

// Hieu ung hover/press cho nut Main Menu kieu pixel-art (nay nhe khi hover,
// lun xuong khi bam). KHONG tu phat am thanh/click logic - de nguyen cho
// HomeManager xu ly, chi phat them am thanh hover neu AudioManager co ho tro.
[RequireComponent(typeof(RectTransform))]
public class CatMenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private float hoverScale = 1.06f;
    [SerializeField] private float pressScale = 0.94f;
    [SerializeField] private float tweenDuration = 0.08f;
    [SerializeField] private AudioManager audioManager;

    private RectTransform _rt;
    private Vector3 _baseScale;
    private Coroutine _routine;
    private bool _isHovering;

    private void Awake()
    {
        _rt = GetComponent<RectTransform>();
        _baseScale = _rt.localScale;
        if (audioManager == null)
        {
            audioManager = FindObjectOfType<AudioManager>();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _isHovering = true;
        AnimateTo(_baseScale * hoverScale);
        if (audioManager != null)
        {
            audioManager.PlayButtonHover();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _isHovering = false;
        AnimateTo(_baseScale);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        AnimateTo(_baseScale * pressScale);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        AnimateTo(_isHovering ? _baseScale * hoverScale : _baseScale);
    }

    private void AnimateTo(Vector3 target)
    {
        if (_routine != null)
        {
            StopCoroutine(_routine);
        }
        _routine = StartCoroutine(ScaleRoutine(target));
    }

    private IEnumerator ScaleRoutine(Vector3 target)
    {
        Vector3 start = _rt.localScale;
        float elapsed = 0f;

        while (elapsed < tweenDuration)
        {
            elapsed += Time.deltaTime;
            _rt.localScale = Vector3.Lerp(start, target, elapsed / tweenDuration);
            yield return null;
        }

        _rt.localScale = target;
    }
}
