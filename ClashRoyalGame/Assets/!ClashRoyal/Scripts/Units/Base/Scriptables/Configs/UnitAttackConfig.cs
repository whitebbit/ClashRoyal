using _ClashRoyal.Scripts.Units.Base.Enums;
using UnityEngine;

namespace _ClashRoyal.Scripts.Units.Base.Scriptables.Configs
{
    [CreateAssetMenu(fileName = "_AttackConfig", menuName = "Configs/Unit/Configs/Attack", order = 0)]
    public class UnitAttackConfig : UnitConfig
    {
        [Header("Damage Settings")]
        [Tooltip("Урон, наносимый за одну атаку")]
        [SerializeField, Min(0f)] private float damage = 10f;
        
        [Header("Detection & Range")]
        [Tooltip("Радиус обнаружения врагов")]
        [SerializeField, Min(0f)] private float detectRadius = 5f;
        
        [Tooltip("Радиус атаки (максимальная дистанция для нанесения урона)")]
        [SerializeField, Min(0f)] private float attackRadius = 2f;
        
        [Header("Attack Rate")]
        [Tooltip("Скорость атаки (атак в секунду)")]
        [SerializeField, Min(0.1f)] private float attackRate = 1f;
        
        [Header("Target Types")]
        [Tooltip("Типы целей, которые может атаковать этот юнит")]
        [SerializeField] private TargetType canAttackTargets = TargetType.Ground | TargetType.Air | TargetType.Tower;
        
        public float AttackRadius => attackRadius;
        public float DetectRadius => detectRadius;
        public float AttackRate => attackRate;
        public float Damage => damage;
        public TargetType CanAttackTargets => canAttackTargets;
    }
}