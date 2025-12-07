using UnityEngine;

namespace _ClashRoyal.Scripts.Units.Base.FSM.Interfaces
{
    public interface IMovable
    {
        public void MoveTo(Vector3 destination);
        public void Stop();
        public float Speed { get; set; }
        public void Update() { }
    }
}