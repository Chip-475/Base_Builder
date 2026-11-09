using System;
using UnityEngine;
using UnityEngine.Audio;
using DG.Tweening;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    [Serializable]
    public class NamedAudioClip
    {
        public string key;
        public AudioClip clip;
    }

    #region Declarations
    public static AudioManager instance;
    public static AudioManager Instance => instance;

    public const string MasterVolumeKey = "Master Volume";
    public const string SFXVolumeKey = "SFX Volume";
    public const string BGMVolumeKey = "BGM Volume";
    public const float DefaultVolume = 1f;

    [SerializeField] List<NamedAudioClip> audioClipEntries = new();
    public Dictionary<string, AudioClip> clips { get; private set; } = new();
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

        clips.Clear();
        foreach (NamedAudioClip entry in audioClipEntries)
        {
            if (entry == null || string.IsNullOrWhiteSpace(entry.key) || entry.clip == null)
                continue;

            clips[entry.key] = entry.clip;
        }

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
    public static AudioSource PlaySFX(string clipKey, Transform spawn, float volume = 1f)
    {
        return PlaySFX(GetClip(clipKey), spawn, volume);
    }

    public static AudioSource PlaySFX(AudioClip clip, Transform spawn, float volume = 1f)
    {
        if (instance == null || instance.source == null || clip == null || spawn == null)
            return null;

        AudioSource audioSource = Instantiate(instance.source, spawn.position, Quaternion.identity);
        audioSource.outputAudioMixerGroup = instance.sfxMixerGroup;
        audioSource.clip = clip;
        audioSource.loop = false;
        audioSource.volume = Mathf.Clamp01(volume);
        audioSource.Play();
        Destroy(audioSource.gameObject, audioSource.clip.length);
        return audioSource;
    }

    public static AudioSource PlaySFX(string clipKey, Vector3 position, float volume = 1f)
    {
        return PlaySFX(GetClip(clipKey), position, volume);
    }

    public static AudioSource PlaySFX(AudioClip clip, Vector3 position, float volume = 1f)
    {
        if (instance == null || instance.source == null || clip == null)
            return null;

        AudioSource audioSource = Instantiate(instance.source, position, Quaternion.identity);
        audioSource.outputAudioMixerGroup = instance.sfxMixerGroup;
        audioSource.clip = clip;
        audioSource.loop = false;
        audioSource.volume = Mathf.Clamp01(volume);
        audioSource.Play();
        Destroy(audioSource.gameObject, audioSource.clip.length);
        return audioSource;
    }

    public static AudioClip GetClip(string key)
    {
        if (instance == null || string.IsNullOrWhiteSpace(key))
            return null;

        return instance.clips.TryGetValue(key, out AudioClip clip) ? clip : null;
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
