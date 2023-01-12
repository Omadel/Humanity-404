namespace MummyPietree
{
    public class PlantPatch : Interactable
    {
        public override bool IsInteractable => !plant.HasSeed && PlayerController.Instance.HasItem && PlayerController.Instance.TransportedItem is SeedSO;

        private Plant plant;

        protected override void Start()
        {
            base.Start();
            plant = GetComponentInChildren<Plant>();
        }

        protected override void OnInteractionEnded()
        {
            SeedSO seed = PlayerController.Instance.UseTransportedItem() as SeedSO;
            plant.SowPlant(seed);
        }
    }
}
