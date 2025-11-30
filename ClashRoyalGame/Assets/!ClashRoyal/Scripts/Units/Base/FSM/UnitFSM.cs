using _ClashRoyal.Scripts.FSM.Base;

namespace _ClashRoyal.Scripts.Units.Base.FSM
{
    public abstract class UnitFsm
    {
        protected Unit Unit;
        public FsmHandler FsmHandler {get; private set;}

        public virtual void Initialize(Unit unit)
        {
            Unit = unit;
            FsmHandler = new FsmHandler();
            SetFsmTransition();
        }

        protected abstract void SetFsmTransition();

        public virtual T GetState<T>() => default;
    }
}