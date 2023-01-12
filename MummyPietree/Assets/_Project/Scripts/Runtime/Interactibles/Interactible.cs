using QuickOutline;
using UnityEngine;

namespace MummyPietree
{
    [RequireComponent(typeof(Outline))]
    public class Interactible : MonoBehaviour
    {
        [SerializeField, Range(-1f,1f)]protected float interactionStress = -.1f;
        [SerializeField]protected float interactionDuration = 1f;

        private Outline outline;

        public virtual bool IsInteractable => true;

        private void Awake()
        {
            outline = GetComponent<Outline>();
        }

        protected virtual void Start()
        {
            outline.enabled = false;
        }

        public void Hover()
        {
            if (!IsInteractable) return;
            outline.enabled = true;
        }

        public void UnHover()
        {
            if (!IsInteractable) return;
            outline.enabled = false;
        }

        public void Click()
        {
            if (!IsInteractable) return;
            outline.OutlineWidth = 4.5f;
        }

        public void Release()
        {
            if (!IsInteractable) return;
            outline.OutlineWidth = 3f;
        }

        public virtual void Interact()
        {
            if (!IsInteractable) return;
            PlayerController.Instance.HandleInteractionStress(interactionStress, interactionDuration, OnInteractionEnded);
        }

        protected virtual void OnInteractionEnded()
        {

        }
    }
}
