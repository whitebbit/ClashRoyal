using System;
using _3._Scripts.FSM.Base;
using _ClashRoyal.Scripts.Detectors.OverlapSystem;
using _ClashRoyal.Scripts.FSM;
using _ClashRoyal.Scripts.Maps;
using _ClashRoyal.Scripts.Units.Base;
using _ClashRoyal.Scripts.Units.Base.FSM;
using _ClashRoyal.Scripts.Units.Base.FSM.Movements;
using _ClashRoyal.Scripts.Units.Base.FSM.States;
using _ClashRoyal.Scripts.Units.Base.Scriptables.Configs;
using UnityEngine;

namespace _ClashRoyal.Scripts.Units.Test
{
    [Serializable]
    public class TestUnitFsm : UnitFsm
    {
        [SerializeField] private MovementState movementState;
        [SerializeField] private AttackState attackState;

        private bool HasTarget => _enemy && _enemy.Health.HealthPoints > 0;

        private bool TargetInAttackRadius => HasTarget &&
                                             (_enemy.transform.position - Unit.transform.position).magnitude <=
                                             Unit.Parameters.GetConfig<UnitAttackConfig>().AttackRadius;

        private Unit _enemy;
        private UnitSphereOverlapDetector _detector;

        public override void Initialize(Unit unit)
        {
            base.Initialize(unit);

            _detector = unit.GetComponent<UnitSphereOverlapDetector>();
            _detector.SetRadius(Unit.Parameters.GetConfig<UnitAttackConfig>().AttackRadius);
            _detector.OnFound += OnFound;
        }

        protected override void SetFsmTransition()
        {
            movementState = movementState.CreateInstance(Unit);
            attackState = attackState.CreateInstance(Unit);

            InitializeMovementState();

            FsmHandler.AddTransition(movementState, new FuncPredicate(() => !TargetInAttackRadius));
            FsmHandler.AddTransition(movementState, attackState, new FuncPredicate(() => TargetInAttackRadius));
        }

        private void OnFound(Unit unit)
        {
            Debug.Log($"{Unit.name}: Unit {unit?.name} found");
            if (!unit) return;

            if (unit.TeamType == Unit.TeamType) return;

            if (_enemy == unit) return;

            if (HasTarget) return;

            _enemy = unit;
        }

        private void InitializeMovementState()
        {
            movementState.Movable = new NavMeshMovement(Unit)
            {
                Speed = Unit.Parameters.GetConfig<UnitMovementConfig>().Speed
            };

            movementState.TargetProvider = () =>
                HasTarget ? _enemy.transform.position : Map.Instance.GetNearestEnemyTower(Unit).transform.position;
        }
    }
}