using QuickOutline;
using UnityEngine;

namespace MummyPietree
{
    [RequireComponent(typeof(Outline))]
    public class Interactible : MonoBehaviour
    {
        [SerializeField, Range(0f,1f)] float interactionStress = -.1f;

        private Outline outline;
        private void Awake()
        {
            outline = GetComponent<Outline>();
        }

        private void Start()
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

        public void Interact()
        {
            PlayerController.Instance.HandleInteractionStress(interactionStress);
        }
    }
}
