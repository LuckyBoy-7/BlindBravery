using System;
using System.Collections;
using Lucky.Framework;
using Lucky.Kits.Utilities;
using UnityEngine;

namespace Lucky.Kits.Utilities
{
    [Serializable]
    public class StateMachine
    {
        private ManagedBehaviour entity;
        [SerializeField] private int state;
        [SerializeField] private string stateName;
        private string[] names;
        private Action[] begins;
        private Action[] ends;
        private Func<int>[] updates;
        private Func<IEnumerator>[] coroutines;
        private Coroutine currentCoroutine;

        public bool ChangedStates;
        public bool Log;
        public int PreviousState { get; private set; }

        public StateMachine(ManagedBehaviour entity, int maxStates = 10)
        {
            this.entity = entity;
            PreviousState = (state = -1);
            names = new string[maxStates];
            begins = new Action[maxStates];
            ends = new Action[maxStates];
            updates = new Func<int>[maxStates];
            coroutines = new Func<IEnumerator>[maxStates];
            currentCoroutine = null;
        }

        public int State
        {
            get => state;
            set
            {
                if (state != value)
                {
                    stateName = names[value];
                    if (Log)
                    {
                        Debug.Log(string.Concat("Enter State ", value, " (leaving ", state, ")"));
                    }

                    ChangedStates = true;
                    PreviousState = state;
                    state = value;
                    if (PreviousState != -1 && ends[PreviousState] != null)
                    {
                        if (Log)
                        {
                            Debug.Log("Calling End " + PreviousState);
                        }

                        ends[PreviousState]();
                    }

                    if (begins[state] != null)
                    {
                        if (Log)
                        {
                            Debug.Log("Calling Begin " + state);
                        }

                        begins[state]();
                    }

                    if (coroutines[state] != null)
                    {
                        if (Log)
                        {
                            Debug.Log("Starting Coroutine " + state);
                        }

                        if (currentCoroutine != null)
                            entity.StopCoroutine(currentCoroutine);
                        currentCoroutine = entity.StartCoroutine(coroutines[state]());
                        return;
                    }
                }
            }
        }


        public void SetCallbacks(int state, string name, Action begin = null, Action end = null, Func<int> onUpdate = null, Func<IEnumerator> coroutine = null)
        {
            names[state] = name;
            begins[state] = begin;
            ends[state] = end;
            updates[state] = onUpdate;
            coroutines[state] = coroutine;
        }

        public void Update()
        {
            ChangedStates = false;
            if (updates[state] != null)
            {
                State = updates[state]();
            }
        }

        public static implicit operator int(StateMachine s)
        {
            return s.state;
        }

        public bool AnyEqual(params int[] states)
        {
            foreach (int state in states)
            {
                if (state == State)
                    return true;
            }

            return false;
        }
    }
}