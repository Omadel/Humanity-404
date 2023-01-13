using System;
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

        internal int CalculateSellingPrice()
        {
            int price = 0;
            foreach (SellingItem item in sellingItems)
            {
                if (item.HasItem) price += item.ItemSO.Price;
            }
            SellAll();

            return price;
        }

        private void SellAll()
        {
            foreach (var item in sellingItems)
            {
                item.RemoveItem();
            }
        }

        private void BuySeedAndTransport()
        {
            PlayerController.Instance.TransportItem(seed);
        }
    }
}
