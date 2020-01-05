using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BuildingState_IDLE : FSMState
{
    BuildingController Owner;
    public BuildingState_IDLE(BuildingController owner)
    {
        this.Owner = owner;
        base.controller = owner;
        stateID = StateID.IDLE;
    }
    public override void DoBeforeEntering()
    {
        base.DoBeforeEntering();
    }
    public override void DoCheck()
    {
        if(this.changeState == true)
        {
            Owner.SetTransition(Transition.IdleToSearch);
        }
    }
    public override void DoAct()
    {
        if(Owner.gameObject.activeSelf == true)
        {
            this.changeState = true;
        }
    }
    public override void DoBeforeLeaving()
    {
        this.changeState = false;
    }
}

public class BuildingState_TargetFind : FSMState
{
    BuildingController Owner;
    Transform target;

    public BuildingState_TargetFind(BuildingController owner)
    {
        base.controller = owner;
        this.Owner = owner;
        stateID = StateID.SEARCH;
    }

    public override void DoBeforeEntering()
    {
        target = null;
    }
    public override void DoCheck()
    {
        if(this.changeState == true)
        {
            Owner.SetTransition(Transition.SearchToAttack);
        }
    }
    public override void DoAct()
    {
        float searchRange = Owner.OwnerEntity.SearchRange * Define.GridDiagonal;
        target = EntityManager.Instance.GetCloseEntity(Owner.myTransform.position, searchRange, EntityType.Unit);
        if(target != null)
        {
            this.changeState = true;
            Owner.myTarget = target;
        }
    }
    public override void DoBeforeLeaving()
    {
        this.changeState = false;
    }
}

public class BuildingState_Attack : FSMState
{
    BuildingController Owner;
    Entity target;
    float attackRange = 0.0f;
    Timer attackCoolTime = null;

    public BuildingState_Attack(BuildingController owner)
    {
        base.controller = owner;
        this.Owner = owner;
        stateID = StateID.ATTACK;
    }

    public void AttackCallback()
    {
        if(Owner.OwnerEntity.IsDead() == false && this.target != null && this.target.IsDead() == false)
        {
            Owner.aniController.PlayAnimation(AnimationType.Attack, false);
            BattleManager.Instance.AttackEntity(Owner.OwnerEntity, this.target,
                Owner.OwnerEntity.AttackPower, this.target.myTransform.position);

        }
    }

    public override void DoBeforeEntering()
    {
        this.attackRange = this.Owner.OwnerEntity.SearchRange * Define.GridDiagonal;
        if(Owner.myTarget != null)
        {
            this.target = Owner.myTarget.GetComponent<Entity>();
            if(attackCoolTime == null)
            {
                attackCoolTime = new Timer();
            }
            attackCoolTime.Repeat(Owner.OwnerEntity.AttackSpeed, AttackCallback, 0.1f);
            TimeManager.Instance.AddTimer(attackCoolTime);
        }
    }
    public override void DoCheck()
    {
        if(this.changeState == true)
        {
            Owner.SetTransition(Transition.AttackToSearch);
        }
    }
    public override void DoAct()
    {
        //타겟이 죽은경우.
        if(target != null && this.target.IsDead() == true)
        {
            this.changeState = true;
        }
        //타겟이 이동한 경우.
        float distance = Vector3.Distance(Owner.myTransform.position, this.target.myTransform.position);
        if(distance > this.attackRange)
        {
            this.changeState = true;
        }
    }
    public override void DoBeforeLeaving()
    {
        if(this.attackCoolTime != null)
        {
            TimeManager.Instance.RemoveTimer(this.attackCoolTime.ID);
            
        }
        this.changeState = false;
        this.target = null;
    }
}

