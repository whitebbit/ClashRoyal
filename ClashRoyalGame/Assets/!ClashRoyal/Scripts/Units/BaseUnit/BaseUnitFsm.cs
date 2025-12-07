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

        private Map _map;
        private Unit _enemy;
        private UnitSphereOverlapDetector _detector;

        // Проверяет, находится ли враг в радиусе преследования
        private bool EnemyInChaseRadius
        {
            get
            {
                if (!_enemy || _enemy.Health.HealthPoints <= 0) return false;

                var chaseRadius = Unit.Parameters.GetConfig<UnitMovementConfig>().ChaseRadius;
                var attackerBodyRadius = Unit.Parameters.BodyRadius;
                var enemyBodyRadius = _enemy.Parameters.BodyRadius;
                var totalCollisionOffset = attackerBodyRadius + enemyBodyRadius;

                var sqrDistance = (_enemy.transform.position - Unit.transform.position).sqrMagnitude;
                var sqrChaseRange = (chaseRadius + totalCollisionOffset) * (chaseRadius + totalCollisionOffset);

                return sqrDistance <= sqrChaseRange;
            }
        }

        // Текущая цель для движения: враг в радиусе преследования или башня
        private Unit CurrentMovementTarget
        {
            get
            {
                // Если враг в радиусе преследования, идем к нему
                if (EnemyInChaseRadius) return _enemy;
                
                // Иначе идем к башне
                var tower = _map?.GetNearestEnemyTower(Unit);
                if (tower && tower.Health.HealthPoints > 0) return tower;
                
                return null;
            }
        }

        // Текущая цель для атаки: враг в радиусе преследования или башня в радиусе атаки
        private Unit CurrentAttackTarget
        {
            get
            {
                // Если враг в радиусе преследования, атакуем его
                if (EnemyInChaseRadius) return _enemy;
                
                // Иначе проверяем башню
                var tower = _map?.GetNearestEnemyTower(Unit);
                if (tower && tower.Health.HealthPoints > 0) return tower;
                
                return null;
            }
        }

        private bool HasTarget => CurrentAttackTarget != null;

        private bool TargetInAttackRadius
        {
            get
            {
                var target = CurrentAttackTarget;
                if (!target) return false;

                var attackRadius = Unit.Parameters.GetConfig<UnitAttackConfig>().AttackRadius;
                var attackerBodyRadius = Unit.Parameters.BodyRadius;
                var enemyBodyRadius = target.Parameters.BodyRadius;
                
                // Используем универсальный метод для расчета эффективной дистанции атаки
                var effectiveAttackDistance = UnitExtensions.GetEffectiveAttackDistance(
                    attackRadius, attackerBodyRadius, enemyBodyRadius);

                var sqrDistance = (target.transform.position - Unit.transform.position).sqrMagnitude;
                // Добавляем небольшой допуск (5%) чтобы предотвратить колебания между состояниями
                var sqrAttackRange = effectiveAttackDistance * effectiveAttackDistance * 1.05f;

                return sqrDistance <= sqrAttackRange;
            }
        }

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

            // Проверяем, находится ли текущий враг в радиусе преследования
            bool currentEnemyInChaseRadius = false;
            if (_enemy && _enemy.Health.HealthPoints > 0)
            {
                var chaseRadius = Unit.Parameters.GetConfig<UnitMovementConfig>().ChaseRadius;
                var attackerBodyRadius = Unit.Parameters.BodyRadius;
                var enemyBodyRadius = _enemy.Parameters.BodyRadius;
                var totalCollisionOffset = attackerBodyRadius + enemyBodyRadius;
                var sqrDistance = (_enemy.transform.position - Unit.transform.position).sqrMagnitude;
                var sqrChaseRange = (chaseRadius + totalCollisionOffset) * (chaseRadius + totalCollisionOffset);
                currentEnemyInChaseRadius = sqrDistance <= sqrChaseRange;
            }

            // Если текущий враг не в радиусе преследования, очищаем его
            if (_enemy && !currentEnemyInChaseRadius)
            {
                _enemy = null;
            }

            // Если уже есть враг-юнит в радиусе преследования, не меняем цель
            if (_enemy && _enemy.Health.HealthPoints > 0) return;

            // Устанавливаем нового врага
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
                var target = CurrentMovementTarget;
                return !target ? Unit.transform.position : target.GetTargetPosition(Unit);
            };
            
            // Функция для проверки, нужно ли остановиться (когда цель в радиусе атаки)
            movementState.ShouldStopMoving = () => TargetInAttackRadius;
        }

        private void InitializeAttackState()
        {
            attackState.TargetProvider = () => CurrentAttackTarget;
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