using UnityEngine;
using DG.Tweening;

namespace MummyPietree
{
    public class Door : MonoBehaviour
    {
        [SerializeField] private Room redRoom, blueRoom;
        private Animator animator;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        private bool GoesToBlueRoom()
        {
            float angle = Vector3.Angle(transform.right, PlayerController.Instance.Direction);
            return angle <= 90f;
        }

        private void OnTriggerEnter(Collider other)
        {
            bool goesToBlueRoom = GoesToBlueRoom();
            animator.CrossFade($"Door_Opened{(goesToBlueRoom ? "Blue" : "Red")}", .2f);
        }

        private void OnTriggerExit(Collider other)
        {
            PlayerController.Instance.EnterRoom(GoesToBlueRoom() ? blueRoom : redRoom);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Color color = UnityEditor.Handles.color;
            UnityEngine.Rendering.CompareFunction zTest = UnityEditor.Handles.zTest;
            Color red = Color.red;
            Color blue = Color.blue;
            UnityEditor.Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
            UnityEditor.Handles.color = red;
            UnityEditor.Handles.ArrowHandleCap(0, transform.position, Quaternion.LookRotation(-transform.right, transform.up), 1f, EventType.Repaint);
            UnityEditor.Handles.color = blue;
            UnityEditor.Handles.ArrowHandleCap(0, transform.position, Quaternion.LookRotation(transform.right, transform.up), 1f, EventType.Repaint);
            UnityEditor.Handles.zTest = UnityEngine.Rendering.CompareFunction.GreaterEqual;
            blue.a = .3f;
            red.a = .3f;
            UnityEditor.Handles.color = red;
            UnityEditor.Handles.ArrowHandleCap(0, transform.position, Quaternion.LookRotation(-transform.right, transform.up), 1f, EventType.Repaint);
            UnityEditor.Handles.color = blue;
            UnityEditor.Handles.ArrowHandleCap(0, transform.position, Quaternion.LookRotation(transform.right, transform.up), 1f, EventType.Repaint);
            UnityEditor.Handles.color = color;
            UnityEditor.Handles.zTest = zTest;
        }
#endif
    }
}
