using _ClashRoyal.Scripts.Units.Base.FSM.Interfaces;
using UnityEngine;

namespace _ClashRoyal.Scripts.Units.Base.FSM.Movements
{
    public class TransformMovement : IMovable
    {
        private readonly Transform _transform;
        private float _speed;

        public float Speed
        {
            get => _speed;
            set => _speed = value;
        }

        private Vector3 _targetPosition;
        private bool _isMoving;

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

            if (distance < 0.1f)
            {
                _isMoving = false;
                return;
            }

            var moveDistance = _speed * Time.deltaTime;
            if (moveDistance > distance)
            {
                moveDistance = distance;
            }

            _transform.position += direction.normalized * moveDistance;
        }
    }
}

