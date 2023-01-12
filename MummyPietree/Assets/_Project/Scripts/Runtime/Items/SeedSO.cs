using UnityEngine;

namespace MummyPietree
{
    [CreateAssetMenu(menuName = "MummyPietree/Seed")]
    public class SeedSO : ItemSO
    {
        public PlantSO GrownPlant => grownPlant;
        [SerializeField] PlantSO grownPlant;

    }


}
