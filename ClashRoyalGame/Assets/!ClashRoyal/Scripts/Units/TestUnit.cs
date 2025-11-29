using _ClashRoyal.Scripts.Units.FSM;
using UnityEngine;

namespace _ClashRoyal.Scripts.Units
{
    public class TestUnit : Unit
    {
        [SerializeField] private TestUnitFsm fsm;

        protected override UnitFsm UnitFsm => fsm;
    }
}