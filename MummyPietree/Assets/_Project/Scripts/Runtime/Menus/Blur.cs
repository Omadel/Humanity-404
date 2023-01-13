using UnityEngine;

namespace MummyPietree
{
    public class Blur : MonoBehaviour
    {
        [SerializeField] private Camera renderCamera;
        private Camera main;

        private void Awake()
        {
            main = Camera.main;
            InputProvider.Instance.OnPauseGame += TurnOn;
            InputProvider.Instance.OnResumeGame += TurnOff;
        }

        private void TurnOn()
        {
            renderCamera.gameObject.SetActive(true);
            main.gameObject.SetActive(false);
        }
        private void TurnOff()
        {
            renderCamera.gameObject.SetActive(false);
            main.gameObject.SetActive(true);
        }
    }
}
