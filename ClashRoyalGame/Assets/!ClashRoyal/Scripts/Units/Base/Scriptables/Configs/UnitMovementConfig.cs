using UnityEngine;

namespace _ClashRoyal.Scripts.Units.Base.Scriptables.Configs
{
    [CreateAssetMenu(fileName = "_MovementConfig", menuName = "Configs/Unit/Configs/Movement", order = 0)]
    public class UnitMovementConfig : UnitConfig
    {
        [Header("Movement Settings")]
        [Tooltip("Скорость перемещения юнита (единиц в секунду)")]
        [SerializeField, Min(0f)] private float speed = 3f;
        
        [Header("Chase Settings")]
        [Tooltip("Радиус преследования цели (на каком расстоянии юнит прекратит преследование)")]
        [SerializeField, Min(0f)] private float chaseRadius = 10f;
        
        public float Speed => speed;
        public float ChaseRadius => chaseRadius;
    }
}