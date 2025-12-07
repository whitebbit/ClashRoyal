using _ClashRoyal.Scripts.Units.Base;
using _ClashRoyal.Scripts.Units.Base.FSM.Interfaces;
using UnityEngine;

namespace _ClashRoyal.Scripts.Units.Base.FSM.Movements.Strategies
{
    [CreateAssetMenu(fileName = "_TransformMovementStrategy", menuName = "FSM/States/Movement/Strategies/Transform", order = 1)]
    public class TransformMovementStrategy : MovementStrategy
    {
        public override IMovable CreateMovable(Unit unit)
        {
            return new TransformMovement(unit);
        }
    }
}

