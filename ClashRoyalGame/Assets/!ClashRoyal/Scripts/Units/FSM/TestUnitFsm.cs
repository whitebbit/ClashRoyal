using System;
using _3._Scripts.FSM.Base;
using _ClashRoyal.Scripts.FSM;
using _ClashRoyal.Scripts.Units.FSM.Movements;
using _ClashRoyal.Scripts.Units.FSM.States;
using UnityEngine;

namespace _ClashRoyal.Scripts.Units.FSM
{
    [Serializable]
    public class TestUnitFsm : UnitFsm
    {
        [SerializeField] private MovementState movementState;
        [SerializeField] private MovementState chaseState;
        [SerializeField] private AttackState attackState;

        protected override void SetFsmTransition()
        {
            movementState = movementState.CreateInstance(Unit);
            chaseState = chaseState.CreateInstance(Unit);
            attackState = attackState.CreateInstance(Unit);

            movementState.SetMovable(new NavMeshMovement(Unit));

            FsmHandler.AddTransition(movementState, new FuncPredicate(() => !movementState.OnPoint));
            FsmHandler.AddTransition(movementState, attackState, new FuncPredicate(() => movementState.OnPoint));
        }
    }
}