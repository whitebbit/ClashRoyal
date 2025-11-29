using _3._Scripts.FSM;
using _3._Scripts.FSM.Interfaces;

namespace _ClashRoyal.Scripts.FSM.Base
{
    public class FsmHandler
    {
        public readonly StateMachine StateMachine = new();
        
        public void AddTransition(IState from, IState to, IPredicate condition) =>
            StateMachine.AddTransition(from, to, condition);
        
        public void AddTransition(IState to, IPredicate condition) =>
            StateMachine.AddAnyTransition(to, condition);
    }
}