using UnityEngine;

namespace _ClashRoyal.Scripts.Units.Base.Scriptables.Configs
{
    [CreateAssetMenu(fileName = "_AnimatorConfig", menuName = "Configs/Unit/Configs/Animator", order = 0)]
    public class UnitAnimatorConfig : UnitConfig
    {
        [Header("Animation Parameter Names")]
        [Tooltip("Имя параметра триггера для атаки")]
        [SerializeField] private string attackTriggerName = "Attack";
        
        [Tooltip("Имя параметра триггера для смерти")]
        [SerializeField] private string deathTriggerName = "Death";
        
        [Tooltip("Имя параметра bool для движения")]
        [SerializeField] private string isMovingBoolName = "IsMoving";
        
        [Tooltip("Имя параметра float для скорости (нормализованной)")]
        [SerializeField] private string speedFloatName = "Speed";
        
        public string AttackTriggerName => attackTriggerName;
        public string DeathTriggerName => deathTriggerName;
        public string IsMovingBoolName => isMovingBoolName;
        public string SpeedFloatName => speedFloatName;
    }
}

