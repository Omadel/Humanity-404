using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MummyPietree
{
    public class SeedPot : Interactable
    {
        public override bool IsInteractable => !PlayerController.Instance.HasItem;

        [SerializeField] SeedData seed;

        protected override void OnInteractionEnded()
        {
            BuySeedAndTransport();
        }

        private void BuySeedAndTransport()
        {
            PlayerController.Instance.TransportItem(seed);
        }
    }
}
