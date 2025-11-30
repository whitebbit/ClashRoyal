using _ClashRoyal.Scripts.Units.Base;
using _ClashRoyal.Scripts.Units.Base.FSM;
using UnityEngine;

namespace _ClashRoyal.Scripts.Units.Test
{
    public class TestUnit : Unit
    {
        [SerializeField] private TestUnitFsm fsm;
        protected override UnitFsm UnitFsm => fsm;
    }
}