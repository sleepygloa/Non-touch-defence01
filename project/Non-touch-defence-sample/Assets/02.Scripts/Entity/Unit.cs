using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : Entity
{
    //private tk2dSprite mySprite = null;
    private Sprite mySprite = null;
    private BoxCollider2D myCollider = null;
    private UnitController unitController = null;

    public override void CashingObject()
    {
        myTransform = transform;
        //mySprite = myTransform.Find("ani_sprite").GetComponent<Sprite>();
        //mySprite = myTransform.Find("ani_sprite").GetComponent<tk2dSprite>();
        myCollider = gameObject.GetComponent<BoxCollider2D>();
        if (myCollider == null)
        {
            myCollider = gameObject.AddComponent<BoxCollider2D>();
        }
    }

    public override void InitEntity(EntityModel entity)
    {
        this.category = entity.entCategory;
        this.entityType = entity.enType;
        this.Level = entity.Level;
        this.HP = this.MaxHP = entity.HP;
        this.SearchRange = entity.SearchRange;
        this.AttackPower = entity.AttackPower;
        this.AttackSpeed = entity.AttackSpeed;
        if (this.unitController == null)
        {
            this.unitController = gameObject.GetComponent<UnitController>();
            if(this.unitController == null)
            {
                this.unitController = gameObject.AddComponent<UnitController>();
            }
        }
        this.unitController.Init(this);
    }

    public override void UpdateEntity()
    {
        if(this.unitController != null)
        {
            this.unitController.UpdateController();
        }
    }

    public override void DestroyEntity()
    {
        this.unitController.OnDestoryEntity();
        Destroy(gameObject);
    }

}
