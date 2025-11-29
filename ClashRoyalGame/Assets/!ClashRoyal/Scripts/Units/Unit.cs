using System;
using _3._Scripts.FSM.Base;
using _ClashRoyal.Scripts.Units.FSM;
using UnityEngine;

namespace _ClashRoyal.Scripts.Units
{
    public abstract class Unit : MonoBehaviour
    {
        #region FIELDS SERIALIZED

        #endregion

        #region FIELDS

        protected abstract UnitFsm UnitFsm { get; }

        #endregion

        #region UNITY FUNCTIONS

        private void Start()
        {
            InitializeFsm();
        }

        private void Update()
        {
            UnitFsm?.FsmHandler?.StateMachine?.Update();
        }

        private void FixedUpdate()
        {
            UnitFsm?.FsmHandler?.StateMachine?.FixedUpdate();
        }

        #endregion

        #region METHODS

        protected virtual void InitializeFsm()
        {
            UnitFsm.Initialize(this);
            SubscribeToStates();
        }

        protected virtual void SubscribeToStates()
        {
        }

        #endregion
    }
}