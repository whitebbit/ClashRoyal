using _ClashRoyal.Scripts.FSM.Base;

namespace _ClashRoyal.Scripts.Units.FSM
{
    public abstract class UnitFsm
    {
        protected Unit Unit;
        public FsmHandler FsmHandler {get; private set;}

        public void Initialize(Unit unit)
        {
            Unit = unit;
            FsmHandler = new FsmHandler();
            SetFsmTransition();
        }

        protected abstract void SetFsmTransition();

        public virtual T GetState<T>() => default;
    }
}