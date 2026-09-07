using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelButtonView : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Image background;
    [SerializeField] private TMP_Text label;
    [SerializeField] private GameObject lockIcon;

    public event Action OnClicked;

    private static readonly Color NormalColor = new Color(0.7f, 0.85f, 1f);
    private static readonly Color CompletedColor = new Color(0.6f, 0.9f, 0.6f);

    private void Awake()
    {
        button.onClick.AddListener(() => OnClicked?.Invoke());
    }

    public void SetLevel(int displayNumber, bool unlocked, bool completed)
    {
        label.text = displayNumber.ToString();
        button.interactable = unlocked;
        lockIcon.SetActive(!unlocked);
        background.color = completed ? CompletedColor : NormalColor;
    }
}
