using _ClashRoyal.Scripts.Units.Base.FSM.Interfaces;
using _ClashRoyal.Scripts.Units.Base.Scriptables.Configs;
using UnityEngine;
using UnityEngine.AI;

namespace _ClashRoyal.Scripts.Units.Base.FSM.Movements
{
    public class NavMeshMovement : IMovable
    {
        private readonly NavMeshAgent _agent;
        private float _speed;

        public float Speed
        {
            get => _agent.speed;
            set
            {
                _speed = value;
                _agent.speed = value;
            }
        }

        public NavMeshMovement(Unit unit)
        {
            _agent = unit.GetComponent<NavMeshAgent>();
            _agent.radius = unit.Parameters.BodyRadius;
        }

        public void MoveTo(Vector3 destination)
        {
            _agent.isStopped = false;
            _agent.SetDestination(destination);
        }

        public void Stop()
        {
            _agent.isStopped = true;
        }
    }
}