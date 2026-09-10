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

    private static readonly Color NormalColor = new Color(0.075f, 0.10f, 0.16f, 0.96f);
    private static readonly Color CompletedColor = new Color(0.20f, 0.15f, 0.045f, 0.96f);

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
