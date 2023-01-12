using DG.Tweening;
using Etienne;
using UnityEngine;

namespace MummyPietree
{
    public class Plant : Interactable
    {
        public bool IsHarverstable => isHarverstable;
        public bool HasSeed => hasSeed;

        [SerializeField] private PlantSO plant;
        [SerializeField, MinMaxRange(0f, 5f)] private Range growthRange = new Range(0f, 1f);
        [SerializeField] private float growthDuration = 10f;
        [SerializeField] private bool hasSeed = false;
        [SerializeField, ReadOnly] private bool isHarverstable = false;

        public override bool IsInteractable => isHarverstable;

        protected override void Start()
        {
            base.Start();
            if (hasSeed)
            {
                isHarverstable = true;
            }
            else
            {
                transform.localScale = Vector3.zero;
            }
        }

        public void SowPlant(SeedSO seed)
        {
            plant = seed.GrownPlant;
            hasSeed = true;
            transform.DOScale(growthRange.Max, growthDuration).OnComplete(GrowthCompleted).SetEase(Ease.Linear);
        }

        private void GrowthCompleted()
        {
            transform.DOPunchScale(Vector3.one * .1f, .2f);
            isHarverstable = true;
        }

        protected override void OnInteractionEnded()
        {
            Harverst();
        }

        private void Harverst()
        {
            isHarverstable = false;
            hasSeed = false;
            transform.localScale = Vector3.one * growthRange.Min;
            PlayerController.Instance.TransportItem(plant);
        }
    }
}
