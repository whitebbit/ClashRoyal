using _ClashRoyal.Scripts.Units.Base;
using _ClashRoyal.Scripts.Units.Base.Scriptables.Configs;
using UnityEngine;

namespace _ClashRoyal.Scripts.Units
{
    public static class UnitExtensions
    {
        /// <summary>
        /// Рассчитывает оптимальную позицию для атакующего юнита, чтобы он мог атаковать цель.
        /// Учитывает радиус атаки атакующего и радиусы тел обоих юнитов.
        /// </summary>
        /// <param name="target">Целевой юнит (враг)</param>
        /// <param name="attacker">Атакующий юнит</param>
        /// <returns>Позиция, куда должен подойти атакующий юнит</returns>
        public static Vector3 GetTargetPosition(this Unit target, Unit attacker)
        {
            if (!target || !attacker) 
                return attacker ? attacker.transform.position : Vector3.zero;

            var attackConfig = attacker.Parameters.GetConfig<UnitAttackConfig>();
            if (attackConfig == null)
            {
                // Если нет конфига атаки, используем старую логику (только радиусы тел)
                return GetTargetPositionByBodyRadius(target, attacker);
            }

            return GetAttackPosition(target, attacker, attackConfig.AttackRadius);
        }

        /// <summary>
        /// Рассчитывает максимальную дистанцию атаки с учетом радиусов тел обоих юнитов.
        /// Используется для проверки, находится ли враг в радиусе атаки.
        /// </summary>
        /// <param name="attackRadius">Радиус атаки атакующего юнита</param>
        /// <param name="attackerBodyRadius">Радиус тела атакующего юнита</param>
        /// <param name="enemyBodyRadius">Радиус тела врага</param>
        /// <returns>Максимальная дистанция между центрами для успешной атаки</returns>
        public static float GetEffectiveAttackDistance(float attackRadius, float attackerBodyRadius, float enemyBodyRadius)
        {
            // Минимальное расстояние между центрами (чтобы тела не пересекались)
            var minDistance = attackerBodyRadius + enemyBodyRadius;
            
            // Максимальное расстояние для атаки: радиус атаки должен доставать до края тела врага
            // Расстояние между центрами = радиус атаки + радиус тела врага
            var maxAttackDistance = attackRadius + enemyBodyRadius;
            
            // Не подходим ближе минимального расстояния
            // Если радиус атаки маленький (ближний бой), юнит подойдет вплотную (minDistance)
            // Если радиус атаки большой, юнит может стоять дальше (maxAttackDistance)
            return Mathf.Max(maxAttackDistance, minDistance);
        }

        /// <summary>
        /// Рассчитывает оптимальную дистанцию для подхода к врагу.
        /// Юнит должен подойти достаточно близко, чтобы враг гарантированно был в радиусе атаки.
        /// Радиус атаки измеряется от центра атакующего, поэтому нужно подходить ближе максимальной дистанции.
        /// </summary>
        /// <param name="attackRadius">Радиус атаки атакующего юнита</param>
        /// <param name="attackerBodyRadius">Радиус тела атакующего юнита</param>
        /// <param name="enemyBodyRadius">Радиус тела врага</param>
        /// <returns>Оптимальная дистанция между центрами для подхода к врагу</returns>
        public static float GetOptimalApproachDistance(float attackRadius, float attackerBodyRadius, float enemyBodyRadius)
        {
            // Минимальное расстояние между центрами (чтобы тела не пересекались)
            var minDistance = attackerBodyRadius + enemyBodyRadius;
            
            // Радиус атаки измеряется от центра атакующего юнита
            // Чтобы враг был в радиусе атаки, расстояние от центра атакующего до края тела врага должно быть <= attackRadius
            // То есть: расстояние между центрами <= attackRadius + enemyBodyRadius
            
            // Для подхода используем 70% от максимальной дистанции атаки
            // Это гарантирует, что враг будет внутри радиуса атаки с запасом
            // Как в Clash Royale - юниты подходят достаточно близко для надежной атаки
            var maxAttackDistance = attackRadius + enemyBodyRadius;
            var approachDistance = maxAttackDistance * 0.7f;
            
            // Но не ближе минимального расстояния
            return Mathf.Max(approachDistance, minDistance);
        }

        /// <summary>
        /// Универсальный метод для расчета позиции атаки с учетом всех параметров.
        /// </summary>
        /// <param name="target">Целевой юнит</param>
        /// <param name="attacker">Атакующий юнит</param>
        /// <param name="attackRadius">Радиус атаки атакующего юнита</param>
        /// <returns>Оптимальная позиция для атаки</returns>
        public static Vector3 GetAttackPosition(Unit target, Unit attacker, float attackRadius)
        {
            if (!target || !attacker) 
                return attacker ? attacker.transform.position : Vector3.zero;

            var direction = (target.transform.position - attacker.transform.position);
            var currentDistance = direction.magnitude;

            // Если юниты уже очень близко, используем текущее направление
            if (currentDistance < 0.01f)
            {
                direction = attacker.transform.forward;
            }
            else
            {
                direction = direction.normalized;
            }

            var attackerBodyRadius = attacker.Parameters.BodyRadius;
            var enemyBodyRadius = target.Parameters.BodyRadius;
            // Используем оптимальную дистанцию для подхода, чтобы гарантировать попадание в радиус атаки
            var targetDistance = GetOptimalApproachDistance(attackRadius, attackerBodyRadius, enemyBodyRadius);
            
            // Вычисляем позицию: от центра врага отступаем на targetDistance
            var targetPosition = target.transform.position - direction * targetDistance;
            
            return targetPosition;
        }

        /// <summary>
        /// Старый метод для расчета позиции только по радиусам тел (для обратной совместимости)
        /// </summary>
        private static Vector3 GetTargetPositionByBodyRadius(Unit target, Unit attacker)
        {
            var direction = (target.transform.position - attacker.transform.position).normalized;
            var enemyRadius = target.Parameters.BodyRadius;
            var attackerRadius = attacker.Parameters.BodyRadius;
            var totalRadius = enemyRadius + attackerRadius;
            return target.transform.position - direction * totalRadius;
        }
    }
}