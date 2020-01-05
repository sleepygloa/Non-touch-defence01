using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public abstract class FSMState
{
    protected Dictionary<Transition, StateID> map = new Dictionary<Transition, StateID>();
    protected StateID stateID;
    protected bool changeState = false;
    public StateID ID { get { return stateID; } }
    public BaseController controller;
    public bool isSearchCompleted = false;

    /// <summary>
    /// A_state -> B_state 로 갈때 일어나는 Transition을 추가한다.
    /// <param name="trans">추가할 트랜지션</param>
    /// <param name="id">트랜지션의 종착 스테이트 A->B에 해당하는 트랜지션이면 B State를 추가한다.</param>
    /// </summary>
    public void AddTransition(Transition trans, StateID id)
    {
        // Check if anyone of the args is invalid
        if (trans == Transition.NullTransition)
        {
            Debug.LogError("FSMState ERROR: NullTransition is not allowed for a real transition");
            return;
        }

        if (id == StateID.NULLSTATEID)
        {
            Debug.LogError("FSMState ERROR: NullStateID is not allowed for a real ID");
            return;
        }

        // Since this is a Deterministic FSM,
        //   check if the current transition was already inside the map
        if (map.ContainsKey(trans))
        {
            if (this.controller == null)
            {
                Debug.LogError("FSMState ERROR: State " + stateID.ToString() + " already has transition " + trans.ToString() +
                           "Impossible to assign to another state");
            }
            else
            {
                Debug.LogError("FSMState ERROR: State " + stateID.ToString() + " already has transition " + trans.ToString() +
                           "Impossible to assign to another state : " + this.controller.gameObject.name);
            }

            return;
        }

        map.Add(trans, id);
    }

    /// <summary>
    /// 트랜지션 삭제.
    /// </summary>
    public void DeleteTransition(Transition trans)
    {
        // Check for NullTransition
        if (trans == Transition.NullTransition)
        {
            Debug.LogError("FSMState ERROR: NullTransition is not allowed");
            return;
        }

        // Check if the pair is inside the map before deleting
        if (map.ContainsKey(trans))
        {
            map.Remove(trans);
            return;
        }
        Debug.LogError("FSMState ERROR: Transition " + trans.ToString() + " passed to " + stateID.ToString() +
                       " was not on the state's transition list");
    }

    /// <summary>
    /// 스테이트 아이디 출력 .
    /// </summary>
    public StateID GetOutputState(Transition trans)
    {
        // Check if the map has this transition
        if (map.ContainsKey(trans))
        {
            return map[trans];
        }
        return StateID.NULLSTATEID;
    }

    /// <summary>
    /// 상태가 바뀌기 전에 처리.
    /// </summary>
    public virtual void DoBeforeEntering() { }

    /// <summary>
    /// 상태 조건 체크.
    /// </summary>
    public abstract void DoCheck();

    /// <summary>
    /// 이번 상태에서 처리할 행동.
    /// </summary>
    public abstract void DoAct();


    /// <summary>
    /// 상태가 끝나기 전에 처리.
    /// </summary>
    public virtual void DoBeforeLeaving() { }



} // class FSMState


/// <summary>
/// 상태머신 클래스.
/// </summary>
public class FSMSystem
{
    private List<FSMState> states;

    // 밖에서 직접 set하면 안된다.
    private StateID currentStateID;
    public StateID CurrentStateID { get { return currentStateID; } }
    private FSMState currentState;
    public FSMState CurrentState { get { return currentState; } }

    public void CreateStates()
    {
        states = new List<FSMState>();
    }

    /// <summary>
    /// 
    /// </summary>
    public void AddState(FSMState s)
    {
        // Check for Null reference before deleting
        if (s == null)
        {
            Debug.LogError("FSM ERROR: Null reference is not allowed");
        }

        // First State inserted is also the Initial state,
        //   the state the machine is in when the simulation begins
        if (states.Count == 0)
        {
            states.Add(s);
            currentState = s;
            currentStateID = s.ID;
            return;
        }

        // Add the state to the List if it's not inside it
        foreach (FSMState state in states)
        {
            if (state.ID == s.ID)
            {
                Debug.LogError("FSM ERROR: Impossible to add state " + s.ID.ToString() +
                               " because state has already been added");
                return;
            }
        }
        states.Add(s);
    }

    /// <summary>
    /// 상태 제거.
    /// </summary>
    public void DeleteState(StateID id)
    {
        // Check for NullState before deleting
        if (id == StateID.NULLSTATEID)
        {
            Debug.LogError("FSM ERROR: NullStateID is not allowed for a real state");
            return;
        }

        // Search the List and delete the state if it's inside it
        foreach (FSMState state in states)
        {
            if (state.ID == id)
            {
                states.Remove(state);
                return;
            }
        }
        Debug.LogError("FSM ERROR: Impossible to delete state " + id.ToString() +
                       ". It was not on the list of states");
    }

    /// <summary>
    /// 트랜지션 확인.
    /// </summary>
    public void PerformTransition(Transition trans)
    {
        // Check for NullTransition before changing the current state
        if (trans == Transition.NullTransition)
        {
            Debug.LogError("FSM ERROR: NullTransition is not allowed for a real transition");
            return;
        }

        // Check if the currentState has the transition passed as argument
        StateID id = currentState.GetOutputState(trans);
        if (id == StateID.NULLSTATEID)
        {
            if (currentState.controller == null)
            {
                Debug.LogError("FSM ERROR: State " + currentStateID.ToString() + " does not have a target state " + " for transition " + trans.ToString());
            }
            else
            {
                Debug.LogError("FSM ERROR: State " + currentStateID.ToString() + " does not have a target state " + " for transition " + trans.ToString() + " at " + currentState.controller.gameObject.name);
            }
            return;
        }

        // Update the currentStateID and currentState
        currentStateID = id;
        foreach (FSMState state in states)
        {
            if (state.ID == currentStateID)
            {
                // Do the post processing of the state before setting the new one
                currentState.DoBeforeLeaving();

                currentState = state;

                // Reset the state to its desired condition before it can reason or act
                currentState.DoBeforeEntering();
                break;
            }
        }

    } // PerformTransition()

} //class FSMSystem