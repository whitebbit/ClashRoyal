using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _ClashRoyal.Scripts.Units.Base
{
    public class UIHealthBar : MonoBehaviour
    {
        #region FIELDS SERIALIZED

        [SerializeField] private Unit unit;
        [SerializeField] private Slider slider;
        [SerializeField] private TMP_Text counterText;

        [SerializeField] private bool showOnDamage;

        #endregion

        #region FIELDS

        #endregion

        #region UNITY FUNCTIONS

        private void Start()
        {
            UpdateHealth(unit.Health.HealthPoints, unit.Health.MaxHealth);
            if (showOnDamage) UpdateActiveState(false);
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

            if (showOnDamage) UpdateActiveState(true);
        }

        public void UpdateActiveState(bool state)
        {
            if (counterText) counterText.gameObject.SetActive(state);

            if (slider) slider.gameObject.SetActive(state);
        }

        #endregion
    }
}