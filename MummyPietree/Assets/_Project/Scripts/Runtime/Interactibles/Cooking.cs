using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MummyPietree
{
    public class Cooking : Interactable
    {
        public override bool IsInteractable => CanInteract();

        enum CookingState
        {
            None,
            Started,
            Ready,

        }

        [SerializeField] private MealData output;
        [SerializeField] private int cookingStep = -1;
        [SerializeField] private GameObject[] cookingSteps;

        [SerializeField] private List<ItemData> currentRecipee;

        CookingState _state;


        protected bool CanInteract()
        {
            if (_state == CookingState.Started)
            {
                if (IsItemInRecipe(PlayerController.Instance.TransportedItem) == false)
                    return false;
            }

            return true;
        }

        bool IsItemInRecipe(ItemData item)
        {
            return currentRecipee.Contains(item);
        }

        protected override void Start()
        {
            base.Start();
            ClearSteps();
        }

        protected override void OnInteractionEnded()
        {

            switch (_state)
            {
                case CookingState.None:
                    StartCooking();
                    break;
                case CookingState.Started:
                    UpdateCooking();
                    break;
                case CookingState.Ready:
                    StopCooking();
                    break;
            }
        }

        private void UpdateCooking()
        {

            ItemData item = PlayerController.Instance.UseTransportedItem();
            ItemData toRemove = currentRecipee.Where(i => i.name == item.name).FirstOrDefault();
            currentRecipee.Remove(toRemove);

            if (currentRecipee.Count == 0)
            {
                CookingComplete();
                return;
            }

            FillPan();
        }

        void ClearSteps()
        {
            for (int i = 0; i < cookingSteps.Length; i++)
            {
                cookingSteps[i].SetActive(false);
            }
        }

        private void SetCookingStep(int i)
        {
            ClearSteps();

            cookingSteps[i].SetActive(true);
            cookingStep = i;
        }
        void StopCooking()
        {
            _state = CookingState.None;
            PlayerController.Instance.TransportItem(output);
            cookingStep = -1;
            ClearSteps();
        }
        void StartCooking()
        {
            _state = CookingState.Started;
            currentRecipee = output.Recipee.ToList();
            SetCookingStep(0);
        }
        void FillPan()
        {
            SetCookingStep(1);
        }
        void CookingComplete()
        {
            _state = CookingState.Ready;

            SetCookingStep(2);
        }
    }
}
