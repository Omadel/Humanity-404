using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MummyPietree
{
    public class Fridge : Interactable
    {
        Animator animator;

        protected override void Start()
        {
            base.Start();
            animator = GetComponent<Animator>();
        }

        public override void Interact()
        {
            Debug.Log("Interact");
            base.Interact();
            animator.CrossFade("FridgeOpen", .25f);
        }

        protected override void OnInteractionEnded()
        {
            animator.CrossFade("FridgeClose", .25f);
            PlayerController.Instance.DisableAI();
        }
    }
}
