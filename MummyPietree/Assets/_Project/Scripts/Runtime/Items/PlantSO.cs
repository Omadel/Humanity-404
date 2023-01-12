using UnityEngine;

namespace MummyPietree
{
    [CreateAssetMenu(menuName = "MummyPietree/Plant")]
    public  class PlantSO : ItemSO
    {
        [SerializeField] private float itemGrowthDuration;
    }


}
