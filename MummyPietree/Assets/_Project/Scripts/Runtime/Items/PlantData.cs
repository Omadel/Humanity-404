using UnityEngine;

namespace MummyPietree
{
    [CreateAssetMenu(menuName = "MummyPietree/Plant")]
    public  class PlantData : ItemData
    {
        [SerializeField] private float itemGrowthDuration;
    }


}
