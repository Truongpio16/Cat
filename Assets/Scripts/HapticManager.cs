using UnityEngine;

public class HapticManager : MonoBehaviour
{
    private const string EnabledPrefKey = "HapticEnabled";

    public bool IsEnabled
    {
        get => PlayerPrefs.GetInt(EnabledPrefKey, 1) == 1;
        set => PlayerPrefs.SetInt(EnabledPrefKey, value ? 1 : 0);
    }

    public void LightTap() => Vibrate();
    public void PlaceFeedback() => Vibrate();
    public void ConflictFeedback() => Vibrate();
    public void WinFeedback() => Vibrate();

    private void Vibrate()
    {
        if (!IsEnabled) return;

#if UNITY_ANDROID || UNITY_IOS
        Handheld.Vibrate();
#endif
    }
}
