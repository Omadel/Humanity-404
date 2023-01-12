using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MummyPietree
{
    public class PlantPatch : Interactible
    {
        public override bool IsInteractable => !plant.HasSeed;

        Plant plant;

        protected override void Start()
        {
            base.Start();
            plant = GetComponentInChildren<Plant>();
        }

        protected override void OnInteractionEnded()
        {
            plant.SowPlant();
            PlayerController.Instance.UseTransportedItem();
        }
    }
}
