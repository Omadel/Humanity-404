using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MummyPietree
{
    public class SeedPot : Interactible
    {
        [SerializeField] Sprite seed;

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
