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
using UnityEditor;
using UnityEngine;

namespace _ClashRoyal.Scripts.Units.Test
{
    [Serializable]
    public class TestUnitFsm : UnitFsm
    {
        [SerializeField] private MovementState movementState;
        [SerializeField] private AttackState attackState;

        private bool HasTarget => _enemy && _enemy.Health.HealthPoints > 0;

        private bool TargetInAttackRadius
        {
            get
            {
                if (!HasTarget) return false;

                var attackRadius = Unit.Parameters.GetConfig<UnitAttackConfig>().AttackRadius;
                var collisionOffset = _enemy.Parameters.BodyRadius;

                var sqrDistance = (_enemy.transform.position - Unit.transform.position).sqrMagnitude;
                var sqrAttackRange = (attackRadius + collisionOffset) * (attackRadius + collisionOffset);

                return sqrDistance <= sqrAttackRange;
            }
        }

        private Map _map;
        private Unit _enemy;
        private UnitSphereOverlapDetector _detector;

        public override void Initialize(Unit unit)
        {
            _map = Map.Instance;

            base.Initialize(unit);

            InitializeDetector(unit);
        }

        protected override void SetFsmTransition()
        {
            movementState = movementState.CreateInstance(Unit);
            attackState = attackState.CreateInstance(Unit);

            InitializeMovementState();
            InitializeAttackState();

            FsmHandler.AddTransition(movementState, new FuncPredicate(() => !TargetInAttackRadius));
            FsmHandler.AddTransition(movementState, attackState, new FuncPredicate(() => TargetInAttackRadius));
        }

        private void OnFound(Unit unit)
        {
            if (!unit) return;

            if (unit.TeamType == Unit.TeamType) return;

            if (_enemy == unit) return;

            if (HasTarget) return;

            _enemy = unit;
        }

        private void InitializeDetector(Unit unit)
        {
            _detector = unit.GetComponent<UnitSphereOverlapDetector>();
            _detector.SetRadius(Unit.Parameters.GetConfig<UnitAttackConfig>().DetectRadius);
            _detector.OnFound += OnFound;
        }

        private void InitializeMovementState()
        {
            movementState.Movable = new NavMeshMovement(Unit)
            {
                Speed = Unit.Parameters.GetConfig<UnitMovementConfig>().Speed
            };

            movementState.TargetProvider = () =>
            {
                var target = HasTarget ? _enemy : _map.GetNearestEnemyTower(Unit);
                var direction = (target.transform.position - Unit.transform.position).normalized;
                var enemyRadius = target.Parameters.BodyRadius;
                var targetPos = target.transform.position - direction * enemyRadius;
                return targetPos;
            };
        }

        private void InitializeAttackState()
        {
            attackState.TargetProvider = () => HasTarget ? _enemy : null;
        }

#if UNITY_EDITOR

        public override void OnDrawGizmos()
        {
            var attackConfig = Unit.Parameters.GetConfig<UnitAttackConfig>();
            if (!attackConfig) return;
            
            Handles.color = Color.red;
            Handles.DrawWireDisc(Unit.transform.position, Vector3.up, attackConfig.AttackRadius);
            
            Handles.color = Color.yellow;
            Handles.DrawWireDisc(Unit.transform.position, Vector3.up, attackConfig.DetectRadius);
        }
#endif
    }
}