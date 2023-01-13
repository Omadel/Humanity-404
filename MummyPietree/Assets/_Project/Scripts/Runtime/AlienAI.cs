using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Threading.Tasks;
using Random = UnityEngine.Random;

namespace MummyPietree
{
    class AlienAI : MonoBehaviour
    {
        public bool IsActivated => _isActivated;

        enum AlienBrainState
        {
            Waiting,
            Walking
        }

        //public List<Interactable> Interactables;
        public float WaitMin = 1;
        public float WaitMax = 3;
        public float TimerAfterDoor = 1f;

        bool _wasDoor;
        Interactable _currentActivity;
        PlayerController _player;
        AlienBrainState _state;
        float _timer;
        bool _isActivated;

        private void Awake()
        {
            _player = GetComponent<PlayerController>();
        }

        private void Start()
        {
            SetWaitState();
        }

        public void ActivateAI()
        {
            _isActivated = true;
        }

        public void DisableAI()
        {
            _isActivated = false;
        }


        void SetWaitState()
        {
            if (_wasDoor)
            {
                _timer = TimerAfterDoor;
            }
            else
            {
                _timer = Random.Range(WaitMin, WaitMax);
            }
            _state = AlienBrainState.Waiting;
        }

        private void Update()
        {
            if (_isActivated == false)
                return;

            if (_state == AlienBrainState.Waiting)
            {
                _timer -= Time.deltaTime;

                if (_timer <= 0)
                {
                    FindRandomActivity();
                }
            }
            else if (_state == AlienBrainState.Walking)
            {
                Vector3 position = transform.position;
                position.y = _currentActivity.transform.position.y;

                if (Vector3.Distance(position, _currentActivity.transform.position) <= 1f)
                {
                    _player.Interact(_currentActivity);
                    _currentActivity = null;

                    SetWaitState();
                    return;
                }
            }
        }

        private void FindRandomActivity()
        {
            int rnd = Random.Range(0, 100);
            Interactable interactable = null;

            if (rnd < 80)
            {
                interactable = GetRandomDoor();
                _wasDoor = true;
            }
            else
            {
                if (_player.CurrentRoom.Activities.Length > 0)
                {
                    rnd = Random.Range(0, _player.CurrentRoom.Activities.Length);
                    interactable = _player.CurrentRoom.Activities[rnd];
                }
                else
                {
                    interactable = GetRandomDoor();
                    _wasDoor = true;
                }
            }
            Debug.Log("Move To : " + interactable.name);
            _player.MoveToPosition(interactable.transform.position);
            _currentActivity = interactable;

            _state = AlienBrainState.Walking;
        }

        Door GetRandomDoor()
        {
            int rnd = Random.Range(0, _player.CurrentRoom.ConnectedDoors.Length);
            Door door = _player.CurrentRoom.ConnectedDoors[rnd];
            return door;
        }

    }

}