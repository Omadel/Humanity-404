using System;
using UnityEngine;
using UnityEngine.UI;

namespace MummyPietree
{
[RequireComponent(typeof(CanvasGroup))]
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private Button playButton;
        [SerializeField] private Button optionsButton;
        [SerializeField] CanvasGroup optionCanvas;
        [SerializeField] private Button quitButton;

        CanvasGroup canvasGroup;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();

            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
            optionCanvas.alpha = 0f;
            optionCanvas.blocksRaycasts = false;
        }


        private void Start()
        {
            playButton.onClick.AddListener(PlayGame);
            optionsButton.onClick.AddListener(OpenOptions);
            quitButton.onClick.AddListener(QuitGame);
        }

        private void OnValidate()
        {
            if (playButton != null) playButton.gameObject.name = "PlayButton";
            if (optionsButton != null) optionsButton.gameObject.name = "OptionsButton";
            if (quitButton != null) quitButton.gameObject.name = "QuitButton";
        }

        private void PlayGame()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(1);
        }

        private void OpenOptions()
        {
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            optionCanvas.alpha = 1f;
            optionCanvas.blocksRaycasts = true;
        }

        private void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
	        Application.Quit();
#endif
        }
    }
}
