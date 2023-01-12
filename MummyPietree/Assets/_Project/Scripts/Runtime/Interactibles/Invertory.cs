using System.Linq;
using UnityEngine;

namespace MummyPietree
{
    public class Invertory : Interactable
    {
        public override bool IsInteractable => PlayerController.Instance.HasItem;

        [SerializeField] private Item[] items;

        protected override void Start()
        {
            base.Start();
        }

        public override void Interact()
        {
            if (!IsInteractable) return;
            PlayerController.Instance.HandleInteractionStress(interactionStress, interactionDuration, UseTransportedItem);
        }

        private void UseTransportedItem()
        {
            ItemSO item = PlayerController.Instance.UseTransportedItem();
            Item emptyItem = items.Where(i => !i.HasItem).FirstOrDefault();
            emptyItem.SetItem(item);
        }
    }
}
