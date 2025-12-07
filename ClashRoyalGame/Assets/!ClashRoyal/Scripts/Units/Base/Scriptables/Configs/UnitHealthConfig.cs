using UnityEngine;

namespace _ClashRoyal.Scripts.Units.Base.Scriptables.Configs
{
    [CreateAssetMenu(fileName = "_HealthConfig", menuName = "Configs/Unit/Configs/Health", order = 0)]
    public class UnitHealthConfig : UnitConfig
    {
        [Header("Health Settings")]
        [Tooltip("Максимальное количество здоровья юнита")]
        [SerializeField, Min(1f)] private float maxHealth = 100f;
        
        public float MaxHealth => maxHealth;
    }
}