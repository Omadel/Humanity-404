using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MummyPietree
{

    [CreateAssetMenu(menuName = "MummyPietree/Meal")]
    public class MealData : ItemData
    {
        public ItemData[] Recipee => recipee;

        [SerializeField] ItemData[] recipee;
    }
}
