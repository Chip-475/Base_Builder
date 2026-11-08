using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public PauseMenu Instance { get; private set; }

    [SerializeField] Button saveButton;
    [SerializeField] Button backToMenuButton;
    [SerializeField] Slider masterSlider;
    [SerializeField] Slider sfxSlider;
    [SerializeField] Slider bgmSlider;

    private void Awake()
    {
        Instance = this;

        if (saveButton != null)
            saveButton.onClick.AddListener(() => SaveManager.Instance.file.SaveAll(SaveManager.Instance.savePath));
        if (backToMenuButton != null)
            backToMenuButton.onClick.AddListener(() => SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Single));

        if (masterSlider != null)
            masterSlider.onValueChanged.AddListener(AudioManager.SetMasterVolume);
        if (sfxSlider != null)
            sfxSlider.onValueChanged.AddListener(AudioManager.SetSfxVolume);
        if (bgmSlider != null)
            bgmSlider.onValueChanged.AddListener(AudioManager.SetBgmVolume);

        PlayerManager.Inputs.Player.Pause.performed += (_) =>
        {
            gameObject.SetActive(!gameObject.activeSelf);
            if (gameObject.activeSelf)
                SyncVolumeSliders();
        };
        gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        SyncVolumeSliders();
    }

    void SyncVolumeSliders()
    {
        SetSliderValue(masterSlider, AudioManager.GetSavedVolume(AudioManager.MasterVolumeKey));
        SetSliderValue(sfxSlider, AudioManager.GetSavedVolume(AudioManager.SFXVolumeKey));
        SetSliderValue(bgmSlider, AudioManager.GetSavedVolume(AudioManager.BGMVolumeKey));
    }

    public static void SetSliderValue(Slider slider, float volume)
    {
        if (slider == null)
            return;

        slider.SetValueWithoutNotify(Mathf.Clamp01(volume));
    }
}
