using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseController : MonoBehaviour
{
    protected FSMSystem Fsm;
    protected StateID LastStateID;

    public StateID CurrentStateID
    {
        get
        {
            if(Fsm == null)
            {
                return StateID.NULLSTATEID;
            }
            return Fsm.CurrentStateID;
        }
    }
    public Entity OwnerEntity = null;
    public Transform myTransform = null;

    public virtual void Init(Entity owner)
    {
        this.OwnerEntity = owner;
        myTransform = transform;
    }

    public virtual void FSMSetup()
    {

    }

    public virtual void SetTransition(Transition t)
    {
        Fsm.PerformTransition(t);
    }

    public virtual bool CheckStateID(StateID stateID)
    {
        return CurrentStateID == stateID;
    }

    public virtual void UpdateController()
    {

    }

    public virtual void OnTargetDestroyed()
    {

    }

    public virtual void OnDestoryEntity()
    {

    }

    public virtual void OnTargetFind()
    {

    }

    public virtual Entity TargetFindandPathSearch(EntityType entityType)
    {
        return null;
    }

}
