using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CellAnimator : MonoBehaviour
{
    [SerializeField] private Image background;
    [SerializeField] private RectTransform bulbTransform;
    [SerializeField] private float defaultDuration = 0.13f;
    [SerializeField] private float pressedScale = 0.94f;
    [SerializeField] private float scaleDuration = 0.08f;
    [SerializeField] private float popDuration = 0.18f;
    [SerializeField] private float popOvershoot = 1.15f;
    [SerializeField] private float shrinkDuration = 0.12f;
    [SerializeField] private float pulseScale = 1.1f;
    [SerializeField] private float pulseDuration = 0.16f;

    private Coroutine _colorRoutine;
    private Coroutine _scaleRoutine;
    private Coroutine _bulbRoutine;

    public void SetColorInstant(Color color)
    {
        if (_colorRoutine != null)
        {
            StopCoroutine(_colorRoutine);
            _colorRoutine = null;
        }

        background.color = color;
    }

    public void AnimateColorTo(Color target)
    {
        if (_colorRoutine != null)
        {
            StopCoroutine(_colorRoutine);
        }

        _colorRoutine = StartCoroutine(ColorRoutine(target));
    }

    private IEnumerator ColorRoutine(Color target)
    {
        Color start = background.color;
        float elapsed = 0f;

        while (elapsed < defaultDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / defaultDuration);
            background.color = Color.Lerp(start, target, t);
            yield return null;
        }

        background.color = target;
        _colorRoutine = null;
    }

    public void PressDown()
    {
        AnimateScaleTo(Vector3.one * pressedScale);
    }

    public void PressUp()
    {
        AnimateScaleTo(Vector3.one);
    }

    private void AnimateScaleTo(Vector3 target)
    {
        if (_scaleRoutine != null)
        {
            StopCoroutine(_scaleRoutine);
        }

        _scaleRoutine = StartCoroutine(ScaleRoutine(target));
    }

    private IEnumerator ScaleRoutine(Vector3 target)
    {
        Vector3 start = transform.localScale;
        float elapsed = 0f;

        while (elapsed < scaleDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / scaleDuration);
            transform.localScale = Vector3.Lerp(start, target, t);
            yield return null;
        }

        transform.localScale = target;
        _scaleRoutine = null;
    }

    public void PopBulb()
    {
        if (_bulbRoutine != null)
        {
            StopCoroutine(_bulbRoutine);
        }

        _bulbRoutine = StartCoroutine(PopBulbRoutine());
    }

    private IEnumerator PopBulbRoutine()
    {
        bulbTransform.localScale = Vector3.zero;
        float half = popDuration * 0.5f;

        float elapsed = 0f;
        while (elapsed < half)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / half);
            bulbTransform.localScale = Vector3.one * Mathf.Lerp(0f, popOvershoot, t);
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < half)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / half);
            bulbTransform.localScale = Vector3.one * Mathf.Lerp(popOvershoot, 1f, t);
            yield return null;
        }

        bulbTransform.localScale = Vector3.one;
        _bulbRoutine = null;
    }

    public void ShrinkBulb()
    {
        if (_bulbRoutine != null)
        {
            StopCoroutine(_bulbRoutine);
        }

        _bulbRoutine = StartCoroutine(ShrinkBulbRoutine());
    }

    private IEnumerator ShrinkBulbRoutine()
    {
        Vector3 start = bulbTransform.localScale;
        float elapsed = 0f;

        while (elapsed < shrinkDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / shrinkDuration);
            bulbTransform.localScale = Vector3.Lerp(start, Vector3.zero, t);
            yield return null;
        }

        bulbTransform.localScale = Vector3.zero;
        bulbTransform.gameObject.SetActive(false);
        _bulbRoutine = null;
    }

    public void Pulse()
    {
        if (_scaleRoutine != null)
        {
            StopCoroutine(_scaleRoutine);
        }

        _scaleRoutine = StartCoroutine(PulseRoutine());
    }

    private IEnumerator PulseRoutine()
    {
        float half = pulseDuration * 0.5f;

        float elapsed = 0f;
        while (elapsed < half)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / half);
            transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * pulseScale, t);
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < half)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / half);
            transform.localScale = Vector3.Lerp(Vector3.one * pulseScale, Vector3.one, t);
            yield return null;
        }

        transform.localScale = Vector3.one;
        _scaleRoutine = null;
    }
}
