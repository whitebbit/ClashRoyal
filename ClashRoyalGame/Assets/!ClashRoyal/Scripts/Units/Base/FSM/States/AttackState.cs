using System;
using _ClashRoyal.Scripts.FSM.Base;
using _ClashRoyal.Scripts.Units.Base.Scriptables.Configs;
using UnityEngine;

namespace _ClashRoyal.Scripts.Units.Base.FSM.States
{
    [CreateAssetMenu(fileName = "_AttackState", menuName = "FSM/States/Attack", order = 0)]
    public class AttackState : State
    {
        [Header("Attack Strategy")]
        [SerializeField] private AttackStrategy attackStrategy;
        
        public AttackStrategy AttackStrategy => attackStrategy;
        public Func<Unit> TargetProvider { private get; set; }

        private float _attackTimer;

        public override void OnEnter()
        {
            _attackTimer = 0f;
        }

        public override void Update()
        {
            var target = TargetProvider?.Invoke();

            if (!target) return;

            Unit.transform.LookAt(target.transform.position);

            _attackTimer += Time.deltaTime;

            if (!(_attackTimer >= Unit.Parameters.GetConfig<UnitAttackConfig>().AttackRate)) return;

            _attackTimer = 0f;
            
            PerformAttack(target);
        }

        private void PerformAttack(Unit target)
        {
            var attackConfig = Unit.Parameters.GetConfig<UnitAttackConfig>();
            
            if (attackStrategy == null)
            {
                target.ApplyDamage(attackConfig.Damage);
                return;
            }
            
            attackStrategy.Execute(Unit, target, attackConfig.Damage);
        }
    }
}