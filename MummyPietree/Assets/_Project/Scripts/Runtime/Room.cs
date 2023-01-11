using DG.Tweening;
using UnityEngine;

namespace MummyPietree
{
    public class Room : MonoBehaviour
    {
        [SerializeField] private Door[] connectedDoors;

        public void EnterRoom()
        {
            gameObject.SetActive(true);
            foreach (Door door in connectedDoors)
            {
                door.gameObject.SetActive(true);
            }
        }

        public void ExitRoom()
        {
            gameObject.SetActive(false);
            foreach (Door door in connectedDoors)
            {
                door.gameObject.SetActive(false);
            }
        }
    }
}
