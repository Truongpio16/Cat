using System.Collections;
using Cat.Core;
using TMPro;
using UnityEngine;

// Shows a short message when the player presses the hint button (including "no clear hint").
// View layer only: it listens to GridManager.OnHintFeedback and never changes game state.
// Wording lives in Cat.Core.HintText.
public class HintMessageView : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private float visibleSeconds = 1.8f;

    private Coroutine _hideRoutine;

    private void Awake()
    {
        messageText.enabled = false;
    }

    private void OnEnable()
    {
        gridManager.OnHintFeedback += Show;
    }

    private void OnDisable()
    {
        gridManager.OnHintFeedback -= Show;
        _hideRoutine = null;
        if (messageText != null) messageText.enabled = false;
    }

    private void Show(HintMove hint)
    {
        messageText.text = HintText.Describe(hint);
        messageText.enabled = true;

        if (_hideRoutine != null)
        {
            StopCoroutine(_hideRoutine);
        }
        _hideRoutine = StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(visibleSeconds);
        messageText.enabled = false;
        _hideRoutine = null;
    }
}
