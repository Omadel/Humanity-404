using DG.Tweening;
using Etienne;
using Etienne.Animator2D;
using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace MummyPietree
{
    public class PlayerController : Singleton<PlayerController>
    {
        public Vector3 Direction => direction;
        public ItemData TransportedItem => transportedItem.ItemSO;
        public Room CurrentRoom => currentRoom;

        [SerializeField] private Room startingRoom;
        [SerializeField] private Volume volume;
        [Header("Change Room Animation")]
        [SerializeField] private float duration = .25f;
        [SerializeField] private float vignetteMaxValue = .8f;
        [Header("Stats")]
        [SerializeField] private float stressGainMoving = .1f;
        [SerializeField, Range(0f, 1f)] private float mood = .5f;
        [SerializeField] private Gradient moodColor;
        [SerializeField] private Slider moodBar;
        [SerializeField] private Slider activityBar;
        [SerializeField] private Item transportedItem;

        private Vector3 direction;
        private NavMeshAgent agent;
        private Interactable hoveredInteractible, selectedInteractible;
        private Room currentRoom;
        private Transform cameraRoot;
        private Animator2D animator2D;
        private CanvasGroup overHeadCanvas;
        private bool isInteracting = false;

        public bool HasItem => transportedItem.HasItem;

        protected override void Awake()
        {
            base.Awake();
            InputProvider.Instance.OnLeftMouseButtonPressed += BeginInteraction;
            InputProvider.Instance.OnLeftMouseButtonReleased += MoveTo;
            agent = GetComponent<NavMeshAgent>();
            agent.updateRotation = false;
            agent.updateUpAxis = false;
            agent.Warp(transform.position);
            animator2D = GetComponent<Animator2D>();
            overHeadCanvas = GetComponentInChildren<CanvasGroup>();
            overHeadCanvas.alpha = 0f;
        }

        private void Start()
        {
            foreach (Room room in FindObjectsOfType<Room>())
            {
                room.ExitRoom();
            }
            cameraRoot = Camera.main.transform.root;
            currentRoom = startingRoom;
            currentRoom.EnterRoom();
            HandleInteractionStress(0f);
        }

        public void EnterRoom(Room room)
        {
            if (room == currentRoom) return;
            currentRoom?.ExitRoom();
            currentRoom = room;
            currentRoom.EnterRoom();
            cameraRoot.DOMove(currentRoom.transform.position, duration);

            volume.profile.TryGet(out Vignette vignette);
            DOTween.To(() => vignette.intensity.value, x => vignette.intensity.value = x, vignetteMaxValue, duration * .5f).SetLoops(2, LoopType.Yoyo);
        }

        private void BeginInteraction(Vector2 mousePosition)
        {
            if (!IsPointerOverCollider(mousePosition, out RaycastHit hit)) return;
            if (!hit.collider.TryGetComponent(out Interactable interactible) || !interactible.IsInteractable) return;
            UnHoverInteractible();
            interactible.Click();
        }
        private bool TryInteract(Vector2 mousePosition)
        {
                UnHoverInteractible();
            if (!IsPointerOverCollider(mousePosition, out RaycastHit hit))
            {
                selectedInteractible = null;
                return false;
            }
            if (!hit.collider.TryGetComponent(out Interactable interactible) || !interactible.IsInteractable)
            {
                selectedInteractible = null;
                return false;
            }
            interactible.Release();
            selectedInteractible = interactible;
            return true;
        }

        private void HoverInteractible(Interactable interactible)
        {
            UnHoverInteractible();
            hoveredInteractible = interactible;
            hoveredInteractible.Hover();
        }

        private void UnHoverInteractible()
        {
            hoveredInteractible?.UnHover();
            hoveredInteractible = null;
        }

        private void Update()
        {
            if (isInteracting) return;
            ComputeDirection();

            if (!IsPointerOverCollider(InputProvider.Instance.MousePosition, out RaycastHit hit)) return;
            if (!hit.collider.TryGetComponent(out Interactable interactible) || !interactible.IsInteractable)
            {
                UnHoverInteractible();
                return;
            }
            HoverInteractible(interactible);
        }

        private void ComputeDirection()
        {
            direction = transform.position.Direction(agent.destination);
            if (Vector3.Distance(transform.position, agent.destination) <= .3f)
            {
                direction = Vector3.zero;
                agent.isStopped = true;
            }
            else
            {
                agent.isStopped = false;
            }
            if (agent.isStopped)
            {
                if (animator2D.GetState() != "Idle")
                {
                    animator2D.SetState("Idle", true);
                    selectedInteractible?.Interact();
                    selectedInteractible = null;
                }
            }
            else
            {
                HandleInteractionStress(stressGainMoving * Time.deltaTime);
                if (animator2D.GetState() != "Walk")
                {
                    animator2D.SetState("Walk", true);
                }
                Vector3 right = transform.GetChild(0).right;
                animator2D.FlipX(Vector3.Angle(right, direction) <= 90);
            }
            direction.Normalize();
        }

        private void MoveTo(Vector2 mousePosition)
        {
            Vector3 positionInWorld;
            if (!IsPointerOverCollider(mousePosition, out RaycastHit hit))
            {
                Plane plane = new Plane(Vector3.up, Vector3.zero);
                Ray ray = Camera.main.ScreenPointToRay(mousePosition);
                if (!plane.Raycast(ray, out float enter))
                {
                    Debug.LogError("No Plane WTF");
                    return;
                }
                positionInWorld = ray.GetPoint(enter);
            }
            else
            {
                positionInWorld = hit.point;
            }
            if (TryInteract(mousePosition))
            {
                Vector3 position = transform.position;
                position.y = positionInWorld.y;
                if (Vector3.Distance(position, positionInWorld) <= 1f)
                {

                    animator2D.SetState("Idle", true);
                    selectedInteractible?.Interact();
                    selectedInteractible = null;
                    return;
                }
            }

            NavMesh.SamplePosition(positionInWorld, out NavMeshHit navHit, 10000, NavMesh.AllAreas);

            agent.SetDestination(navHit.position);
        }

        private bool IsPointerOverCollider(Vector2 mousePosition, out RaycastHit hit)
        {
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            return Physics.Raycast(ray, out hit);
        }

        internal void HandleInteractionStress(float interactionStress, float interactionDuration = -1f, TweenCallback onComplete = null)
        {
            if (interactionDuration <= 0)
            {

                mood += interactionStress;
                mood = Mathf.Clamp01(mood);
                UpdateMoodBar();
                onComplete?.Invoke();
            }
            else
            {
                agent.isStopped = true;
                animator2D.SetState("Action");
                isInteracting = true;
                overHeadCanvas.DOFade(1f, .2f);
                DG.Tweening.Core.TweenerCore<float, float, DG.Tweening.Plugins.Options.FloatOptions> moodTween = DOTween.To(() => mood, x => mood = x, mood + interactionStress, interactionDuration)
                    .SetEase(Ease.Linear)
                    .OnUpdate(UpdateMoodBar);
                activityBar.value = 0f;
                activityBar.DOValue(1f, interactionDuration).SetEase(Ease.Linear)
                    .OnComplete(OnInteractionEnd);
                if (onComplete != null)
                {
                    moodTween.OnComplete(onComplete);
                }
            }
        }

        private void OnInteractionEnd()
        {
            overHeadCanvas.DOFade(0f, .2f).SetDelay(.1f);
            isInteracting = false;
            animator2D.SetState("Idle");
        }

        private void UpdateMoodBar()
        {
            if (moodBar != null)
            {
                moodBar.fillRect.GetComponent<Image>().color = moodColor.Evaluate(mood);
                moodBar.value = mood;
            }
        }

        public void TransportItem(ItemData item) => transportedItem.SetItem(item);

        public ItemData UseTransportedItem() => transportedItem.RemoveItem();
    }
}
