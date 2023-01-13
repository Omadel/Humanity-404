using DG.Tweening;
using Etienne;
using UnityEngine;

namespace MummyPietree
{
    public class Plant : Interactable
    {
        public bool IsHarverstable => isHarverstable;
        public bool HasSeed => hasSeed;
        public override bool IsInteractable => isHarverstable;

        [SerializeField] private PlantData plant;
        [SerializeField, MinMaxRange(0f, 5f)] private Range growthRange = new Range(0f, 1f);
        [SerializeField] private float growthDuration = 10f;
        [SerializeField] private bool hasSeed = false;
        [SerializeField, ReadOnly] private bool isHarverstable = false;

        new MeshRenderer renderer;
        MeshFilter filter;

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
            renderer = GetComponent<MeshRenderer>();
            filter = GetComponent<MeshFilter>();
        }

        public void SowPlant(SeedData seed)
        {
            plant = seed.GrownPlant;
            hasSeed = true;
            transform.DOScale(growthRange.Max, plant.ItemGrowthDuration).OnUpdate(UpdatePlantMesh).OnComplete(GrowthCompleted).SetEase(Ease.Linear);
        }

        private void UpdatePlantMesh()
        {
            var value = growthRange.Lerp(transform.localScale.x);
            int meshIndex =Mathf.CeilToInt(value * plant.GrowingStateMeshes.Length-1);
            Debug.Log(meshIndex);
            filter.mesh = plant.GrowingStateMeshes[meshIndex];
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
