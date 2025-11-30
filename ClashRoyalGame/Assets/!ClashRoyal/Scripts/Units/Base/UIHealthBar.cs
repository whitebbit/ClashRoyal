using System;
using _ClashRoyal.Scripts.Units.Base;
using _ClashRoyal.Scripts.Units.Base.Enums;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Scripts.UI.Units
{
    public class UIHealthBar : MonoBehaviour
    {
        #region FIELDS SERIALIZED

        [SerializeField] private Unit unit;
        [SerializeField] private Slider slider;
        [SerializeField] private TMP_Text counterText;

        #endregion

        #region FIELDS

        #endregion

        #region UNITY FUNCTIONS

        private void Start()
        {
            UpdateHealth(unit.Health.HealthPoints, unit.Health.MaxHealth);
            unit.Health.OnHealthChanged += UpdateHealth;
        }

        private void OnDestroy()
        {
            unit.Health.OnHealthChanged -= UpdateHealth;
        }

        #endregion

        #region METHODS

        private void UpdateHealth(float currentHealth, float maxHealth)
        {
            var percent = currentHealth / maxHealth;

            slider.value = percent;

            if (counterText)
                counterText.text = $"{currentHealth}";
        }

        #endregion
    }
}