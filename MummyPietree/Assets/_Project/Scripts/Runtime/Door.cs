using Etienne;
using System.Collections;
using UnityEngine;

namespace MummyPietree
{
    public class Door : Interactable
    {
        public IEnumerator Rooms()
        {
            yield return redRoom;
            yield return blueRoom;
        }
        
        [SerializeField] private float openedDuration = 3f;
        [SerializeField] private Room redRoom, blueRoom;



        private Animator animator;
        private Timer openedTimer;

        protected override void Start()
        {
            base.Start();
            animator = GetComponent<Animator>();
            openedTimer = Timer.Create(openedDuration, false).OnComplete(() => animator.CrossFade($"Door_Closed", .2f));
        }

        protected override void OnInteractionEnded()
        {
            bool goesToBlueRoom = blueRoom != PlayerController.Instance.CurrentRoom;
            if (goesToBlueRoom)
            {
                PlayerController.Instance.EnterRoom(blueRoom);
            }
            else
            {
                PlayerController.Instance.EnterRoom(redRoom);
            }
            animator.CrossFade($"Door_Opened{(goesToBlueRoom ? "Blue" : "Red")}", .2f);
            openedTimer.Restart();
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
