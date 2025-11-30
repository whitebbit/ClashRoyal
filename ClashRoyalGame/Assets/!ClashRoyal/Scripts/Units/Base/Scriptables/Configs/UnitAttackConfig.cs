using UnityEngine;

namespace _ClashRoyal.Scripts.Units.Base.Scriptables.Configs
{
    [CreateAssetMenu(fileName = "_AttackConfig", menuName = "Configs/Unit/Configs/Attack", order = 0)]
    public class UnitAttackConfig : UnitConfig
    {
        [SerializeField] private float damage;
        
        [SerializeField] private float detectRadius;
        [SerializeField] private float attackRadius;
        [Space] [SerializeField] private float attackRate;
        
        public float AttackRadius => attackRadius;
        public float DetectRadius => detectRadius;
        public float AttackRate => attackRate;
        public float Damage => damage;
    }
}