using System;
using _3._Scripts.FSM.Interfaces;
using _ClashRoyal.Scripts.Units;
using _ClashRoyal.Scripts.Units.FSM.Interfaces;
using UnityEngine;

namespace _ClashRoyal.Scripts.FSM.Base
{
    public abstract class State : ScriptableObject, IState
    {
        protected Unit Unit;

        public event Action OnEnterAction;
        public event Action OnExitAction;

        public void Construct(Unit unit)
        {
            Unit = unit;
        }

        public virtual void OnEnter()
        {
            OnEnterAction?.Invoke();
        }

        public virtual void Update()
        {
        }

        public virtual void FixedUpdate()
        {
        }

        public virtual void OnExit()
        {
            OnExitAction?.Invoke();
        }
    }
}