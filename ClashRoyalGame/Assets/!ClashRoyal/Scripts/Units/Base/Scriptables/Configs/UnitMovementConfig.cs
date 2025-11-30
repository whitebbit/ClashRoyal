using UnityEngine;

namespace _ClashRoyal.Scripts.Units.Base.Scriptables.Configs
{
    [CreateAssetMenu(fileName = "_MovementConfig", menuName = "Configs/Unit/Configs/Movement", order = 0)]
    public class UnitMovementConfig : UnitConfig
    {
        [SerializeField] private float speed;
        public float Speed => speed;
    }
}