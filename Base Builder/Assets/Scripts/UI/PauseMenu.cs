using Unity.VectorGraphics;
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

        saveButton.onClick.AddListener(() => SaveManager.Instance.file.SaveAll(SaveManager.Instance.savePath));
        backToMenuButton.onClick.AddListener(() => SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Single));

        //masterSlider.onValueChanged.AddListener();

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
        if (AudioManager.Instance == null)
            return;

        SetSliderValue(masterSlider, AudioManager.Instance.MasterVolume);
        SetSliderValue(sfxSlider, AudioManager.Instance.SfxVolume);
        SetSliderValue(bgmSlider, AudioManager.Instance.BgmVolume);
    }

    public static void SetSliderValue(Slider slider, float volume)
    {
        if (slider == null)
            return;

        slider.SetValueWithoutNotify(Mathf.Clamp01(volume / AudioManager.MaxVolume));
    }
}
