using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CellView : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [SerializeField] private CellAnimator animator;
    [SerializeField] private TMP_Text numberText;
    [SerializeField] private GameObject bulbIcon;

    public event Action OnClicked;

    private bool _isWall;
    private bool _wasConflict;

    // Neon "light-up on black" theme (anh 1)
    private static readonly Color WallColor = new Color(0.098f, 0.110f, 0.180f);   // navy dam
    private static readonly Color EmptyColor = new Color(0.929f, 0.890f, 0.729f);  // vang kem mo
    private static readonly Color LitColor = new Color(0.976f, 0.886f, 0.478f);    // vang gold sang
    private static readonly Color ConflictColor = new Color(1f, 0.365f, 0.365f);   // do neon
    private static readonly Color HintColor = new Color(0.353f, 0.831f, 1f);       // cyan (khop subtitle menu)
    private static readonly Color AvoidHintColor = new Color(1f, 0.420f, 0.769f);  // hong (khop tai meo menu)

    public void SetWall(bool hasNumber, int number)
    {
        _isWall = true;
        animator.SetColorInstant(WallColor);
        numberText.gameObject.SetActive(hasNumber);
        numberText.text = hasNumber ? number.ToString() : "";
        bulbIcon.SetActive(false);
    }

    public void SetEmpty()
    {
        _isWall = false;
        animator.SetColorInstant(EmptyColor);
        numberText.gameObject.SetActive(false);
        bulbIcon.SetActive(false);
    }

    public void SetBulb(bool active)
    {
        if (active)
        {
            bulbIcon.SetActive(true);
            animator.PopBulb();
        }
        else
        {
            animator.ShrinkBulb();
        }
    }

    public void SetLit(bool lit)
    {
        if (_isWall) return;
        animator.AnimateColorTo(lit ? LitColor : EmptyColor);
    }

    public void SetConflict(bool hasConflict)
    {
        if (_isWall) return;

        if (hasConflict)
        {
            animator.AnimateColorTo(ConflictColor);
            if (!_wasConflict)
            {
                animator.Pulse();
            }
        }

        _wasConflict = hasConflict;
    }

    public void SetHint(Cat.Core.HintType type)
    {
        if (_isWall) return;
        Color target = type == Cat.Core.HintType.PlaceBulb ? HintColor : AvoidHintColor;
        animator.AnimateColorTo(target);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClicked?.Invoke();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_isWall) return;
        animator.PressDown();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        animator.PressUp();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        animator.PressUp();
    }

    public void PlayWinPulse()
    {
        if (_isWall) return;
        animator.Pulse();
    }
}
