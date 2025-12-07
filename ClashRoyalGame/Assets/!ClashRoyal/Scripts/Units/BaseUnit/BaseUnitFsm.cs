using System;
using _3._Scripts.FSM.Base;
using _ClashRoyal.Scripts.Detectors.OverlapSystem;
using _ClashRoyal.Scripts.FSM;
using _ClashRoyal.Scripts.Maps;
using _ClashRoyal.Scripts.Units;
using _ClashRoyal.Scripts.Units.Base;
using _ClashRoyal.Scripts.Units.Base.FSM;
using _ClashRoyal.Scripts.Units.Base.FSM.States;
using _ClashRoyal.Scripts.Units.Base.Scriptables.Configs;
using UnityEditor;
using UnityEngine;

namespace _ClashRoyal.Scripts.Units.BaseUnit
{
    [Serializable]
    public class BaseUnitFsm : UnitFsm
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
                var attackerBodyRadius = Unit.Parameters.BodyRadius;
                var enemyBodyRadius = _enemy.Parameters.BodyRadius;
                
                // Используем универсальный метод для расчета эффективной дистанции атаки
                var effectiveAttackDistance = UnitExtensions.GetEffectiveAttackDistance(
                    attackRadius, attackerBodyRadius, enemyBodyRadius);

                var sqrDistance = (_enemy.transform.position - Unit.transform.position).sqrMagnitude;
                // Добавляем небольшой допуск (5%) чтобы предотвратить колебания между состояниями
                var sqrAttackRange = effectiveAttackDistance * effectiveAttackDistance * 1.05f;

                return sqrDistance <= sqrAttackRange;
            }
        }

        private bool TargetInChaseRadius
        {
            get
            {
                if (!HasTarget) return false;

                var chaseRadius = Unit.Parameters.GetConfig<UnitMovementConfig>().ChaseRadius;
                var attackerBodyRadius = Unit.Parameters.BodyRadius;
                var enemyBodyRadius = _enemy.Parameters.BodyRadius;
                var totalCollisionOffset = attackerBodyRadius + enemyBodyRadius;

                var sqrDistance = (_enemy.transform.position - Unit.transform.position).sqrMagnitude;
                var sqrChaseRange = (chaseRadius + totalCollisionOffset) * (chaseRadius + totalCollisionOffset);

                return sqrDistance <= sqrChaseRange;
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

            if (!TargetFilter.CanAttack(Unit, unit)) return;

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
            var movementConfig = Unit.Parameters.GetConfig<UnitMovementConfig>();
            var movementStrategy = movementState.MovementStrategy;
            
            if (movementStrategy == null)
            {
                Debug.LogError($"MovementStrategy is not set in MovementState for {Unit.name}");
                return;
            }
            
            var movable = movementStrategy.CreateMovable(Unit);
            movable.Speed = movementConfig.Speed;
            movementState.Movable = movable;

            movementState.TargetProvider = () =>
            {
                var target = TargetInChaseRadius ? _enemy : _map.GetNearestEnemyTower(Unit);
                return !target ? Unit.transform.position : target.GetTargetPosition(Unit);
            };
            
            // Функция для проверки, нужно ли остановиться (когда цель в радиусе атаки)
            movementState.ShouldStopMoving = () => TargetInAttackRadius;
        }

        private void InitializeAttackState()
        {
            attackState.TargetProvider = () => HasTarget ? _enemy : null;
        }

#if UNITY_EDITOR

        public override void OnDrawGizmos(Unit unit)
        {
            var attackConfig = unit.Parameters.GetConfig<UnitAttackConfig>();
            var movementConfig = unit.Parameters.GetConfig<UnitMovementConfig>();

            if (!attackConfig) return;

            Handles.color = Color.red;
            Handles.DrawWireDisc(unit.transform.position, Vector3.up, attackConfig.AttackRadius);

            Handles.color = Color.yellow;
            Handles.DrawWireDisc(unit.transform.position, Vector3.up, movementConfig.ChaseRadius);

            Handles.color = Color.green;
            Handles.DrawWireDisc(unit.transform.position, Vector3.up, attackConfig.DetectRadius);
        }
#endif
    }
}