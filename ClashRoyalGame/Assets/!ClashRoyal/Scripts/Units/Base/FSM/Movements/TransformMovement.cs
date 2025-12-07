using _ClashRoyal.Scripts.Units.Base.FSM.Interfaces;
using UnityEngine;

namespace _ClashRoyal.Scripts.Units.Base.FSM.Movements
{
    public class TransformMovement : IMovable
    {
        private readonly Transform _transform;
        private float _speed;
        private float _rotationSpeed = 720f; // Скорость поворота в градусах в секунду

        public float Speed
        {
            get => _speed;
            set => _speed = value;
        }

        private Vector3 _targetPosition;
        private bool _isMoving;
        private const float StopThreshold = 0.1f; // Порог остановки

        public TransformMovement(Unit unit)
        {
            _transform = unit.transform;
        }

        public void MoveTo(Vector3 destination)
        {
            _targetPosition = destination;
            _isMoving = true;
        }

        public void Stop()
        {
            _isMoving = false;
        }

        public void Update()
        {
            if (!_isMoving) return;

            var direction = (_targetPosition - _transform.position);
            var distance = direction.magnitude;

            // Проверяем, достигли ли мы цели
            if (distance < StopThreshold)
            {
                // Останавливаемся, но не устанавливаем точную позицию,
                // чтобы избежать проблем с движущимися целями
                _isMoving = false;
                return;
            }

            // Нормализуем направление для поворота и движения
            var normalizedDirection = direction.normalized;

            // Поворачиваем объект к цели
            if (normalizedDirection != Vector3.zero)
            {
                var targetRotation = Quaternion.LookRotation(normalizedDirection);
                _transform.rotation = Quaternion.RotateTowards(
                    _transform.rotation, 
                    targetRotation, 
                    _rotationSpeed * Time.deltaTime);
            }

            // Двигаемся к цели
            var moveDistance = _speed * Time.deltaTime;
            if (moveDistance >= distance)
            {
                // Если оставшееся расстояние меньше или равно расстоянию движения,
                // перемещаемся точно к цели и останавливаемся
                _transform.position = _targetPosition;
                _isMoving = false;
                return;
            }

            _transform.position += normalizedDirection * moveDistance;
        }
    }
}

