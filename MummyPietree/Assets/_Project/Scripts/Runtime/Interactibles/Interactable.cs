using QuickOutline;
using UnityEngine;

namespace MummyPietree
{
    [RequireComponent(typeof(Outline))]
    public class Interactable : MonoBehaviour
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
            outline.enabled = true;
        }

        public void UnHover()
        {
            outline.enabled = false;
        }

        public void Click()
        {
            outline.OutlineWidth = 4.5f;
        }

        public void Release()
        {
            outline.OutlineWidth = 3f;
        }

        public virtual void Interact()
        {
                PlayerController.Instance.HandleInteractionStress(interactionStress, interactionDuration, OnInteractionEnded);
        }

        protected virtual void OnInteractionEnded()
        {

        }

 
    }
}
