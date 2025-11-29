using UnityEngine;

namespace _ClashRoyal.Scripts.Units.FSM.Interfaces
{
    public interface IMovable
    {
        public void MoveTo(Vector3 destination);
        public void Stop();
    }
}