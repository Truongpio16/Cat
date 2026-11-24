using System;
using System.Collections.Generic;
using UnityEngine;

// Chuyen dong nen nho: hoa/co dung dua, firefly bay lon. Danh sach de trong
// van chay binh thuong (khong lam gi), nen co the gan dan asset sau.
public class CatBackgroundAnimator : MonoBehaviour
{
    [Serializable]
    public class SwayItem
    {
        public RectTransform target;
        public float angle = 6f;
        public float speed = 1.5f;
        public float phaseOffset;
    }

    [Serializable]
    public class FireflyItem
    {
        public RectTransform target;
        public CanvasGroup canvasGroup;
        public float radius = 40f;
        public float speed = 0.6f;
        public float phaseOffset;

        [NonSerialized] public Vector2 BasePos;
    }

    [SerializeField] private List<SwayItem> swayItems = new List<SwayItem>();
    [SerializeField] private List<FireflyItem> fireflies = new List<FireflyItem>();

    private void Start()
    {
        foreach (FireflyItem f in fireflies)
        {
            if (f.target != null)
            {
                f.BasePos = f.target.anchoredPosition;
            }
        }
    }

    private void Update()
    {
        float t = Time.time;

        foreach (SwayItem item in swayItems)
        {
            if (item.target == null) continue;
            float angle = Mathf.Sin((t + item.phaseOffset) * item.speed) * item.angle;
            item.target.localRotation = Quaternion.Euler(0f, 0f, angle);
        }

        foreach (FireflyItem f in fireflies)
        {
            if (f.target == null) continue;

            float x = Mathf.Sin((t + f.phaseOffset) * f.speed) * f.radius;
            float y = Mathf.Cos((t + f.phaseOffset) * f.speed * 0.7f) * f.radius * 0.6f;
            f.target.anchoredPosition = f.BasePos + new Vector2(x, y);

            if (f.canvasGroup != null)
            {
                f.canvasGroup.alpha = 0.5f + 0.5f * Mathf.Sin((t + f.phaseOffset) * f.speed * 1.3f);
            }
        }
    }
}
