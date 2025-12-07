using System.Collections;
using _ClashRoyal.Scripts.Units.Base;
using UnityEngine;

namespace _ClashRoyal.Scripts.Units.Base.FSM.States.Strategies
{
    [CreateAssetMenu(fileName = "RangedAttackStrategy", menuName = "FSM/States/Attack/Strategies/Ranged", order = 2)]
    public class RangedAttackStrategy : AttackStrategy
    {
        [Header("Projectile Settings")] [SerializeField]
        private Projectile projectilePrefab;

        [SerializeField] private Vector3 offset;

        [SerializeField] private float projectileSpeed = 10f;

        [Header("Animation Settings")]
        [Tooltip(
            "Нормализованное время анимации (0-1), когда должен спавниться проектиль. 0 = начало анимации, 1 = конец анимации")]
        [SerializeField]
        [Range(0f, 1f)]
        private float projectileSpawnNormalizedTime = 0.3f;

        [Tooltip("Альтернатива: фиксированная задержка в секундах. Если > 0, используется вместо normalizedTime")]
        [SerializeField]
        private float projectileSpawnDelay = 0f;

        public override void Execute(Unit attacker, Unit target, float damage)
        {
            if (!target || !projectilePrefab) return;

            // Запускаем анимацию атаки
            attacker.Animator?.PlayAttackAnimation();

            // Запускаем корутину для ожидания нужного момента анимации
            attacker.StartCoroutine(SpawnProjectileWhenReady(attacker, target, damage));
        }

        private IEnumerator SpawnProjectileWhenReady(Unit attacker, Unit target, float damage)
        {
            // Если указана фиксированная задержка, используем её
            if (projectileSpawnDelay > 0f)
            {
                yield return new WaitForSeconds(projectileSpawnDelay);
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
                    // Если нет аниматора, спавним сразу
                    SpawnProjectile(attacker, target, damage);
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
                        if (stateInfo.normalizedTime >= projectileSpawnNormalizedTime)
                        {
                            // Спавним проектиль
                            SpawnProjectile(attacker, target, damage);
                            yield break;
                        }

                        // Если анимация уже прошла нужный момент (нормализованное время > 1), спавним сразу
                        if (stateInfo.normalizedTime >= 1f)
                        {
                            SpawnProjectile(attacker, target, damage);
                            yield break;
                        }
                    }

                    elapsedTime += Time.deltaTime;
                    yield return null;
                }

                // Если не удалось найти состояние атаки за отведенное время, спавним проектиль
                SpawnProjectile(attacker, target, damage);
            }
        }

        private void SpawnProjectile(Unit attacker, Unit target, float damage)
        {
            if (!target || !projectilePrefab) return;

            var spawnPosition = GetProjectileSpawnPosition(attacker);
            var projectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);

            projectile.Initialize(attacker, target, damage, projectileSpeed);
        }

        private Vector3 GetProjectileSpawnPosition(Unit attacker)
        {
            // TransformPoint преобразует локальные координаты offset в мировые координаты
            // относительно позиции и поворота юнита
            return attacker.transform.TransformPoint(offset);
        }
    }
}