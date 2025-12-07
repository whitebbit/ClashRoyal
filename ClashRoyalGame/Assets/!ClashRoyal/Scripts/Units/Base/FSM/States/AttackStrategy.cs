using _ClashRoyal.Scripts.Units.Base;
using UnityEngine;

namespace _ClashRoyal.Scripts.Units.Base.FSM.States
{
    public abstract class AttackStrategy : ScriptableObject
    {
        public abstract void Execute(Unit attacker, Unit target, float damage);
    }
}

