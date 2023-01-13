using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace MummyPietree
{
    [RequireComponent(typeof(CanvasGroup))]
    public class OptionMenu : MonoBehaviour
    {
        [SerializeField] private CanvasGroup returnCanvas;
        [SerializeField] private Button returnButton;
        [SerializeField] private Slider masterSlider, musicSlider, sfxSlider;
        [SerializeField] private AudioMixer mixer;
        private CanvasGroup canvasGroup;
        bool isMainMenu;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            mixer.GetFloat("MasterVolume", out float value);
            masterSlider.SetValueWithoutNotify(Mathf.Pow(10, value / 20f));
            mixer.GetFloat("MusicVolume", out value);
            musicSlider.SetValueWithoutNotify(Mathf.Pow(10, value / 20f));
            mixer.GetFloat("SFXVolume", out value);
            sfxSlider.SetValueWithoutNotify(Mathf.Pow(10, value / 20f));

            var inputs = InputProvider.Instance;
            isMainMenu = inputs == null;
            if (!isMainMenu)
            {
            inputs.OnPauseGame += PauseGame;
            inputs.OnResumeGame += ResumeGame;
            ResumeGame();
            }
        }

        private void ResumeGame()
        {
            returnCanvas.alpha = 0f;
            returnCanvas.blocksRaycasts = false;
            Time.timeScale = 1f;
        }

        private void PauseGame()
        {
            returnCanvas.alpha = 1f;
            returnCanvas.blocksRaycasts = true;
            Time.timeScale = 0f;
        }

        private void Start()
        {
            returnButton.onClick.AddListener(Return);
            masterSlider.onValueChanged.AddListener(ChangeMasterVolume);
            musicSlider.onValueChanged.AddListener(ChangeMusicVolume);
            sfxSlider.onValueChanged.AddListener(ChangeSFXVolume);
        }

        private void ChangeMasterVolume(float value)
        {
            SetMixerFloat("MasterVolume", value);
        }
        private void ChangeMusicVolume(float value)
        {
            SetMixerFloat("MusicVolume", value);
        }
        private void ChangeSFXVolume(float value)
        {
            SetMixerFloat("SFXVolume", value);
        }
        private void SetMixerFloat(string floatName, float value)
        {
            mixer.SetFloat(floatName, Mathf.Log10(value) * 20f);
        }

        public void Return()
        {
            if (isMainMenu)
            {
                canvasGroup.alpha = 0f;
                canvasGroup.blocksRaycasts = false;
                returnCanvas.alpha = 1f;
                returnCanvas.blocksRaycasts = true;
            }
            else
            {
                ResumeGame();
            }
        }
    }
}
