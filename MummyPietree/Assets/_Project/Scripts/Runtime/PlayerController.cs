using DG.Tweening;
using Etienne;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

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
        [SerializeField, Range(0f, 1f)] float mood = .5f;
        [SerializeField] Gradient moodColor;
        [SerializeField] MeshRenderer moodRenderer;

        private Vector3 direction, position;
        private NavMeshAgent agent;
        private Interactible hoveredInteractible;
        private Room currentRoom;
        private Transform cameraRoot;

        protected override void Awake()
        {
            base.Awake();
            InputProvider.Instance.OnLeftMouseButtonPressed += BeginInteraction;
            InputProvider.Instance.OnLeftMouseButtonReleased += Interact;
            InputProvider.Instance.OnRightMouseButtonPressed += MoveTo;
            agent = GetComponent<NavMeshAgent>();
            agent.updateRotation = false;
            agent.updateUpAxis = false;
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
        private void Interact(Vector2 mousePosition)
        {
            if (!IsPointerOverCollider(mousePosition, out RaycastHit hit)) return;
            if (!hit.collider.TryGetComponent(out Interactible interactible)) return;
            UnHoverInteractible();
            interactible.Release();
            interactible.Interact();
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
            moodRenderer.material.color = moodColor.Evaluate(mood);
            ComputeDirection();

            if (!IsPointerOverCollider(InputProvider.Instance.MousePosition, out RaycastHit hit)) return;
            if (!hit.collider.TryGetComponent(out Interactible interactible))
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
            direction = oldPosition.Direction(position).normalized;
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

            NavMesh.SamplePosition(positionInWorld, out NavMeshHit navHit, 10000, NavMesh.AllAreas);
            agent.SetDestination(navHit.position);
        }

        private bool IsPointerOverCollider(Vector2 mousePosition, out RaycastHit hit)
        {
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            return Physics.Raycast(ray, out hit);
        }
    }
}
