using UnityEngine;

namespace _ClashRoyal.Scripts.Units.Base.Scriptables.Configs
{
    [CreateAssetMenu(fileName = "_AttackConfig", menuName = "Configs/Unit/Configs/Attack", order = 0)]
    public class UnitAttackConfig : UnitConfig
    {
        [SerializeField] private float attackRadius;
        public float AttackRadius => attackRadius;
    }
}