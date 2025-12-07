using System;
using _ClashRoyal.Scripts.FSM.Base;
using _ClashRoyal.Scripts.Units.Base.FSM.Interfaces;
using _ClashRoyal.Scripts.Units.Base.FSM.Movements;
using UnityEngine;

namespace _ClashRoyal.Scripts.Units.Base.FSM.States
{
    [CreateAssetMenu(fileName = "_MovementState", menuName = "FSM/States/Movement", order = 0)]
    public class MovementState : State
    {
        [Header("Movement Strategy")]
        [SerializeField] private MovementStrategy movementStrategy;
        
        public MovementStrategy MovementStrategy => movementStrategy;
        public IMovable Movable { private get; set; }
        public Func<Vector3> TargetProvider { private get; set; }
        
        private Vector3 _destination;

        public override void OnEnter()
        {
            base.OnEnter();
            
            UpdateDestination();
        }
        
        public override void Update()
        {
            UpdateDestination();
            Movable?.Update();
        }
        
        public override void OnExit()
        {
            base.OnExit();
            Movable.Stop();
        }
        
        private void UpdateDestination()
        {
            if (TargetProvider == null) return;

            var newDest = TargetProvider.Invoke();

            if (!((_destination - newDest).sqrMagnitude > 0.1f)) return;
            
            _destination = newDest;
            Movable.MoveTo(_destination);
        }
    }
}