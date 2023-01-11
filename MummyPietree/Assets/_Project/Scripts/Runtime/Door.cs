using UnityEngine;

namespace MummyPietree
{
    public class Door : MonoBehaviour
    {
        [SerializeField] private Transform redRoom, blueRoom;
        private void OnTriggerExit(Collider other)
        {
            float angle = Vector3.Angle(transform.right, other.GetComponent<PlayerController>().Direction);
            Debug.Log(angle);
            bool goestoBlueRoom = angle <= 90f;
            Vector3 position = goestoBlueRoom ? blueRoom.position : redRoom.position;
            Camera.main.transform.parent.position = position;
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
