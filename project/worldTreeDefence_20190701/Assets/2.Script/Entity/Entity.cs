using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public int UID;
    public int ID;
    public EntityCategory category;
    public EntityType entityType;
    public int HP
    {
        get
        {
            return hp;
        }
        set
        {
            SetHP(value);
        }
    }
    private int hp;
    public int MaxHP;
    public int Level;
    public int AttackPower;
    public float SearchRange;
    public string Prefab;
    public float AttackSpeed;
    public float MoveSpeed;
    public float FirstAttackDelay = 0.1f;
    public Transform myTransform;

    public virtual void InitEntity(EntityModel entity)
    {

    }

    public virtual void CashingObject()
    {
        myTransform = transform;
    }

    public virtual void UpdateEntity()
    {

    }

    public virtual void DestroyEntity()
    {

    }

    public virtual void OnTargetDestroy()
    {

    }

    public virtual void OnDamaged(int damage)
    {

    }

    public bool IsDead()
    {
        if (this.HP > 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public virtual int SetHP(int newHP)
    {
        this.hp = newHP;
        return hp;
    }

}
