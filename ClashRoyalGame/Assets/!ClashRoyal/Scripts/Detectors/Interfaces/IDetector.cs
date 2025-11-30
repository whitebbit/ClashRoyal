using System;

namespace _ClashRoyal.Scripts.Detectors.Interfaces
{
    public interface IDetector<out T>
    {
        public bool ObjectsDetected();
        public event Action<T> OnFound;
    }
}