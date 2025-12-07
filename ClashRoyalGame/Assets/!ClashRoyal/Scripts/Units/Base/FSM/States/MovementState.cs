using System;
using _ClashRoyal.Scripts.FSM.Base;
using _ClashRoyal.Scripts.Units.Base.FSM.Interfaces;
using _ClashRoyal.Scripts.Units.Base.FSM.Movements;
using UnityEngine;

namespace _ClashRoyal.Scripts.Units.Base.FSM.States
{
    [CreateAssetMenu(fileName = "_MovementState", menuName = "FSM/States/Movement", order = 0)]
    public class MovementState : State
    {
        [Header("Movement Strategy")]
        [SerializeField] private MovementStrategy movementStrategy;
        
        public MovementStrategy MovementStrategy => movementStrategy;
        public IMovable Movable { private get; set; }
        public Func<Vector3> TargetProvider { private get; set; }
        public Func<bool> ShouldStopMoving { private get; set; }
        
        private Vector3 _destination;
        private const float DestinationUpdateThreshold = 0.5f; // Обновляем цель только если она сдвинулась больше чем на 0.5 единицы

        public override void OnEnter()
        {
            base.OnEnter();
            
            // Управление анимациями движения
            if (Unit?.Animator != null)
            {
                Unit.Animator.SetIsMoving(true);
                if (Movable != null)
                {
                    Unit.Animator.SetSpeed(Movable.Speed);
                }
            }
            
            UpdateDestination();
        }
        
        public override void Update()
        {
            // Проверяем, нужно ли остановиться (например, цель в радиусе атаки)
            if (ShouldStopMoving != null && ShouldStopMoving.Invoke())
            {
                Movable?.Stop();
                if (Unit?.Animator != null)
                {
                    Unit.Animator.SetIsMoving(false);
                }
                return;
            }
            
            // Обновляем цель только если не нужно останавливаться
            UpdateDestination();
            Movable?.Update();
            
            // Обновляем скорость анимации на основе текущей скорости движения
            if (Unit?.Animator != null && Movable != null)
            {
                float currentSpeed = Movable.Speed;
                Unit.Animator.SetSpeed(currentSpeed);
            }
        }
        
        public override void OnExit()
        {
            base.OnExit();
            Movable?.Stop();
            
            // Останавливаем анимации движения
            if (Unit?.Animator != null)
            {
                Unit.Animator.SetIsMoving(false);
            }
        }
        
        private void UpdateDestination()
        {
            if (TargetProvider == null) return;
            
            // Не обновляем цель, если нужно остановиться
            if (ShouldStopMoving != null && ShouldStopMoving.Invoke()) return;

            var newDest = TargetProvider.Invoke();

            // Обновляем цель только если она значительно сдвинулась
            if (!((_destination - newDest).sqrMagnitude > DestinationUpdateThreshold * DestinationUpdateThreshold)) return;
            
            _destination = newDest;
            Movable?.MoveTo(_destination);
        }
    }
}