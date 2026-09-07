using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioClip buttonClickClip;
    [SerializeField] private AudioClip placeCatClip;
    [SerializeField] private AudioClip removeCatClip;
    [SerializeField] private AudioClip conflictClip;
    [SerializeField] private AudioClip hintClip;
    [SerializeField] private AudioClip winClip;
    [SerializeField] private AudioClip backgroundMusicClip;

    private const string EnabledPrefKey = "AudioEnabled";

    public bool IsEnabled
    {
        get => PlayerPrefs.GetInt(EnabledPrefKey, 1) == 1;
        set
        {
            PlayerPrefs.SetInt(EnabledPrefKey, value ? 1 : 0);
            if (musicSource != null) musicSource.mute = !value;
        }
    }

    private void Start()
    {
        if (musicSource != null && backgroundMusicClip != null)
        {
            musicSource.clip = backgroundMusicClip;
            musicSource.loop = true;
            musicSource.mute = !IsEnabled;
            musicSource.Play();
        }
    }

    public void PlayButtonClick() => Play(buttonClickClip);
    public void PlayPlaceCat() => Play(placeCatClip);
    public void PlayRemoveCat() => Play(removeCatClip);
    public void PlayConflict() => Play(conflictClip);
    public void PlayHint() => Play(hintClip);
    public void PlayWin() => Play(winClip);

    private void Play(AudioClip clip)
    {
        if (!IsEnabled || clip == null) return;
        audioSource.PlayOneShot(clip);
    }
}
