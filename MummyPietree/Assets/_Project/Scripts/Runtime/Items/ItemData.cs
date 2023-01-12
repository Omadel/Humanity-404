using Etienne;
using UnityEngine;

namespace MummyPietree
{
    public abstract class ItemData : ScriptableObject
    {
        public Sprite ItemSprite => itemSprite;

        [SerializeField, PreviewSprite] private Sprite itemSprite;
    }


}
