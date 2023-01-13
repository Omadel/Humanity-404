using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Etienne.Animator2D
{
    using AnimationState2D = AnimatorController2D.AnimationState2D;
    public class Animator2D : MonoBehaviour
    {
        public AnimationState2D NextAnimationState => nextAnimationState;
        public AnimationState2D CurrentAnimationState => currentAnimationState;
        public AnimationState2D PreviousAnimationState => previousAnimationState;
        public SpriteRenderer Renderer => renderer;

        [SerializeField] private AnimatorController2D controller;
        private AnimationState2D currentAnimationState, nextAnimationState, previousAnimationState;
        private new SpriteRenderer renderer;
        private Dictionary<string, AnimationState2D> animationStates;
        private Coroutine routine;

        private void Start()
        {
            renderer = GetComponentInChildren<SpriteRenderer>();
            animationStates = new Dictionary<string, AnimationState2D>();
            foreach (AnimationState2D animationState in controller.AnimationStates)
            {
                animationStates.Add(animationState.Name, animationState);
            }
            currentAnimationState = controller.AnimationStates[0];
            RestartUpdateRoutine();
        }

        private void OnEnable()
        {
            if (!Application.isPlaying || renderer == null) return;
            currentAnimationState = controller.AnimationStates[0];
            RestartUpdateRoutine();
        }

        private void OnDisable()
        {
            if (!Application.isPlaying) return;
            if (routine != null) StopCoroutine(routine);
        }

        private IEnumerator UpdateRoutine()
        {
            int index = 0;
            while (enabled && Application.isPlaying)
            {
                yield return new WaitUntil(() => renderer.isVisible);
                Animation2D animation = currentAnimationState.Animation;
                if (animation == null)
                {
                    renderer.sprite = null;
                    gameObject.SetActive(false);
                    break;
                }

                if (!currentAnimationState.IsLooping && index+1 >= animation.Sprites.Length)
                {
                    if (currentAnimationState.OutStateName != string.Empty) SetState(currentAnimationState.OutStateName, true);
                    break;
                }

                float frameTime = 1000 / animation.FPS / 1000f;
                index %= animation.Sprites.Length;
                renderer.sprite = animation.Sprites[index];
                ++index;
                yield return new WaitForSeconds(frameTime);
            }
        }

        public bool FlipY(bool? value = null)
        {
            bool oldValue = renderer.flipY;
            renderer.flipY = value ?? !renderer.flipY;
            return renderer.flipY != oldValue;
        }
        public bool FlipX(bool? value = null)
        {
            bool oldValue = renderer.flipX;
            renderer.flipX = value ?? !renderer.flipX;
            return renderer.flipX != oldValue;
        }

        public void SetState(string stateName, bool force = false)
        {
            if (currentAnimationState.Name == stateName) force = false;
            previousAnimationState = currentAnimationState;
            currentAnimationState = animationStates[stateName];
            nextAnimationState = currentAnimationState.OutStateName == "" ? null : animationStates[currentAnimationState.OutStateName];
            if (force) RestartUpdateRoutine();
        }

        public void SetNextState(string nextStateName)
        {
            if (nextAnimationState.Name == nextStateName) return;
            nextAnimationState = animationStates[nextStateName];
        }

        public string GetState()
        {
            return currentAnimationState.Name;
        }

        private void RestartUpdateRoutine()
        {
            if (routine != null) StopCoroutine(routine);
            routine = StartCoroutine(UpdateRoutine());
        }

#if UNITY_EDITOR
        private async void OnValidate()
        {
            await System.Threading.Tasks.Task.Delay(10);
            if (this == null) return;
            Animation2D animation = controller?[0]?.Animation;
            bool isNull = animation == null
                || animation.Sprites == null
                || animation.Sprites.Length <= 0
                || animation.Sprites[0] == null;
            GetComponentInChildren<SpriteRenderer>().sprite = isNull ? null : animation.Sprites[0];
        }
#endif
    }
}