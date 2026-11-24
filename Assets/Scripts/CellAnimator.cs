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
    [SerializeField] private float popWiggleAngle = 16f;
    [SerializeField] private float shrinkDuration = 0.12f;
    [SerializeField] private float pulseScale = 1.1f;
    [SerializeField] private float pulseDuration = 0.16f;
    [SerializeField] private float sadScale = 0.88f;
    [SerializeField] private float sadDroopAngle = 14f;
    [SerializeField] private float idleBobAmplitude = 5f;
    [SerializeField] private float idleBobSpeed = 2.2f;
    [SerializeField] private float excitedDuration = 0.45f;
    [SerializeField] private float excitedHopHeight = 22f;
    [SerializeField] private float excitedScale = 1.28f;

    private Coroutine _colorRoutine;
    private Coroutine _scaleRoutine;
    private Coroutine _bulbRoutine;
    private Coroutine _idleRoutine;

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
        bulbTransform.localRotation = Quaternion.identity;
        float half = popDuration * 0.5f;

        // Nua dau: mocc len + nghieng sang 1 ben (tao cam giac "nhay vao")
        float elapsed = 0f;
        while (elapsed < half)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / half);
            bulbTransform.localScale = Vector3.one * Mathf.Lerp(0f, popOvershoot, t);
            bulbTransform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(-popWiggleAngle, popWiggleAngle, t));
            yield return null;
        }

        // Nua sau: on dinh scale + lac nguoc lai ve 0 (wiggle settle)
        elapsed = 0f;
        while (elapsed < half)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / half);
            bulbTransform.localScale = Vector3.one * Mathf.Lerp(popOvershoot, 1f, t);
            bulbTransform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(popWiggleAngle, 0f, t));
            yield return null;
        }

        bulbTransform.localScale = Vector3.one;
        bulbTransform.localRotation = Quaternion.identity;

        // Nay nho vui ve sau khi da on dinh
        float hopDuration = 0.12f;
        float hopElapsed = 0f;
        while (hopElapsed < hopDuration)
        {
            hopElapsed += Time.deltaTime;
            float t = Mathf.Clamp01(hopElapsed / hopDuration);
            bulbTransform.localScale = Vector3.one * (1f + Mathf.Sin(t * Mathf.PI) * 0.08f);
            yield return null;
        }

        bulbTransform.localScale = Vector3.one;
        StartIdleBob();
        _bulbRoutine = null;
    }

    public void PopBulbSad()
    {
        if (_bulbRoutine != null)
        {
            StopCoroutine(_bulbRoutine);
        }
        StopIdleBob(resetPosition: true);

        _bulbRoutine = StartCoroutine(PopBulbSadRoutine());
    }

    private IEnumerator PopBulbSadRoutine()
    {
        bulbTransform.localScale = Vector3.zero;
        bulbTransform.localRotation = Quaternion.identity;
        float riseDuration = popDuration * 0.5f;

        // Moc len yeu ot (khong overshoot) + cui dau xuong
        float elapsed = 0f;
        while (elapsed < riseDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / riseDuration);
            bulbTransform.localScale = Vector3.one * Mathf.Lerp(0f, sadScale, t);
            bulbTransform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(0f, -sadDroopAngle, t));
            yield return null;
        }

        // Lac dau that vong, bien do giam dan ("khong khong khong")
        float shakeDuration = 0.28f;
        float shakeFreq = 22f;
        float shakeAmp = 10f;
        elapsed = 0f;
        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / shakeDuration;
            float damp = 1f - t;
            float angle = -sadDroopAngle + Mathf.Sin(elapsed * shakeFreq) * shakeAmp * damp;
            bulbTransform.localRotation = Quaternion.Euler(0f, 0f, angle);
            yield return null;
        }

        bulbTransform.localScale = Vector3.one * sadScale;
        bulbTransform.localRotation = Quaternion.Euler(0f, 0f, -sadDroopAngle * 0.4f);
        _bulbRoutine = null;
    }

    public void PlayExcited()
    {
        if (_bulbRoutine != null)
        {
            StopCoroutine(_bulbRoutine);
        }
        StopIdleBob(resetPosition: true);

        _bulbRoutine = StartCoroutine(ExcitedRoutine());
    }

    private IEnumerator ExcitedRoutine()
    {
        Vector2 basePos = bulbTransform.anchoredPosition;
        float elapsed = 0f;

        while (elapsed < excitedDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / excitedDuration);
            float hop = Mathf.Sin(t * Mathf.PI) * excitedHopHeight;
            float scale = 1f + Mathf.Sin(t * Mathf.PI) * (excitedScale - 1f);
            bulbTransform.anchoredPosition = basePos + new Vector2(0f, hop);
            bulbTransform.localRotation = Quaternion.Euler(0f, 0f, t * 360f);
            bulbTransform.localScale = Vector3.one * scale;
            yield return null;
        }

        bulbTransform.anchoredPosition = basePos;
        bulbTransform.localRotation = Quaternion.identity;
        bulbTransform.localScale = Vector3.one;
        StartIdleBob();
        _bulbRoutine = null;
    }

    public void StartIdleBob()
    {
        StopIdleBob(resetPosition: true);
        _idleRoutine = StartCoroutine(IdleBobRoutine());
    }

    public void StopIdleBob(bool resetPosition)
    {
        if (_idleRoutine != null)
        {
            StopCoroutine(_idleRoutine);
            _idleRoutine = null;
        }

        if (resetPosition)
        {
            bulbTransform.anchoredPosition = Vector2.zero;
        }
    }

    private IEnumerator IdleBobRoutine()
    {
        float t = 0f;
        while (true)
        {
            t += Time.deltaTime * idleBobSpeed;
            float offsetY = Mathf.Sin(t) * idleBobAmplitude;
            bulbTransform.anchoredPosition = new Vector2(0f, offsetY);
            yield return null;
        }
    }

    public void ShrinkBulb()
    {
        StopIdleBob(resetPosition: true);
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
        bulbTransform.localRotation = Quaternion.identity;
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
