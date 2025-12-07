using System;
using UnityEngine;

namespace _ClashRoyal.Scripts.Units.Base
{
    public class UnitAnimator : MonoBehaviour
    {
        #region FIELDS SERIALIZED

        #endregion

        #region FIELDS

        private Animator _animator;

        #endregion

        #region UNITY FUNCTIONS

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        #endregion

        #region METHODS

        #endregion
    }
}