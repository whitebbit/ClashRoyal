using _ClashRoyal.Scripts.Units.Base;
using _ClashRoyal.Scripts.Units.Base.FSM.Interfaces;
using UnityEngine;

namespace _ClashRoyal.Scripts.Units.Base.FSM.Movements.Strategies
{
    [CreateAssetMenu(fileName = "_NavMeshMovementStrategy", menuName = "FSM/States/Movement/Strategies/NavMesh", order = 0)]
    public class NavMeshMovementStrategy : MovementStrategy
    {
        public override IMovable CreateMovable(Unit unit)
        {
            return new NavMeshMovement(unit);
        }
    }
}

