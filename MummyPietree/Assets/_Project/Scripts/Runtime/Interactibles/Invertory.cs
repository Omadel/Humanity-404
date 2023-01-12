using System.Linq;
using UnityEngine;

namespace MummyPietree
{
    public class Invertory : Interactible
    {
        public override bool IsInteractable => PlayerController.Instance.TransportsItem;

        [SerializeField] private SpriteRenderer[] items;

        protected override void Start()
        {
            base.Start();
            foreach (SpriteRenderer item in items)
            {
                item.enabled = false;
            }
        }

        public override void Interact()
        {
            if (!IsInteractable) return;
            PlayerController.Instance.HandleInteractionStress(interactionStress, interactionDuration, UseTransportedItem);
        }

        private void UseTransportedItem()
        {
            Sprite item = PlayerController.Instance.UseTransportedItem();
            SpriteRenderer emptyItem = items.Where(i => !i.enabled).FirstOrDefault();
            emptyItem.enabled = item != null;
            emptyItem.sprite = item;
        }
    }
}
