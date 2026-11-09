using UnityEngine;
using UnityEngine.Audio;
using DG.Tweening;

public class AudioManager : MonoBehaviour
{
    #region Declarations
    public static AudioManager instance;
    public static AudioManager Instance => instance;

    public const string MasterVolumeKey = "Master Volume";
    public const string SFXVolumeKey = "SFX Volume";
    public const string BGMVolumeKey = "BGM Volume";
    public const float DefaultVolume = 1f;

    public AudioSource source;
    public AudioMixer mixer;
    public AudioMixerGroup sfxMixerGroup;
    public AudioMixerGroup bgmMixerGroup;

    public AudioSource bgmPlayer;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        if (sfxMixerGroup == null && mixer != null)
        {
            AudioMixerGroup[] groups = mixer.FindMatchingGroups("SFX");
            if (groups.Length > 0)
                sfxMixerGroup = groups[0];
        }

        if (bgmMixerGroup == null && mixer != null)
        {
            AudioMixerGroup[] groups = mixer.FindMatchingGroups("BGM");
            if (groups.Length > 0)
                bgmMixerGroup = groups[0];
        }

        ApplySavedVolumes();
    }

    private void Start()
    {
        if (bgmPlayer == null)
            bgmPlayer = source;

        if (bgmPlayer == null)
        {
            Debug.LogWarning("AudioManager has no main theme audio source assigned.");
        }
        else
        {
            bgmPlayer = Instantiate(bgmPlayer, transform);
            bgmPlayer.outputAudioMixerGroup = bgmMixerGroup;
            bgmPlayer.loop = true;
            bgmPlayer.volume = 0f;
            bgmPlayer.Play();
            FadeVolume(bgmPlayer, 1f);
        }
    }
    #endregion

    #region SFX
    public static void PlaySFX(AudioClip clip, Transform spawn, float volume = 1f)
    {
        if (instance == null || instance.source == null || clip == null || spawn == null)
            return;

        AudioSource audioSource = Instantiate(instance.source, spawn.position, Quaternion.identity);
        audioSource.outputAudioMixerGroup = instance.sfxMixerGroup;
        audioSource.clip = clip;
        audioSource.loop = false;
        audioSource.volume = Mathf.Clamp01(volume);
        audioSource.Play();
        Destroy(audioSource.gameObject, audioSource.clip.length);
    }

    public static void PlaySFX(AudioClip clip, Vector3 position, float volume = 1f)
    {
        if (instance == null || instance.source == null || clip == null)
            return;

        AudioSource audioSource = Instantiate(instance.source, position, Quaternion.identity);
        audioSource.outputAudioMixerGroup = instance.sfxMixerGroup;
        audioSource.clip = clip;
        audioSource.loop = false;
        audioSource.volume = Mathf.Clamp01(volume);
        audioSource.Play();
        Destroy(audioSource.gameObject, audioSource.clip.length);
    }
    #endregion

    #region Volume
    public static float GetSavedVolume(string key)
    {
        return Mathf.Clamp01(PlayerPrefs.GetFloat(key, DefaultVolume));
    }

    public static void ApplySavedVolumes()
    {
        SetMixerVolume("Master", GetSavedVolume(MasterVolumeKey));
        SetMixerVolume("SFX", GetSavedVolume(SFXVolumeKey));
        SetMixerVolume("BGM", GetSavedVolume(BGMVolumeKey));
    }

    public static void SetMasterVolume(float volume)
    {
        SetSavedVolume(MasterVolumeKey, "Master", volume);
    }

    public static void SetSfxVolume(float volume)
    {
        SetSavedVolume(SFXVolumeKey, "SFX", volume);
    }

    public static void SetBgmVolume(float volume)
    {
        SetSavedVolume(BGMVolumeKey, "BGM", volume);
    }

    public static void SetMixerVolume(string parameter, float volume)
    {
        if (instance == null)
        {
            Debug.LogWarning("AudioManager is not initialized.");
            return;
        }

        if (instance.mixer == null)
        {
            Debug.LogWarning("AudioManager has no AudioMixer assigned.");
            return;
        }

        volume = Mathf.Clamp01(volume);
        if (!instance.mixer.SetFloat(parameter, VolumeToDb(volume)))
            Debug.LogWarning($"AudioMixer parameter '{parameter}' is not exposed.");
    }

    public static float VolumeToDb(float volume)
    {
        volume = Mathf.Clamp01(volume);
        return volume <= 0f ? -80f : Mathf.Log10(volume) * 20f;
    }

    private static void SetSavedVolume(string key, string parameter, float volume)
    {
        volume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat(key, volume);
        PlayerPrefs.Save();
        SetMixerVolume(parameter, volume);
    }
    #endregion

    #region Music
    public static void FadeVolume(AudioSource audioSource, float endVolume)
    {
        if (audioSource == null)
            return;

        audioSource.DOKill();
        DOTween.To(() => audioSource.volume,
            volume => audioSource.volume = volume,
            Mathf.Clamp01(endVolume),
            0.2f);
    }
    #endregion
}
