using Etienne;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MummyPietree
{
    [DefaultExecutionOrder(-1)]
    public class InputProvider : Singleton<InputProvider>
    {
        public event Action<Vector2> OnLeftMouseButtonPressed, OnLeftMouseButtonReleased,
            OnMouseDrag,
            OnRightMouseButtonPressed, OnRightMouseButtonReleased;
        public event Action OnPauseGame, OnResumeGame;
        public Vector2 MousePosition => Mouse.current.position.ReadValue();

        private float leftButtonState = 0;
        private float rightButtonState = 0;

        private void Update()
        {
            if (Time.timeScale <= 0f) return;
            HandleMouseLeftButton();
            HandleMouseRightButton();
        }

        private void HandleMouseLeftButton()
        {
            float oldMouseState = leftButtonState;
            Mouse mouse = Mouse.current;
            leftButtonState = mouse.leftButton.ReadValue();
            if (leftButtonState != oldMouseState)
            {
                if (leftButtonState > 0f) OnLeftMouseButtonPressed?.Invoke(MousePosition);
                else OnLeftMouseButtonReleased?.Invoke(MousePosition);
            }
            else
            {
                if (leftButtonState > 0f) OnMouseDrag?.Invoke(MousePosition);
            }
        }

        private void HandleMouseRightButton()
        {
            float oldMouseState = rightButtonState;
            Mouse mouse = Mouse.current;
            rightButtonState = mouse.rightButton.ReadValue();
            if (rightButtonState != oldMouseState)
            {
                if (rightButtonState > 0f) OnRightMouseButtonPressed?.Invoke(MousePosition);
                else OnRightMouseButtonReleased?.Invoke(MousePosition);
            }
        }

        private void OnPause(InputValue value)
        {
            if (Time.timeScale <= 0f)
            {
                OnResumeGame?.Invoke();
            }
            else
            {
                OnPauseGame?.Invoke();
            }
        }
    }
}
