using System;
using _3._Scripts.FSM.Base;
using _ClashRoyal.Scripts.FSM.Base;
using _ClashRoyal.Scripts.Maps;
using _ClashRoyal.Scripts.Units.FSM.Interfaces;
using UnityEngine;

namespace _ClashRoyal.Scripts.Units.FSM.States
{
    [CreateAssetMenu(fileName = "_MovementState", menuName = "FSM/States/Movement", order = 0)]
    public class MovementState : State
    {
        [SerializeField] private float stoppingDistance;

        private IMovable _movable;
        private Vector3 _destination;

        public bool OnPoint => (Unit.transform.position - _destination).magnitude <= stoppingDistance;

        public void SetMovable(IMovable movable)
        {
            _movable = movable;
        }

        public override void OnEnter()
        {
            _destination = Map.Instance.GetNearestEnemyTower(Unit).transform.position;
            _movable.MoveTo(_destination);
        }

        public override void Update()
        {
            if (OnPoint) _movable.Stop();
        }

        public override void OnExit()
        {
        }
    }
}