using _ClashRoyal.Scripts.Units.Base;
using _ClashRoyal.Scripts.Units.Base.Enums;
using _ClashRoyal.Scripts.Units.Base.FSM;
using UnityEngine;

namespace _ClashRoyal.Scripts.Units.BaseUnit
{
    public class BaseUnit : Unit
    {
        [Header("FSM Configuration")]
        [Tooltip("Конечный автомат состояний для базового юнита")]
        [SerializeField] private BaseUnitFsm fsm;
        
        protected override UnitFsm UnitFsm => fsm;
    }
}