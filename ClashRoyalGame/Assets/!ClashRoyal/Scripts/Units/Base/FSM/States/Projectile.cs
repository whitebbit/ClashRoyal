using _ClashRoyal.Scripts.Units.Base;
using UnityEngine;

namespace _ClashRoyal.Scripts.Units.Base.FSM.States
{
    public class Projectile : MonoBehaviour
    {
        private Unit _attacker;
        private Unit _target;
        private float _damage;
        private float _speed;
        private bool _isInitialized;

        public void Initialize(Unit attacker, Unit target, float damage, float speed)
        {
            _attacker = attacker;
            _target = target;
            _damage = damage;
            _speed = speed;
            _isInitialized = true;
        }

        private void Update()
        {
            if (!_isInitialized) return;
            
            if (_target == null || !_target.gameObject.activeInHierarchy)
            {
                Destroy(gameObject);
                return;
            }

            var direction = (_target.transform.position - transform.position).normalized;
            transform.position += direction * (_speed * Time.deltaTime);
            transform.rotation = Quaternion.LookRotation(direction);

            var distance = Vector3.Distance(transform.position, _target.transform.position);
            if (distance < 0.5f)
            {
                OnHit();
            }
        }

        private void OnHit()
        {
            if (_target != null)
            {
                _target.ApplyDamage(_damage);
            }
            
            Destroy(gameObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            var unit = other.GetComponent<Unit>();
            if (unit == _target)
            {
                OnHit();
            }
        }
    }
}

