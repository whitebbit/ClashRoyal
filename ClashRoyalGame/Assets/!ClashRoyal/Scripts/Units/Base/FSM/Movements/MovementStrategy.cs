using _ClashRoyal.Scripts.Units.Base;
using _ClashRoyal.Scripts.Units.Base.FSM.Interfaces;
using UnityEngine;

namespace _ClashRoyal.Scripts.Units.Base.FSM.Movements
{
    public abstract class MovementStrategy : ScriptableObject
    {
        public abstract IMovable CreateMovable(Unit unit);
    }
}

