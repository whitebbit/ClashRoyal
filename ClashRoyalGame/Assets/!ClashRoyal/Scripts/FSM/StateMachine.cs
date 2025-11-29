using System;
using System.Collections.Generic;
using System.Linq;
using _3._Scripts.FSM.Base;
using _3._Scripts.FSM.Interfaces;
using UnityEngine;
using UnityEngine.XR;

namespace _3._Scripts.FSM
{
    public class StateMachine
    {
        public IState CurrentState => _currentNode?.State;
        
        private StateNode _currentNode;
        
        private readonly Dictionary<Type, StateNode> _nodes = new();
        private readonly HashSet<ITransition> _anyTransitions = new();
        public void Update()
        {
            var transition = GetTransition();
            if (transition != null)
                ChangeState(transition.To);

            CurrentState?.Update();
        }

        public void FixedUpdate()
        {
            CurrentState?.FixedUpdate();
        }

        public void SetState(IState state)
        {
            _currentNode = GetOrAddNode(state);
            _currentNode.State?.OnEnter();
        }

        private void ChangeState(IState state)
        {
            if (_currentNode != null && state == _currentNode.State) return;

            var previousState = CurrentState;
            var nextNode = GetOrAddNode(state);
            var nextState = nextNode.State;

            previousState?.OnExit();
            nextState?.OnEnter();
            _currentNode = nextNode;
        }

        public void AddTransition(IState from, IState to, IPredicate condition)
        {
            GetOrAddNode(from).AddTransition(GetOrAddNode(to).State, condition);
        }

        public void AddAnyTransition(IState to, IPredicate condition)
        {
            _anyTransitions.Add(new Transition(GetOrAddNode(to).State, condition));
        }
        
        private StateNode GetOrAddNode(IState state)
        {
            var node = _nodes.GetValueOrDefault(state.GetType());
            
            if (node != null) return node;
            
            node = new StateNode(state);
            _nodes.Add(state.GetType(), node);

            return node;
        }

        private ITransition GetTransition()
        {
            foreach (var transition in _anyTransitions.Where(transition => transition.Condition.Evaluate()))
            {
                return transition;
            }

            if (_currentNode == null) return null;

            return _currentNode.Transitions.FirstOrDefault(transition => transition.Condition.Evaluate());
        }
    }
}