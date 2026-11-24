using System.Collections;
using UnityEngine;

// Hoat anh "song" cho nhan vat meo o Main Menu: tho phong-xep, vay duoi,
// ve tai nhe, thinh thoang ngoanh dau. earLeft/earRight/tail co the de
// trong neu asset hien tai la 1 anh phang - script van chay an toan.
public class CatMenuAnimator : MonoBehaviour
{
    [SerializeField] private RectTransform body;
    [SerializeField] private RectTransform earLeft;
    [SerializeField] private RectTransform earRight;
    [SerializeField] private RectTransform tail;

    [SerializeField] private float breatheScale = 0.04f;
    [SerializeField] private float breatheSpeed = 1.2f;
    [SerializeField] private float tailWagAngle = 10f;
    [SerializeField] private float tailWagSpeed = 1.8f;
    [SerializeField] private float earTwitchAngle = 6f;
    [SerializeField] private float lookAroundMinInterval = 3f;
    [SerializeField] private float lookAroundMaxInterval = 7f;
    [SerializeField] private float lookAroundAngle = 6f;

    private Vector3 _bodyBaseScale = Vector3.one;
    private float _lookOffset;

    private void Awake()
    {
        if (body != null)
        {
            _bodyBaseScale = body.localScale;
        }
    }

    private void Start()
    {
        StartCoroutine(LookAroundRoutine());
    }

    private void Update()
    {
        float t = Time.time;

        if (body != null)
        {
            float s = 1f + Mathf.Sin(t * breatheSpeed) * breatheScale;
            body.localScale = new Vector3(_bodyBaseScale.x * s, _bodyBaseScale.y * s, _bodyBaseScale.z);
            body.localRotation = Quaternion.Euler(0f, 0f, _lookOffset);
        }

        if (tail != null)
        {
            float angle = Mathf.Sin(t * tailWagSpeed) * tailWagAngle;
            tail.localRotation = Quaternion.Euler(0f, 0f, angle);
        }

        if (earLeft != null)
        {
            earLeft.localRotation = Quaternion.Euler(0f, 0f, Mathf.Sin(t * 2.1f) * earTwitchAngle);
        }

        if (earRight != null)
        {
            earRight.localRotation = Quaternion.Euler(0f, 0f, Mathf.Sin(t * 2.1f + 0.5f) * earTwitchAngle);
        }
    }

    private IEnumerator LookAroundRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(lookAroundMinInterval, lookAroundMaxInterval));
            yield return StartCoroutine(TurnTo(Random.Range(-lookAroundAngle, lookAroundAngle)));
            yield return new WaitForSeconds(1.2f);
            yield return StartCoroutine(TurnTo(0f));
        }
    }

    private IEnumerator TurnTo(float target)
    {
        float duration = 0.4f;
        float elapsed = 0f;
        float start = _lookOffset;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            _lookOffset = Mathf.Lerp(start, target, elapsed / duration);
            yield return null;
        }

        _lookOffset = target;
    }
}
