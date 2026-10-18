using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Setup")]
    [SerializeField] List<AudioClip> sfxClips = new();
    [SerializeField] List<AudioClip> bgmClips = new();

    [Header("Runtime")]
    AudioClip currentBgmPlaying;
    [Tooltip("0 - 100")] float masterVolume;
    float sfxVolume;
    float bgmVolume;

    private void Awake()
    {
        Instance = this;
    }
    
    public void PlaySFX(AudioClip clip)
    {
        if (!sfxClips.Contains(clip))
        {
            Debug.Log("Clip is not a SFX.");
            return;
        }

        AudioSource source = new GameObject().AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = sfxVolume * masterVolume / 100;
        Destroy(source.gameObject, clip.length + 0.5f);
    }
    public void PlayBGM(AudioClip clip)
    {
        if (!bgmClips.Contains(clip))
        {
            Debug.Log("Clip is not a BGM.");
            return;
        }
        if(clip == currentBgmPlaying) return;

        AudioSource source = new GameObject().AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = bgmVolume * masterVolume / 100;
    }

    public static float VolumeToDb(float volume)
    {
        return volume <= 0f ? -80f : 20f * Mathf.Log10(volume / 100f);
    }
}
