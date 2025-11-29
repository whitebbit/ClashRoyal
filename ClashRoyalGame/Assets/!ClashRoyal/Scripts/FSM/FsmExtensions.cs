using _3._Scripts.FSM.Base;
using _ClashRoyal.Scripts.FSM.Base;
using _ClashRoyal.Scripts.Units;
using _ClashRoyal.Scripts.Units.FSM.Interfaces;
using UnityEngine;

namespace _ClashRoyal.Scripts.FSM
{
    public static class FsmExtensions
    {
        public static T CreateInstance<T>(this T state, Unit unit) where T : State
        {
            var instance = Object.Instantiate(state);
            instance.Construct(unit);
            return instance;
        }
    }
}