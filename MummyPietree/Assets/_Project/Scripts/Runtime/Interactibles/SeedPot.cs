using System.Linq;
using UnityEngine;

namespace MummyPietree
{
    public class SeedPot : Interactable
    {
        public override bool IsInteractable
        {
            get
            {
                bool isFull = sellingItems.Where(i => !i.HasItem).FirstOrDefault() == null;
                if (isFull && !PlayerController.Instance.HasItem) return true;
                return !isFull;
            }
        }

        [SerializeField] private SeedData seed;
        [SerializeField] private SellingItem[] sellingItems;

        protected override void Start()
        {
            base.Start();
            sellingItems = GetComponentsInChildren<SellingItem>();
        }

        protected override void OnInteractionEnded()
        {
            if (PlayerController.Instance.HasItem)
            {
                StockItemForSale(PlayerController.Instance.UseTransportedItem());
            }
            else
            {
                BuySeedAndTransport();
            }
        }

        private void StockItemForSale(ItemData itemData)
        {
            Item emptyItem = sellingItems.Where(i => !i.HasItem).FirstOrDefault();
            emptyItem.SetItem(itemData);
        }

        private void BuySeedAndTransport()
        {
            PlayerController.Instance.TransportItem(seed);
        }
    }
}
