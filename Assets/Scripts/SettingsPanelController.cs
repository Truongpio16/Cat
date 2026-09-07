using UnityEngine;
using UnityEngine.UI;

public class SettingsPanelController : MonoBehaviour
{
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private HapticManager hapticManager;
    [SerializeField] private Toggle soundToggle;
    [SerializeField] private Toggle hapticToggle;
    [SerializeField] private Button backButton;

    private void Awake()
    {
        soundToggle.isOn = audioManager.IsEnabled;
        hapticToggle.isOn = hapticManager.IsEnabled;

        soundToggle.onValueChanged.AddListener(value => audioManager.IsEnabled = value);
        hapticToggle.onValueChanged.AddListener(value => hapticManager.IsEnabled = value);
        backButton.onClick.AddListener(() =>
        {
            audioManager.PlayButtonClick();
            gameObject.SetActive(false);
        });
    }
}
