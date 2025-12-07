using _ClashRoyal.Scripts.Units.Base;
using UnityEngine;

namespace _ClashRoyal.Scripts.Units.Base.FSM.States.Strategies
{
    [CreateAssetMenu(fileName = "_MeleeAttackStrategy", menuName = "FSM/States/Attack/Strategies/Melee", order = 1)]
    public class MeleeAttackStrategy : AttackStrategy
    {
        [Header("Animation Settings")]
        [SerializeField] private string attackAnimationTrigger = "Attack";
        [SerializeField] private float attackHitDelay = 0.5f;
        
        public override void Execute(Unit attacker, Unit target, float damage)
        {
            if (target == null) return;
            
            var animator = attacker.GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetTrigger(attackAnimationTrigger);
            }
            
            if (attackHitDelay > 0)
            {
                attacker.StartCoroutine(DelayedDamageCoroutine(target, damage, attackHitDelay));
            }
            else
            {
                target.ApplyDamage(damage);
            }
        }

        private System.Collections.IEnumerator DelayedDamageCoroutine(Unit target, float damage, float delay)
        {
            yield return new WaitForSeconds(delay);
            if (target != null)
            {
                target.ApplyDamage(damage);
            }
        }
    }
}

