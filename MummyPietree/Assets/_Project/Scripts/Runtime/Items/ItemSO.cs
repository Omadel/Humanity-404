using Etienne;
using UnityEngine;

namespace MummyPietree
{
    public abstract class ItemSO : ScriptableObject
    {
        public Sprite ItemSprite => itemSprite;

        [SerializeField, PreviewSprite] private Sprite itemSprite;
    }


}
