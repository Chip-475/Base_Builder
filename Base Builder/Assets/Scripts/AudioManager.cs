using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    public const float MaxVolume = 100f;

    [Header("Setup")]
    [SerializeField] List<AudioClip> sfxClips = new();
    [SerializeField] List<AudioClip> bgmClips = new();

    [Header("Runtime")]
    AudioClip currentBgmPlaying;
    float masterVolume;
    float sfxVolume;
    float bgmVolume;

    public float MasterVolume => masterVolume;
    public float SfxVolume => sfxVolume;
    public float BgmVolume => bgmVolume;

    private void Awake()
    {
        Instance = this;

        PlayBGM(bgmClips[0]);
    }
    
    public void PlaySFX(AudioClip clip)
    {
        if (!sfxClips.Contains(clip))
        {
            Debug.Log("Clip is not in SFX.");
            return;
        }

        AudioSource source = new GameObject().AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = sfxVolume * masterVolume / MaxVolume;
        Destroy(source.gameObject, clip.length + 0.5f);
    }
    public void PlayBGM(AudioClip clip)
    {
        if (!bgmClips.Contains(clip))
        {
            Debug.Log("Clip is not in BGM.");
            return;
        }
        if(clip == currentBgmPlaying) return;

        AudioSource source = new GameObject().AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = bgmVolume * masterVolume / MaxVolume;
    }

    public static float VolumeToDb(float volume)
    {
        return volume <= 0f ? -80f : 20f * Mathf.Log10(volume / MaxVolume);
    }

    public static void SetVolume(string volumeName, float volume)
    {

    }
}
