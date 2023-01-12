using UnityEngine;

namespace MummyPietree
{
    [CreateAssetMenu(menuName = "MummyPietree/Seed")]
    public class SeedData : ItemData
    {
        public PlantData GrownPlant => grownPlant;
        [SerializeField] PlantData grownPlant;

    }


}
