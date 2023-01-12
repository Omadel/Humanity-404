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
        [SerializeField] private SpriteRenderer transportedItem;

        private Vector3 direction, position;
        private NavMeshAgent agent;
        private Interactible hoveredInteractible, selectedInteractible;
        private Room currentRoom;
        private Transform cameraRoot;
        private Animator animator;
        private Animator2D animator2D;
        private CanvasGroup overHeadCanvas;

        public bool TransportsItem => transportedItem.sprite != null;

        protected override void Awake()
        {
            base.Awake();
            InputProvider.Instance.OnLeftMouseButtonPressed += BeginInteraction;
            InputProvider.Instance.OnLeftMouseButtonReleased += MoveTo;
            agent = GetComponent<NavMeshAgent>();
            agent.updateRotation = false;
            agent.updateUpAxis = false;
            animator = GetComponent<Animator>();
            animator2D = GetComponent<Animator2D>();
            overHeadCanvas = GetComponentInChildren<CanvasGroup>();
            overHeadCanvas.alpha = 0f;
            transportedItem.color = Color.clear;
            transportedItem.sprite = null;
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
            if (!hit.collider.TryGetComponent(out Interactible interactible)) return;
            UnHoverInteractible();
            interactible.Click();
        }
        private bool TryInteract(Vector2 mousePosition)
        {
            if (!IsPointerOverCollider(mousePosition, out RaycastHit hit)) return false;
            if (!hit.collider.TryGetComponent(out Interactible interactible))
            {
                return false;
            }
            UnHoverInteractible();
            interactible.Release();
            selectedInteractible = interactible;
            Debug.Log("Select interactible");
            return true;
        }

        private void HoverInteractible(Interactible interactible)
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
            ComputeDirection();

            if (!IsPointerOverCollider(InputProvider.Instance.MousePosition, out RaycastHit hit)) return;
            if (!hit.collider.TryGetComponent(out Interactible interactible) || !interactible.IsInteractable)
            {
                UnHoverInteractible();
                return;
            }
            HoverInteractible(interactible);
        }

        private void ComputeDirection()
        {
            Vector3 oldPosition = position;
            position = transform.position;
            direction = oldPosition.Direction(position);
            if (Vector3.Distance(position, agent.destination) <= .2f)
            {
                direction = Vector3.zero;
                agent.isStopped = true;
            }
            else
            {
                agent.isStopped = false;
            }
            AnimatorClipInfo[] infos = animator.GetCurrentAnimatorClipInfo(0);
            if (agent.isStopped)
            {
                if (animator2D.GetState() != "Idle")
                {
                    animator2D.SetState("Idle", true);
                    animator.Play("Player_Idle");
                    selectedInteractible?.Interact();
                    selectedInteractible = null;
                    Debug.Log("Deselect Interactible");
                }
            }
            else
            {
                HandleInteractionStress(stressGainMoving * Time.deltaTime);
                if (animator2D.GetState() != "Walk")
                {
                    animator2D.SetState("Walk", true);
                    animator.Play("Player_Walk");
                }
                Vector3 right = transform.GetChild(0).right;
                animator2D.Renderer.flipX = Vector3.Angle(right, direction) <= 90;
            }
            direction.Normalize();
        }

        private void MoveTo(Vector2 mousePosition)
        {
            if (TryInteract(mousePosition))
            {
                //return;
            }
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
            }
            else
            {
                overHeadCanvas.DOFade(1f, .2f);
                DG.Tweening.Core.TweenerCore<float, float, DG.Tweening.Plugins.Options.FloatOptions> moodTween = DOTween.To(() => mood, x => mood = x, mood + interactionStress, interactionDuration)
                    .SetEase(Ease.Linear)
                    .OnUpdate(UpdateMoodBar);
                activityBar.value = 0f;
                activityBar.DOValue(1f, interactionDuration).SetEase(Ease.Linear)
                    .OnComplete(() => overHeadCanvas.DOFade(0f, .2f).SetDelay(.1f));
                if (onComplete != null)
                {
                    moodTween.OnComplete(onComplete);
                }
            }
        }

        private void UpdateMoodBar()
        {
            if (moodBar != null)
            {
                moodBar.fillRect.GetComponent<Image>().color = moodColor.Evaluate(mood);
                moodBar.value = mood;
            }
        }

        public void TransportItem(Sprite sprite)
        {
            transportedItem.sprite = sprite;
            transportedItem.color = Color.white;
        }

        public Sprite UseTransportedItem()
        {
            transportedItem.color = Color.clear;
            Sprite sprite = transportedItem.sprite;
            transportedItem.sprite = null;
            return sprite;
        }
    }
}
