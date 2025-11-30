using UnityEngine;

namespace _ClashRoyal.Scripts.Units.Base.Scriptables.Configs
{
    [CreateAssetMenu(fileName = "_HealthConfig", menuName = "Configs/Unit/Configs/Health", order = 0)]
    public class UnitHealthConfig : UnitConfig
    {
        [SerializeField] private float maxHealth;
        public float MaxHealth => maxHealth;
    }
}