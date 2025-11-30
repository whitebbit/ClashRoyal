using System;
using UnityEngine;

namespace _ClashRoyal.Scripts.Extensions
{
    public class LookAtCamera : MonoBehaviour
    {
        private Transform _targetCamera;

        private void Awake()
        {
            if (Camera.main) _targetCamera = Camera.main.transform;
        }

        void LateUpdate()
        {
            if (_targetCamera == null) return;

            // направление на камеру в мировых координатах
            Vector3 direction = _targetCamera.transform.position - transform.position;

            // если направление почти нулевое, ничего не делаем
            if (direction.sqrMagnitude < 0.0001f) return;

            // создаём временный Quaternion, который смотрит на камеру
            Quaternion lookRotation = Quaternion.LookRotation(direction, Vector3.up);

            // вытаскиваем только X угол в мировых координатах
            Vector3 euler = lookRotation.eulerAngles;

            // применяем только X, Y и Z фиксируем, чтобы объект не переворачивался
            transform.rotation = Quaternion.Euler(-euler.x, 0f, 0f);
        }
    }
}