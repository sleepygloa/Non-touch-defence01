using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitState_IDLE : FSMState
{
    UnitController Owner;

    public UnitState_IDLE(UnitController unit)
    {
        base.controller = unit;
        this.Owner = unit;
        stateID = StateID.IDLE;
    }

    public override void DoBeforeEntering()
    {
        this.Owner.aILerp.canMove = false;
    }
    public override void DoCheck()
    {
        if(this.changeState == true)
        {
            this.Owner.SetTransition(Transition.IdleToSearch);
        }
    }
    public override void DoAct()
    {
        if(this.changeState == false)
        {
            this.changeState = true;
        }
    }
    public override void DoBeforeLeaving()
    {
        this.changeState = false;
    }
}

public class UnitState_TargetFind : FSMState
{
    UnitController Owner;
    bool finishedSearchPath = false;
    public UnitState_TargetFind(UnitController unit)
    {
        base.controller = unit;
        this.Owner = unit;
        stateID = StateID.SEARCH;
    }

    public override void DoBeforeEntering()
    {
        this.Owner.aniController.PlayAnimation(AnimationType.Walk);
        this.isSearchCompleted = false;
        this.finishedSearchPath = false;
        if(this.Owner.aILerp != null)
        {
            this.Owner.aILerp.canMove = false;
        }
        this.Owner.myTarget = null;
    }
    public override void DoCheck()
    {
        if(this.isSearchCompleted == true)
        {
            this.Owner.SetTransition(Transition.SearchToWalk);
        }
    }
    public override void DoAct()
    {
        if(this.finishedSearchPath == false)
        {
            Entity target = null;
            target = Owner.TargetFindandPathSearch(EntityType.Defense);
            if(target != null)
            {
                this.finishedSearchPath = true;
            }
        }
    }
    public override void DoBeforeLeaving()
    {
        this.isSearchCompleted = false;
        this.finishedSearchPath = false;
    }
}

public class UnitState_Walk : FSMState
{
    UnitController Owner;
    Entity targetEntity;
    Vector3 currDirection;

    public UnitState_Walk(UnitController unit)
    {
        this.Owner = unit;
        base.controller = unit;
        stateID = StateID.WALK;
    }

    public override void DoBeforeEntering()
    {
        if(Owner.myTarget != null)
        {
            //방향 전환.
            Vector3 targetDir = (Owner.myTarget.position - Owner.myTransform.position).normalized;
            Owner.DirectionChange(targetDir);
            //타겟 가져오기
            if(Owner.myTarget.GetComponent<Entity>() != null)
            {
                this.targetEntity = Owner.myTarget.GetComponent<Entity>();
            }
            //걷는 애니메이션 재생.
            Owner.aniController.PlayAnimation(AnimationType.Walk);
        }
    }
    public override void DoCheck()
    {
        if(Owner.myTarget == null || this.targetEntity != null && this.targetEntity.IsDead() == true)
        {
            //새로운 타겟을 찾습니다.
            Owner.SetTransition(Transition.WalkToSearch);
            return;
        }
        //공격 가능 거리 안에 들어왔다면 공격 상태로 전환. 
        float distance = Vector3.Distance(Owner.myTarget.position, Owner.myTransform.position);
        if (distance < Owner.OwnerEntity.SearchRange * Define.GridDiagonal)
        {
            Owner.SetTransition(Transition.WalkToAttack);
        }

    }

    public override void DoAct()
    {
        //방향이 바뀌었다면.
        if(Owner.aILerp.AIdirection != this.currDirection)
        {
            Owner.DirectionChange(Owner.aILerp.AIdirection);
            Owner.aniController.PlayAnimation(AnimationType.Walk);
            this.currDirection = Owner.aILerp.AIdirection;
        }
    }
    public override void DoBeforeLeaving()
    {
        this.Owner.aILerp.canMove = false;
        //공격전에 타겟을 바라보도록 세팅.
        if(this.Owner.myTarget != null)
        {
            Vector3 targetDir = (Owner.myTarget.position - Owner.myTransform.position).normalized;
            Owner.DirectionChange(targetDir);
            Owner.aniController.PlayAnimation(AnimationType.Walk);
        }
    }

}

public class UnitState_Attack : FSMState
{
    UnitController Owner;
    Timer attackCoolTime;
    Entity TargetEntity = null;

    public UnitState_Attack(UnitController unit)
    {
        this.Owner = unit;
        base.controller = unit;
        stateID = StateID.ATTACK;
    }

    public void AttackCallback()
    {
        if(Owner.OwnerEntity.IsDead() == false)
        {
            Owner.aniController.PlayAnimation(AnimationType.Attack, false);
            if(this.TargetEntity != null)
            {
                BattleManager.Instance.AttackEntity(Owner.OwnerEntity, TargetEntity,
                    Owner.OwnerEntity.AttackPower, Owner.myTarget.position);
            }
        }
        else
        {
            TimeManager.Instance.RemoveTimer(attackCoolTime.ID);
        }
    }


    public override void DoBeforeEntering()
    {
        if(this.Owner.myTarget != null)
        {
            this.TargetEntity = this.Owner.myTarget.GetComponent<Entity>();
            this.Owner.aniController.PlayAnimation(AnimationType.Attack);
            this.Owner.aILerp.canMove = false;
            if(this.attackCoolTime == null)
            {
                this.attackCoolTime = new Timer();
            }
            this.attackCoolTime.Repeat(Owner.OwnerEntity.AttackSpeed, AttackCallback, Owner.OwnerEntity.FirstAttackDelay);
            TimeManager.Instance.AddTimer(this.attackCoolTime);
        }
    }
    public override void DoCheck()
    {
        if(this.Owner.myTarget == null || this.TargetEntity == null ||
            this.TargetEntity != null && this.TargetEntity.IsDead() == true)
        {
            Owner.SetTransition(Transition.AttackToSearch);
        }
    }
    public override void DoAct()
    {
        
    }
    public override void DoBeforeLeaving()
    {
        if(attackCoolTime != null)
        {
            TimeManager.Instance.RemoveTimer(attackCoolTime.ID);
        }
        this.TargetEntity = null;
    }
}

