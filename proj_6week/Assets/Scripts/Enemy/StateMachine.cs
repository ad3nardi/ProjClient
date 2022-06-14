using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;
using Object = System.Object;

// ALEX'S

public class StateMachine
{
    private IState _currentState;

    private Dictionary<Type, List<Transition>> _transitions = new Dictionary<Type, List<Transition>>();
    private List<Transition> _currentTransitions = new List<Transition>();
    private List<Transition> _anyTransitions = new List<Transition>();
    private static List<Transition> EmptyTransitions = new List<Transition>();

    public void Tick()
    {
        var transition = GetTransition();
        if (transition != null)
            SetState(transition.To);

        if (_currentState != null)
        {
            _currentState.Tick();
            //statePatrol asPatrol = (statePatrol)_currentState;
            //if (asPatrol != null)
            //{
            //    if (asPatrol._reachedPos)
            //    {
            //        Debug.Log("YEAH BRO");
            //    }
            //}
        }
    }

    public void SetState(IState state)
    {
        if(state == _currentState)
            return;

        _currentState?.OnExit();
        _currentState = state;

        _transitions.TryGetValue(_currentState.GetType(), out _currentTransitions);

        if (_currentTransitions == null)
        {
            //_currentTransitions = EmptyTransitions;
            _currentTransitions = new List<Transition>();
        }
            

        _currentState.OnEnter();
    }
    public void AddTransition(IState from, IState to, conditionDelegate condition)
    {
        //if (_transitions.ContainsKey(from.GetType()))
        //{
        //    var transitions = _transitions[from.GetType()];
        //    transitions.Add(new Transition(to, condition));
        //} else
        //{
        //    var transitions = new List<Transition>();
        //    transitions.Add(new Transition(to, condition));
        //    _transitions.Add(from.GetType(), transitions);
        //}
        if (_transitions.TryGetValue(from.GetType(), out var transitions) == false)
        {
            transitions = new List<Transition>();
            _transitions[from.GetType()] = transitions;
        }
        transitions.Add(new Transition(to, condition));
    }
    
    public void AddAnyTransition(IState state, conditionDelegate condition)
    {
        _anyTransitions.Add(new Transition(state, condition));
    }

    private class Transition
    {
        public conditionDelegate Condition { get; }
        public IState To { get; }

        public Transition(IState to, conditionDelegate condition)
        {
            To = to;
            Condition = condition;
        }
    }

    private Transition GetTransition()
    {
        foreach (var transition in _anyTransitions)
            if(transition.Condition())
                return transition;

        foreach (var transition in _currentTransitions)
            if (transition.Condition())
                return transition;

        return null;  
    }

}
