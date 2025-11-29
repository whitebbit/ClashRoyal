using System;
using _ClashRoyal.Scripts.Units.FSM;
using UnityEngine;

namespace _ClashRoyal.Scripts.Units
{
    public class Unit : MonoBehaviour
    {
        #region FIELDS SERIALIZED

        [SerializeField] private UnitState defaultState;
        [SerializeField] private UnitState chaseState;
        [SerializeField] private UnitState attackState;

        #endregion

        #region FIELDS

        private UnitState _defaultState;
        private UnitState _chaseState;
        private UnitState _attackState;

        private UnitState _currentState;

        #endregion

        #region UNITY FUNCTIONS

        private void Awake()
        {
            _defaultState = Instantiate(defaultState);
            _chaseState = Instantiate(chaseState);
            _attackState = Instantiate(attackState);

            _currentState = defaultState;
        }

        private void Update()
        {
            _currentState.OnUpdate();
        }

        #endregion

        #region METHODS

        #endregion
    }
}