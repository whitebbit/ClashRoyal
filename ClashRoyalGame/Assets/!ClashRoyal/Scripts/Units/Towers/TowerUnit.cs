using _ClashRoyal.Scripts.Units.FSM;
using UnityEngine;

namespace _ClashRoyal.Scripts.Units.Towers
{
    public class TowerUnit : Unit
    {
        [SerializeField] private TowerUnitFsm fsm;

        protected override UnitFsm UnitFsm => fsm;
    }
}