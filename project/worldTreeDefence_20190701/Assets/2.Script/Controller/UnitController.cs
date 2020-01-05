using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitController : BaseController
{
    public Transform myTarget = null;
    public AILerp aILerp = null;
    public UnitAniController aniController = null;
    public Direction8Way myDirection = Direction8Way.e;

    public override void Init(Entity owner)
    {
        base.Init(owner);
        this.aniController = GetComponent<UnitAniController>();
        if(this.aniController == null)
        {
            this.aniController = gameObject.AddComponent<UnitAniController>();
        }
        this.aniController.Init(this.OwnerEntity.category);

        this.aILerp = GetComponent<AILerp>();
        if(this.aILerp == null)
        {
            this.aILerp = gameObject.AddComponent<AILerp>();
        }
        this.aILerp.unitController = this;

        FSMSetup();
    }

    public override Entity TargetFindandPathSearch(EntityType entityType)
    {
        float searchRange = 1000000.0f;
        Building targetBuilding = null;
        Transform target = EntityManager.Instance.GetCloseEntity(myTransform.position, searchRange, EntityType.Defense);
        if(target != null)
        {
            this.myTarget = target;
            targetBuilding = this.myTarget.GetComponent<Building>();
            if(targetBuilding != null)
            {
                Vector3 targetPos = targetBuilding.GetTargetPos(myTarget.position, myTransform.position);
                if(this.aILerp != null)
                {
                    this.aILerp.SearchPath(targetPos);
                }
            }
        }
        return targetBuilding;
    }

    public bool DirectionChange(Vector3 Dir)
    {
        Direction8Way newDir = Direction8Way.e;
        if(Mathf.Abs(Dir.x) > 0.2f)
        {
            if(Mathf.Abs(Dir.y) > 0.2f)
            {
                if(Dir.x > 0.0f)
                {
                    if(Dir.y > 0.0f)
                    {
                        newDir = Direction8Way.ne;
                    }else
                    {
                        newDir = Direction8Way.se;
                    }
                }else
                {
                    if(Dir.y > 0.0f)
                    {
                        newDir = Direction8Way.nw;
                    }else
                    {
                        newDir = Direction8Way.sw;
                    }
                }
            }
            else
            {
                if(Dir.x > 0.0f)
                {
                    newDir = Direction8Way.e;
                }else
                {
                    newDir = Direction8Way.w;
                }
            }
        }
        else
        {
            if(Mathf.Abs(Dir.y) > 0.2f)
            {
                if(Dir.y > 0.0f)
                {
                    newDir = Direction8Way.n;
                }else
                {
                    newDir = Direction8Way.s;
                }
            }else
            {
                if(Dir.x > 0.0f)
                {
                    if(Dir.y > 0.0f)
                    {
                        newDir = Direction8Way.ne;
                    }
                    else
                    {
                        newDir = Direction8Way.se;
                    }
                }else
                {
                    if(Dir.y > 0.0f)
                    {
                        newDir = Direction8Way.nw;
                    }else
                    {
                        newDir = Direction8Way.sw;
                    }
                }
            }
        }
        bool retValue = false;
        if(this.myDirection != newDir)
        {
            retValue = true;
        }
        this.myDirection = newDir;
        this.aniController.ChangeDirection(this.myDirection);

        return retValue;
    }


    public override void FSMSetup()
    {
        this.myTarget = null;

        UnitState_IDLE state_IDLE = new UnitState_IDLE(this);
        state_IDLE.AddTransition(Transition.IdleToSearch, StateID.SEARCH);

        UnitState_TargetFind state_TargetFind = new UnitState_TargetFind(this);
        state_TargetFind.AddTransition(Transition.SearchToWalk, StateID.WALK);
        state_TargetFind.AddTransition(Transition.SearchToIdle, StateID.IDLE);

        UnitState_Walk state_Walk = new UnitState_Walk(this);
        state_Walk.AddTransition(Transition.WalkToAttack, StateID.ATTACK);
        state_Walk.AddTransition(Transition.WalkToSearch, StateID.SEARCH);
        state_Walk.AddTransition(Transition.WalkToIdle, StateID.IDLE);

        UnitState_Attack state_Attack = new UnitState_Attack(this);
        state_Attack.AddTransition(Transition.AttackToSearch, StateID.SEARCH);
        state_Attack.AddTransition(Transition.AttackToIdle, StateID.IDLE);

        this.Fsm = new FSMSystem();
        this.Fsm.CreateStates();
        this.Fsm.AddState(state_IDLE);
        this.Fsm.AddState(state_TargetFind);
        this.Fsm.AddState(state_Walk);
        this.Fsm.AddState(state_Attack);
    }

    private StateID LastStateID = StateID.NULLSTATEID;
    public override void UpdateController()
    {
        
        if(this.Fsm != null && this.Fsm.CurrentState != null)
        {
            if(LastStateID != CurrentStateID)
            {
                Debug.Log("유닛 상태 변화 : " + LastStateID.ToString() + " 에서 " + CurrentStateID.ToString());
                LastStateID = CurrentStateID;
            }
            this.Fsm.CurrentState.DoCheck();
            this.Fsm.CurrentState.DoAct();
        }
        if(this.aniController != null)
        {
            this.aniController.UpdateAnimation();
        }
        //Tip Y축 위치에따라 소팅.
        //this.aniController.sprite_main.SortingOrder = 10000 - (int)this.myTransform.position.y;
    }

    public override void OnTargetFind()
    {
        if(CurrentStateID != StateID.SEARCH)
        {
            return;
        }
        if(Fsm.CurrentState is UnitState_TargetFind)
        {
            UnitState_TargetFind state = (UnitState_TargetFind)Fsm.CurrentState;
            state.isSearchCompleted = true;
        }
        this.aILerp.canMove = true;
    }

    public override void OnDestoryEntity()
    {
        this.myTarget = null;
        if (CurrentStateID == StateID.SEARCH)
        {
            SetTransition(Transition.SearchToIdle);
        }
        else if (CurrentStateID == StateID.ATTACK)
        {
            SetTransition(Transition.AttackToIdle);
        }
    }
    public override void OnTargetDestroyed()
    {
        this.myTarget = null;
        if (CurrentStateID == StateID.ATTACK)
        {
            SetTransition(Transition.AttackToSearch);
        }
    }

}
