using Etienne;
using UnityEngine;
using UnityEngine.AI;

namespace MummyPietree
{
    public class PlayerController : MonoBehaviour
    {
        public Vector3 Direction => direction;

        private Vector3 direction, position;
        private NavMeshAgent agent;
        private Interactible hoveredInteractible;

        private void Awake()
        {
            InputProvider.Instance.OnLeftMouseButtonPressed += BeginInteraction;
            InputProvider.Instance.OnLeftMouseButtonReleased += Interact;
            InputProvider.Instance.OnRightMouseButtonPressed += MoveTo;
            agent = GetComponent<NavMeshAgent>();
            agent.updateRotation = false;
            agent.updateUpAxis = false;
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
            //agent.SetDestination(positionInWorld);
        }

        private bool IsPointerOverCollider(Vector2 mousePosition, out RaycastHit hit)
        {
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            return Physics.Raycast(ray, out hit);
        }
    }
}
