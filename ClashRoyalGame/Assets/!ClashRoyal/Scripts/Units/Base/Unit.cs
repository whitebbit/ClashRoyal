using System;
using _ClashRoyal.Scripts.Units.Base.Enums;
using _ClashRoyal.Scripts.Units.Base.FSM;
using _ClashRoyal.Scripts.Units.Base.Interfaces;
using _ClashRoyal.Scripts.Units.Base.Scriptables;
using _ClashRoyal.Scripts.Units.Base.Scriptables.Configs;
using UnityEngine;

namespace _ClashRoyal.Scripts.Units.Base
{
    public abstract class Unit : MonoBehaviour, IDamageable
    {
        #region FIELDS SERIALIZED

        [Header("Unit Identity")]
        [Tooltip("Команда, к которой принадлежит юнит")]
        [SerializeField] private TeamType teamType;
        
        [Header("Unit Configuration")]
        [Tooltip("Параметры и конфигурации юнита (здоровье, атака, движение и т.д.)")]
        [SerializeField] private UnitParameters parameters;

        #endregion

        #region FIELDS

        public TeamType TeamType => teamType;
        public UnitParameters Parameters => parameters;
        public UnitHealth Health { get; private set; }
        public UnitAnimator Animator { get; private set; }

        protected abstract UnitFsm UnitFsm { get; }

        #endregion

        #region UNITY FUNCTIONS

        private void Awake()
        {
            Health = new UnitHealth(Parameters.GetConfig<UnitHealthConfig>().MaxHealth);
            Animator = GetComponent<UnitAnimator>();
        }

        private void Start()
        {
            Health.OnDying += OnDying;
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

        private void OnDying()
        {
            Destroy(gameObject);
        }
        
        protected virtual void InitializeFsm()
        {
            UnitFsm.Initialize(this);
            SubscribeToStates();
        }

        protected virtual void SubscribeToStates()
        {
        }

        public void ApplyDamage(float damage)
        {
            Health.HealthPoints -= damage;
        }

        #endregion

        private void OnDrawGizmos()
        {
            UnitFsm?.OnDrawGizmos(this);
        }
    }
}