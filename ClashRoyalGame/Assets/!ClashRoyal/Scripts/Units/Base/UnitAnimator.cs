using System;
using System.Collections;
using _ClashRoyal.Scripts.Units.Base.Scriptables.Configs;
using UnityEngine;

namespace _ClashRoyal.Scripts.Units.Base
{
    /// <summary>
    /// Универсальный контроллер анимаций для юнитов.
    /// Управляет анимациями атаки, движения, получения урона и смерти.
    /// Автоматически подписывается на события Unit для управления анимациями здоровья.
    /// </summary>
    public class UnitAnimator : MonoBehaviour
    {
        #region FIELDS SERIALIZED

        [Header("Animation Configuration")]
        [Tooltip("Конфигурация параметров аниматора. Если не указана, используются значения по умолчанию.")]
        [SerializeField] private UnitAnimatorConfig animatorConfig;

        #endregion

        #region FIELDS

        private Animator _animator;
        private Unit _unit;
        private UnitAnimatorConfig _config;
        
        // Кэшированные хеши параметров для производительности
        private int _attackTriggerHash;
        private int _deathTriggerHash;
        private int _isMovingBoolHash;
        private int _speedFloatHash;
        
        private bool _isInitialized;
        private float _currentSpeed;
        private bool _isMoving;
        
        // Кэшированные значения для автоматического расчета
        private float _cachedReferenceSpeed;
        private float _cachedBaseAnimationSpeed;
        private float _cachedMaxSpeedForNormalization;
        
        // Для управления скоростью анимации атаки
        private Coroutine _attackAnimationSpeedCoroutine;
        private float _baseAnimationSpeedBeforeAttack = 1f;
        private bool _isAttackAnimationActive;

        #endregion

        #region UNITY FUNCTIONS

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _unit = GetComponent<Unit>();
        }

        private void Start()
        {
            Initialize();
        }

        #endregion

        #region METHODS

        /// <summary>
        /// Инициализация контроллера анимаций.
        /// Должен быть вызван после инициализации Unit.
        /// </summary>
        public void Initialize()
        {
            if (_isInitialized) return;
            
            if (_unit == null)
            {
                Debug.LogWarning($"UnitAnimator на {gameObject.name}: Unit компонент не найден!");
                return;
            }

            _config = _unit.Parameters?.GetConfig<UnitAnimatorConfig>() ?? animatorConfig;
            
            if (_config == null)
            {
                Debug.LogWarning($"UnitAnimator на {gameObject.name}: AnimatorConfig не найден, используются значения по умолчанию.");
                InitializeWithDefaults();
            }
            else
            {
                InitializeWithConfig(_config);
            }
            
            // Автоматически вычисляем значения из MovementConfig, если они не заданы
            CacheMovementConfigValues();

            _isInitialized = true;
        }

        private void InitializeWithConfig(UnitAnimatorConfig config)
        {
            _attackTriggerHash = Animator.StringToHash(config.AttackTriggerName);
            _deathTriggerHash = Animator.StringToHash(config.DeathTriggerName);
            _isMovingBoolHash = Animator.StringToHash(config.IsMovingBoolName);
            _speedFloatHash = Animator.StringToHash(config.SpeedFloatName);
         
        }

        private void InitializeWithDefaults()
        {
            _attackTriggerHash = Animator.StringToHash("Attack");
            _deathTriggerHash = Animator.StringToHash("Death");
            _isMovingBoolHash = Animator.StringToHash("IsMoving");
            _speedFloatHash = Animator.StringToHash("Speed");
        }
        
        /// <summary>
        /// Кэширует значения из MovementConfig для автоматического расчета скорости анимации.
        /// </summary>
        private void CacheMovementConfigValues()
        {
            if (_unit?.Parameters == null)
            {
                Debug.LogWarning($"UnitAnimator на {gameObject.name}: UnitParameters не найден, скорость анимации не будет синхронизироваться!");
                return;
            }
            
            var movementConfig = _unit.Parameters.GetConfig<UnitMovementConfig>();
            if (movementConfig == null)
            {
                Debug.LogWarning($"UnitAnimator на {gameObject.name}: UnitMovementConfig не найден, скорость анимации не будет синхронизироваться!");
                return;
            }
            
            // Всегда используем скорость из MovementConfig
            _cachedReferenceSpeed = movementConfig.Speed;
            _cachedMaxSpeedForNormalization = movementConfig.Speed;
            _cachedBaseAnimationSpeed = 1f; // Базовая скорость анимации всегда 1.0
            
            // Убеждаемся, что значения не равны нулю
            if (_cachedReferenceSpeed <= 0f)
            {
                Debug.LogWarning($"UnitAnimator на {gameObject.name}: Скорость из MovementConfig равна нулю или отрицательна ({_cachedReferenceSpeed})!");
                _cachedReferenceSpeed = 3f; // Значение по умолчанию
            }
        }

        #region Public Animation Control Methods

        /// <summary>
        /// Воспроизводит анимацию атаки.
        /// Автоматически регулирует скорость анимации, если скорость атаки быстрее длительности анимации.
        /// </summary>
        public void PlayAttackAnimation()
        {
            if (!ValidateAnimator()) return;
            
            // Останавливаем предыдущую корутину, если она еще работает
            if (_attackAnimationSpeedCoroutine != null)
            {
                StopCoroutine(_attackAnimationSpeedCoroutine);
            }
            
            // Сохраняем текущую скорость анимации перед атакой
            _baseAnimationSpeedBeforeAttack = _animator.speed;
            
            // Устанавливаем триггер атаки
            _animator.SetTrigger(_attackTriggerHash);
            
            // Запускаем корутину для автоматической регулировки скорости анимации атаки
            _attackAnimationSpeedCoroutine = StartCoroutine(AdjustAttackAnimationSpeed());
        }
        
        /// <summary>
        /// Корутина для автоматической регулировки скорости анимации атаки.
        /// Ускоряет анимацию, если скорость атаки быстрее длительности анимации.
        /// </summary>
        private IEnumerator AdjustAttackAnimationSpeed()
        {
            if (_unit?.Parameters == null) yield break;
            
            var attackConfig = _unit.Parameters.GetConfig<Scriptables.Configs.UnitAttackConfig>();
            if (attackConfig == null) yield break;
            
            float attackRate = attackConfig.AttackRate;
            
            // Ждем несколько кадров, чтобы анимация успела перейти в состояние атаки
            yield return null;
            yield return null;
            
            // Получаем информацию о текущем состоянии аниматора
            AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            
            // Пытаемся найти состояние атаки - проверяем все слои
            bool foundAttackState = false;
            for (int layer = 0; layer < _animator.layerCount; layer++)
            {
                stateInfo = _animator.GetCurrentAnimatorStateInfo(layer);
                
                // Проверяем по тегу "Attack" или по имени, содержащему "Attack"
                if (stateInfo.IsTag("Attack") || 
                    stateInfo.IsName("Attack") || 
                    stateInfo.fullPathHash == _attackTriggerHash)
                {
                    foundAttackState = true;
                    break;
                }
            }
            
            // Если не нашли состояние атаки, выходим
            if (!foundAttackState)
            {
                _attackAnimationSpeedCoroutine = null;
                yield break;
            }
            
            // Получаем длительность анимации атаки
            float animationDuration = stateInfo.length;
            
            // Если длительность анимации больше скорости атаки, нужно ускорить
            if (animationDuration > attackRate && animationDuration > 0f)
            {
                // Вычисляем множитель скорости: нужно чтобы анимация завершилась за время attackRate
                float speedMultiplier = animationDuration / attackRate;
                
                // Применяем ускорение только к анимации атаки
                _animator.speed = _baseAnimationSpeedBeforeAttack * speedMultiplier;
                _isAttackAnimationActive = true;
                
                // Ждем завершения анимации атаки или пока не пройдет время attackRate
                float elapsedTime = 0f;
                float maxWaitTime = Mathf.Min(attackRate, animationDuration / speedMultiplier);
                
                while (elapsedTime < maxWaitTime && _isAttackAnimationActive)
                {
                    // Проверяем, все еще ли мы в состоянии атаки
                    bool stillInAttack = false;
                    for (int layer = 0; layer < _animator.layerCount; layer++)
                    {
                        stateInfo = _animator.GetCurrentAnimatorStateInfo(layer);
                        if (stateInfo.IsTag("Attack") || 
                            stateInfo.IsName("Attack") || 
                            stateInfo.fullPathHash == _attackTriggerHash)
                        {
                            stillInAttack = true;
                            
                            // Если анимация завершилась (normalizedTime >= 1), выходим
                            if (stateInfo.normalizedTime >= 1f)
                            {
                                _isAttackAnimationActive = false;
                            }
                            break;
                        }
                    }
                    
                    // Если вышли из состояния атаки, прекращаем ожидание
                    if (!stillInAttack)
                    {
                        break;
                    }
                    
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }
                
                // Восстанавливаем нормальную скорость анимации на основе текущей скорости движения
                _isAttackAnimationActive = false;
                // Пересчитываем скорость на основе текущей скорости движения
                if (_isMoving && _currentSpeed > 0f)
                {
                    UpdateAnimationSpeed(_currentSpeed);
                }
                else
                {
                    _animator.speed = _baseAnimationSpeedBeforeAttack;
                }
            }
            else
            {
                // Если скорость атаки медленнее или равна длительности анимации,
                // не изменяем скорость - анимация сама перейдет в idle
                _isAttackAnimationActive = false;
            }
            
            _attackAnimationSpeedCoroutine = null;
        }

        /// <summary>
        /// Воспроизводит анимацию смерти.
        /// </summary>
        public void PlayDeathAnimation()
        {
            if (!ValidateAnimator()) return;
            _animator.SetTrigger(_deathTriggerHash);
        }

        /// <summary>
        /// Устанавливает состояние движения.
        /// </summary>
        /// <param name="isMoving">Движется ли юнит</param>
        public void SetIsMoving(bool isMoving)
        {
            if (!ValidateAnimator()) return;
            _isMoving = isMoving;
            _animator.SetBool(_isMovingBoolHash, isMoving);
            
            if (!isMoving)
            {
                // Если не движется, сбрасываем скорость и скорость анимации
                SetSpeed(0f);
                ResetAnimationSpeed();
            }
            else
            {
                // Если начали двигаться, обновляем скорость анимации на основе текущей скорости
                if (_currentSpeed > 0f)
                {
                    UpdateAnimationSpeed(_currentSpeed);
                }
            }
        }

        /// <summary>
        /// Устанавливает скорость движения.
        /// Устанавливает параметр Speed равным реальной скорости юнита (для использования в Animator Controller).
        /// Также автоматически синхронизирует глобальную скорость воспроизведения анимации.
        /// </summary>
        /// <param name="speed">Текущая скорость движения юнита</param>
        public void SetSpeed(float speed)
        {
            if (!ValidateAnimator()) return;
            _currentSpeed = speed;
            
            // Устанавливаем параметр Speed равным реальной скорости юнита
            // В Animator Controller можно использовать этот параметр как Multiplier для скорости анимации
            // Например: если скорость 0.1, то параметр будет 0.1 (анимация в 10 раз медленнее)
            // Если скорость 3.0, то параметр будет 3.0 (анимация в 3 раза быстрее)
            float speedValue = Mathf.Max(0f, speed);
            
            // Ограничиваем для стабильности (0.0 - 10.0)
            speedValue = Mathf.Clamp(speedValue, 0f, 10f);
            
            _animator.SetFloat(_speedFloatHash, speedValue);
            
            // Автоматически синхронизируем глобальную скорость анимации со скоростью движения
            UpdateAnimationSpeed(speed);
        }
        
        /// <summary>
        /// Обновляет скорость воспроизведения анимации на основе скорости движения юнита.
        /// Автоматически использует значения из MovementConfig.
        /// Не изменяет скорость во время активной анимации атаки.
        /// </summary>
        /// <param name="unitSpeed">Текущая скорость движения юнита</param>
        public void UpdateAnimationSpeed(float unitSpeed)
        {
            if (!ValidateAnimator()) return;
            
            // Не изменяем скорость во время активной анимации атаки
            if (_isAttackAnimationActive) return;
            
            // Используем кэшированные значения (автоматически вычисленные из MovementConfig)
            float referenceSpeed = _cachedReferenceSpeed > 0f ? _cachedReferenceSpeed : 3f;
            float baseSpeed = _cachedBaseAnimationSpeed > 0f ? _cachedBaseAnimationSpeed : 1f;
            
            // Если скорость равна нулю, сбрасываем к базовой
            if (unitSpeed <= 0f)
            {
                _animator.speed = baseSpeed;
                return;
            }
            
            // Вычисляем множитель скорости анимации на основе скорости юнита
            // Скорость анимации пропорциональна скорости движения
            float speedMultiplier = referenceSpeed > 0f ? unitSpeed / referenceSpeed : 1f;
            
            // Применяем базовую скорость и множитель
            float finalSpeed = baseSpeed * speedMultiplier;
            
            // Ограничиваем минимальную и максимальную скорость для стабильности
            finalSpeed = Mathf.Clamp(finalSpeed, 0.1f, 3f);
            
            _animator.speed = finalSpeed;
        }
        
        /// <summary>
        /// Устанавливает скорость воспроизведения анимации напрямую.
        /// </summary>
        /// <param name="speed">Скорость анимации (1.0 = нормальная скорость)</param>
        public void SetAnimationSpeed(float speed)
        {
            if (!ValidateAnimator()) return;
            _animator.speed = Mathf.Clamp(speed, 0.1f, 3f);
        }
        
        /// <summary>
        /// Сбрасывает скорость анимации к базовому значению.
        /// </summary>
        public void ResetAnimationSpeed()
        {
            if (!ValidateAnimator()) return;
            
            float baseSpeed = _cachedBaseAnimationSpeed > 0f ? _cachedBaseAnimationSpeed : 1f;
            _animator.speed = baseSpeed;
        }

        /// <summary>
        /// Устанавливает скорость движения напрямую (уже нормализованную).
        /// </summary>
        /// <param name="normalizedSpeed">Нормализованная скорость (0-1)</param>
        public void SetNormalizedSpeed(float normalizedSpeed)
        {
            if (!ValidateAnimator()) return;
            _animator.SetFloat(_speedFloatHash, Mathf.Clamp01(normalizedSpeed));
        }

        /// <summary>
        /// Устанавливает триггер по имени параметра (для расширяемости).
        /// </summary>
        /// <param name="triggerName">Имя параметра триггера</param>
        public void SetTrigger(string triggerName)
        {
            if (!ValidateAnimator()) return;
            _animator.SetTrigger(triggerName);
        }

        /// <summary>
        /// Устанавливает bool параметр по имени (для расширяемости).
        /// </summary>
        /// <param name="boolName">Имя параметра bool</param>
        /// <param name="value">Значение</param>
        public void SetBool(string boolName, bool value)
        {
            if (!ValidateAnimator()) return;
            _animator.SetBool(boolName, value);
        }

        /// <summary>
        /// Устанавливает float параметр по имени (для расширяемости).
        /// </summary>
        /// <param name="floatName">Имя параметра float</param>
        /// <param name="value">Значение</param>
        public void SetFloat(string floatName, float value)
        {
            if (!ValidateAnimator()) return;
            _animator.SetFloat(floatName, value);
        }

        /// <summary>
        /// Устанавливает int параметр по имени (для расширяемости).
        /// </summary>
        /// <param name="intName">Имя параметра int</param>
        /// <param name="value">Значение</param>
        public void SetInt(string intName, int value)
        {
            if (!ValidateAnimator()) return;
            _animator.SetInteger(intName, value);
        }

        #endregion

        #region Helper Methods

        private bool ValidateAnimator()
        {
            if (_animator == null)
            {
                Debug.LogWarning($"UnitAnimator на {gameObject.name}: Animator компонент не найден!");
                return false;
            }

            if (!_isInitialized)
            {
                Initialize();
            }

            return true;
        }

        /// <summary>
        /// Получает текущее состояние движения.
        /// </summary>
        public bool IsMoving => _isMoving;

        /// <summary>
        /// Получает текущую скорость.
        /// </summary>
        public float CurrentSpeed => _currentSpeed;

        /// <summary>
        /// Получает ссылку на Unity Animator (для расширенного использования).
        /// </summary>
        public Animator UnityAnimator => _animator;

        #endregion

        #endregion
    }
}