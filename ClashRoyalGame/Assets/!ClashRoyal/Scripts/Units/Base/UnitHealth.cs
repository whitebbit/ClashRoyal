using System;
using UnityEngine;

namespace _ClashRoyal.Scripts.Units.Base
{
    public class UnitHealth
    {
        public float MaxHealth { get; }

        public float HealthPoints
        {
            get => _currentHealth;
            set
            {
                _currentHealth = Math.Clamp(value, 0, MaxHealth);
                Debug.Log($"{_currentHealth} / {MaxHealth}");
                OnHealthChanged?.Invoke(_currentHealth, MaxHealth);
                if (_currentHealth <= 0)
                    OnDying?.Invoke();
            }
        }

        public event Action<float, float> OnHealthChanged;
        public event Action OnDying;

        private float _currentHealth;

        public UnitHealth(float maxHealth)
        {
            MaxHealth = maxHealth;
            _currentHealth = maxHealth;
        }
    }
}