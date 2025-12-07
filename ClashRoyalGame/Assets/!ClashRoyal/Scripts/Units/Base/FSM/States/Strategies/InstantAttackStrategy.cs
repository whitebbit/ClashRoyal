using UnityEngine;

namespace _ClashRoyal.Scripts.Units.Base.FSM.States.Strategies
{
    [CreateAssetMenu(fileName = "_InstantAttackStrategy", menuName = "FSM/States/Attack/Strategies/Instant", order = 0)]
    public class InstantAttackStrategy : AttackStrategy
    {
        public override void Execute(Unit attacker, Unit target, float damage)
        {
            if (!target) return;
            target.ApplyDamage(damage);
        }
    }
}

