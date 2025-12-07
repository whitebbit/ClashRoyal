using _ClashRoyal.Scripts.Units.Base;
using _ClashRoyal.Scripts.Units.Base.Enums;
using _ClashRoyal.Scripts.Units.Base.FSM;
using UnityEngine;

namespace _ClashRoyal.Scripts.Units.Towers
{
    public class TowerUnit : Unit
    {
        [Header("FSM Configuration")]
        [Tooltip("Конечный автомат состояний для башни")]
        [SerializeField] private TowerUnitFsm fsm;

        protected override UnitFsm UnitFsm => fsm;
    }
}