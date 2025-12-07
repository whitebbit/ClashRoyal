using System.Collections;
using _ClashRoyal.Scripts.Units.Base;
using UnityEngine;

namespace _ClashRoyal.Scripts.Units.Base.FSM.States.Strategies
{
    [CreateAssetMenu(fileName = "_MeleeAttackStrategy", menuName = "FSM/States/Attack/Strategies/Melee", order = 1)]
    public class MeleeAttackStrategy : AttackStrategy
    {
      
        [Tooltip(
            "Нормализованное время анимации (0-1), когда должен наноситься урон. 0 = начало анимации, 1 = конец анимации")]
        [SerializeField]
        [Range(0f, 1f)]
        private float damageHitNormalizedTime = 0.5f;

        [Tooltip("Альтернатива: фиксированная задержка в секундах. Если > 0, используется вместо normalizedTime")]
        [SerializeField]
        private float damageHitDelay = 0f;
        
        public override void Execute(Unit attacker, Unit target, float damage)
        {
            if (target == null) return;
            
            attacker.Animator?.PlayAttackAnimation();
            
            attacker.StartCoroutine(ApplyDamageWhenReady(attacker, target, damage));
        }

        private IEnumerator ApplyDamageWhenReady(Unit attacker, Unit target, float damage)
        {
            // Если указана фиксированная задержка, используем её
            if (damageHitDelay > 0f)
            {
                yield return new WaitForSeconds(damageHitDelay);
            }
            else
            {
                // Ждем, пока анимация перейдет в состояние атаки
                yield return null;
                yield return null;

                // Получаем Animator
                var animator = attacker.Animator?.UnityAnimator;
                if (animator == null)
                {
                    // Если нет аниматора, наносим урон сразу
                    ApplyDamage(target, damage);
                    yield break;
                }

                // Ждем, пока анимация достигнет нужного момента
                float elapsedTime = 0f;
                float maxWaitTime = 5f; // Максимальное время ожидания (защита от бесконечного цикла)

                while (elapsedTime < maxWaitTime)
                {
                    // Проверяем все слои аниматора
                    bool foundAttackState = false;
                    AnimatorStateInfo stateInfo = default;

                    for (int layer = 0; layer < animator.layerCount; layer++)
                    {
                        stateInfo = animator.GetCurrentAnimatorStateInfo(layer);

                        // Проверяем по тегу "Attack" или по имени, содержащему "Attack"
                        if (stateInfo.IsTag("Attack") ||
                            stateInfo.IsName("Attack"))
                        {
                            foundAttackState = true;
                            break;
                        }
                    }

                    if (foundAttackState)
                    {
                        // Проверяем, достигли ли мы нужного момента анимации
                        if (stateInfo.normalizedTime >= damageHitNormalizedTime)
                        {
                            // Наносим урон
                            ApplyDamage(target, damage);
                            yield break;
                        }

                        // Если анимация уже прошла нужный момент (нормализованное время > 1), наносим урон сразу
                        if (stateInfo.normalizedTime >= 1f)
                        {
                            ApplyDamage(target, damage);
                            yield break;
                        }
                    }

                    elapsedTime += Time.deltaTime;
                    yield return null;
                }

                // Если не удалось найти состояние атаки за отведенное время, наносим урон
                ApplyDamage(target, damage);
            }
        }

        private void ApplyDamage(Unit target, float damage)
        {
            if (target != null)
            {
                target.ApplyDamage(damage);
            }
        }
    }
}

