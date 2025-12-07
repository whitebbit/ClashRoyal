using _ClashRoyal.Scripts.Units.Base;
using UnityEngine;

namespace _ClashRoyal.Scripts.Units.Base.FSM.States.Strategies
{
    [CreateAssetMenu(fileName = "RangedAttackStrategy", menuName = "FSM/States/Attack/Strategies/Ranged", order = 2)]
    public class RangedAttackStrategy : AttackStrategy
    {
        [Header("Projectile Settings")]
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private Transform projectileSpawnPoint;
        [SerializeField] private float projectileSpeed = 10f;
        
        public override void Execute(Unit attacker, Unit target, float damage)
        {
            if (target == null || projectilePrefab == null) return;
            
            var spawnPosition = GetProjectileSpawnPosition(attacker);
            var projectileObj = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
            var projectile = projectileObj.GetComponent<Projectile>();
            
            if (projectile == null)
            {
                projectile = projectileObj.AddComponent<Projectile>();
            }
            
            projectile.Initialize(attacker, target, damage, projectileSpeed);
        }

        private Vector3 GetProjectileSpawnPosition(Unit attacker)
        {
            if (projectileSpawnPoint != null)
            {
                return projectileSpawnPoint.position;
            }
            
            return attacker.transform.position + Vector3.up * 0.5f;
        }
    }
}

