using Etienne.Animator2D;
using System;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace EtienneEditor.Animator2D
{
    public class Timeline : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<Timeline> { }

        public event Action<Sprite> OnSelectedSpriteChanged
        {
            add => previewHolder.OnSelectedSpriteChanged += value;
            remove => previewHolder.OnSelectedSpriteChanged -= value;
        }
        public float Value { get => previewHolder.value; set => previewHolder.value = value; }
        public Sprite SelectedSprite => previewHolder.SelectedSprite;

        private SpritePreviewHolder previewHolder;
        private bool isDragging = false;
        private Bar selection;


        public Timeline()
        {
            Toolbar timebar = new Toolbar();
            Add(timebar);
            Toolbar eventbar = new Toolbar();
            Add(eventbar);

            previewHolder = new SpritePreviewHolder();
            Add(previewHolder);

            selection = new Bar(Color.white, "Selection");
            selection.SetLengthUnit(LengthUnit.Percent);
            Add(selection);
            previewHolder.OnValueChanged += selection.SetValue;
            SetEnabled(false);
        }


        public override void HandleEvent(EventBase evt)
        {
            if (!enabledSelf) return;
            if (evt is MouseDownEvent downEvent)
            {
                if (downEvent.button == 0)
                {
                    isDragging = true;
                    previewHolder.SetPixelValue(downEvent.localMousePosition.x);
                }
            }
            if (evt is MouseUpEvent upEvent)
            {
                if (upEvent.button == 0) isDragging = false;
            }
            if (evt is MouseMoveEvent moveEvent)
            {
                if (isDragging) previewHolder.SetPixelValue(moveEvent.localMousePosition.x);
            }
            if (evt is MouseLeaveEvent leaveEvent)
            {
                isDragging = false;
            }
        }

        public void SetAnimation(Animation2D animation, float? percentValue)
        {
            SetEnabled(true);
            previewHolder.SetAnimation(animation, percentValue);
            if (percentValue.HasValue) selection.SetValueWithoutNotify(percentValue.Value);
        }

        public void FirstFrame()
        {
            if (enabledSelf) previewHolder.FirstFrame();
        }
        public void LastFrame()
        {
            if (enabledSelf) previewHolder.LastFrame();
        }
        public void PreviousFrame()
        {
            if (enabledSelf) previewHolder.PreviousFrame();
        }
        public void NextFrame()
        {
            if (enabledSelf) previewHolder.NextFrame();
        }
    }

}
