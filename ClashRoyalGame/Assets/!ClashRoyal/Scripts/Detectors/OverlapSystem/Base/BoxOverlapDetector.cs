using UnityEngine;

namespace _ClashRoyal.Scripts.Detectors.OverlapSystem.Base
{
    public abstract class BoxOverlapDetector<T> : OverlapDetector<T>
    {
        [Header("Box settings")] [SerializeField]
        private Vector3 size;
        
        protected override int GetOverlapResult(Vector3 position)
        {
            return Physics.OverlapBoxNonAlloc(position, size / 2, OverlapResults, startPoint.rotation, searchLayer.value);
        }

        protected override void DrawGizmos()
        {
            gizmos.DrawGizmos(offset, size, startPoint);
        }
    }
}