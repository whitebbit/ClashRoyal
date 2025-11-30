using System;
using _ClashRoyal.Scripts.Detectors.Interfaces;
using UnityEngine;

namespace _3._Scripts.Detectors
{
    public abstract class BaseDetector<T>: MonoBehaviour, IDetector<T>
    {
        public event Action<T> OnFound;
        public abstract bool ObjectsDetected();

        protected void CallOnFound(T obj)
        {
            OnFound?.Invoke(obj);
        }
    }
}