namespace MummyPietree
{
    public class PlantPatch : Interactable
    {
        public override bool IsInteractable => !plant.HasSeed && PlayerController.Instance.HasItem && PlayerController.Instance.TransportedItem is SeedData;

        private Plant plant;

        protected override void Start()
        {
            base.Start();
            plant = GetComponentInChildren<Plant>();
        }

        protected override void OnInteractionEnded()
        {
            SeedData seed = PlayerController.Instance.UseTransportedItem() as SeedData;
            plant.SowPlant(seed);
        }
    }
}
