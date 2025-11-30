using _ClashRoyal.Scripts.Units.Base.FSM.Interfaces;
using UnityEngine;
using UnityEngine.AI;

namespace _ClashRoyal.Scripts.Units.Base.FSM.Movements
{
    public class NavMeshMovement : IMovable
    {
        private readonly NavMeshAgent _agent;

        public float Speed
        {
            get => _agent.speed;
            set => _agent.speed = value;
        }

        public NavMeshMovement(Unit unit)
        {
            _agent = unit.GetComponent<NavMeshAgent>();
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