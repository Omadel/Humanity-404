using Etienne;
using UnityEngine;

namespace MummyPietree
{
    public abstract class ItemData : ScriptableObject
    {
        public Sprite ItemSprite => itemSprite;
        public int Price=> price;

        [SerializeField, PreviewSprite] private Sprite itemSprite;
        [SerializeField] int price = 100;
    }


}
