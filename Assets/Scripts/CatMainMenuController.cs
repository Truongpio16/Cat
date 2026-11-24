using System.Collections;
using UnityEngine;

// Dieu phoi chung cho Main Menu: choi hieu ung xuat hien (title roi den
// tung nut) khi mo scene. KHONG dung logic chuyen scene o day - HomeManager
// van la noi duy nhat xu ly click nut va chuyen canh.
public class CatMainMenuController : MonoBehaviour
{
    [SerializeField] private CanvasGroup titleGroup;
    [SerializeField] private CanvasGroup[] buttonGroups;
    [SerializeField] private float titleDuration = 0.4f;
    [SerializeField] private float buttonStagger = 0.12f;
    [SerializeField] private float buttonDuration = 0.3f;
    [SerializeField] private float slideOffset = 40f;

    private void Start()
    {
        StartCoroutine(EntranceSequence());
    }

    private IEnumerator EntranceSequence()
    {
        if (titleGroup != null)
        {
            yield return StartCoroutine(FadeScaleIn(titleGroup, titleDuration));
        }

        if (buttonGroups != null)
        {
            foreach (CanvasGroup group in buttonGroups)
            {
                if (group == null) continue;
                StartCoroutine(FadeSlideIn(group, buttonDuration));
                yield return new WaitForSeconds(buttonStagger);
            }
        }
    }

    private IEnumerator FadeScaleIn(CanvasGroup group, float duration)
    {
        RectTransform rt = group.GetComponent<RectTransform>();
        Vector3 baseScale = rt.localScale;
        Vector3 startScale = baseScale * 0.85f;

        group.alpha = 0f;
        rt.localScale = startScale;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            group.alpha = t;
            rt.localScale = Vector3.Lerp(startScale, baseScale, t);
            yield return null;
        }

        group.alpha = 1f;
        rt.localScale = baseScale;
    }

    private IEnumerator FadeSlideIn(CanvasGroup group, float duration)
    {
        RectTransform rt = group.GetComponent<RectTransform>();
        Vector2 basePos = rt.anchoredPosition;
        Vector2 startPos = basePos + new Vector2(0f, -slideOffset);

        group.alpha = 0f;
        rt.anchoredPosition = startPos;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            group.alpha = t;
            rt.anchoredPosition = Vector2.Lerp(startPos, basePos, t);
            yield return null;
        }

        group.alpha = 1f;
        rt.anchoredPosition = basePos;
    }
}
