using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingController : BaseController
{
    public Transform myTarget = null;
    public DefenseBuildingAniController aniController;

    public override void Init(Entity owner)
    {
        base.Init(owner);
        if(this.aniController == null)
        {
            this.aniController = gameObject.GetComponent<DefenseBuildingAniController>();
            if(this.aniController == null)
            {
                this.aniController = gameObject.AddComponent<DefenseBuildingAniController>();
            }
        }
        this.aniController.Init(this.OwnerEntity.category);
        //FSM
        FSMSetup();
    }

    public override void FSMSetup()
    {
        this.myTarget = null;

        BuildingState_IDLE state_IDLE = new BuildingState_IDLE(this);
        state_IDLE.AddTransition(Transition.IdleToSearch, StateID.SEARCH);

        BuildingState_TargetFind state_TargetFind = new BuildingState_TargetFind(this);
        state_TargetFind.AddTransition(Transition.SearchToAttack, StateID.ATTACK);
        state_TargetFind.AddTransition(Transition.SearchToIdle, StateID.IDLE);

        BuildingState_Attack state_Attack = new BuildingState_Attack(this);
        state_Attack.AddTransition(Transition.AttackToSearch, StateID.SEARCH);
        state_Attack.AddTransition(Transition.AttackToIdle, StateID.IDLE);

        Fsm = new FSMSystem();
        Fsm.CreateStates();
        Fsm.AddState(state_IDLE);
        Fsm.AddState(state_TargetFind);
        Fsm.AddState(state_Attack);
    }

    public override void UpdateController()
    {
        if(aniController != null)
        {
            aniController.UpdateAnimation();
        }
        if(Fsm != null && Fsm.CurrentState != null)
        {
            Fsm.CurrentState.DoCheck();
            Fsm.CurrentState.DoAct();
        }
    }
    //내가 파괴된 경우.
    public override void OnDestoryEntity()
    {
        this.myTarget = null;
        if(CurrentStateID == StateID.SEARCH)
        {
            SetTransition(Transition.SearchToIdle);
        }else if(CurrentStateID == StateID.ATTACK)
        {
            SetTransition(Transition.AttackToIdle);
        }
    }
    //내 타겟 유닛이 파괴된 경우.
    public override void OnTargetDestroyed()
    {
        this.myTarget = null;
        if(CurrentStateID == StateID.ATTACK)
        {
            SetTransition(Transition.AttackToSearch);
        }
    }

}
