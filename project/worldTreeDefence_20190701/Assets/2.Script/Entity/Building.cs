using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : Entity
{
    //드래그용 
    public Vector3 originPos = Vector3.zero;
    public Vector2 lastPos = Vector2.zero;
    private List<Vector2> movePosList = new List<Vector2>();
    //캐싱.
    private Sprite mySprite = null;
    //private tk2dSprite mySprite = null;
    private BoxCollider2D myCollider = null;
    private BuildingController buildingController = null;

    public override void CashingObject()
    {
        myTransform = transform;
        mySprite = myTransform.Find("sprite").GetComponent<Sprite>();
        myCollider = GetComponent<BoxCollider2D>();
        if (myCollider == null)
        {
            myCollider = gameObject.AddComponent<BoxCollider2D>();
            myCollider.isTrigger = true;
            myCollider.size = new Vector2(128.0f, 128.0f);
        }
    }

    public override void InitEntity(EntityModel entity)
    {
        this.ID = entity.ID;
        this.category = entity.entCategory;
        this.entityType = entity.enType;
        this.Level = entity.Level;
        this.MaxHP = this.HP = entity.HP;
        this.SearchRange = entity.SearchRange;
        this.AttackPower = entity.AttackPower;
        this.AttackSpeed = entity.AttackSpeed;

        this.buildingController = GetComponent<BuildingController>();
        if (this.buildingController == null)
        {
            this.buildingController = gameObject.AddComponent<BuildingController>();
        }
        this.buildingController.Init(this);
    }

    private bool _isSelect = false;
    public bool IsSelect
    {
        get
        {
            return _isSelect;
        }
        set
        {
            if (_isSelect != value)
            {
                //선택되어 있지 않은 상황이라면.
                if (_isSelect == false)
                {
                    if (myCollider != null)
                    {
                        myCollider.enabled = false;
                    }
                }
                else
                {
                    if (myCollider != null)
                    {
                        myCollider.enabled = true;
                        //길찾기 경로의 재탐색.
                        AstarPath.active.Scan();
                    }
                }
            }
            _isSelect = value;
        }
    }

    void DragUpdate()
    {
        lastPos = myTransform.localPosition;

        InputManager.FingerInput currentInput = InputManager.Instance.GetCurrentInput();
        if (currentInput.currentRayHitTransform != null)
        {
            if (currentInput.currentRayHitTransform.gameObject.layer == LayerMask.NameToLayer("Grid"))
            {
                Vector2 currentRayHitTransformPosition = currentInput.currentRayHitTransform.position;
                if (lastPos != currentRayHitTransformPosition)
                {
                    if(movePosList.Contains(currentRayHitTransformPosition) == false)
                    {
                        movePosList.Add(currentRayHitTransformPosition);
                    }
                }
            }
        }
        if(movePosList.Count > 0)
        {
            myTransform.localPosition = new Vector3(movePosList[0].x, movePosList[0].y, -1.0f);
            movePosList.RemoveAt(0);
        }
    }

    public override void UpdateEntity()
    {
        //현재 선택되어 있는 상황이라면.
        if(IsSelect == true)
        {
            DragUpdate();
        }
        if(this.buildingController != null)
        {
            this.buildingController.UpdateController();
        }
    }

    public Vector3 GetTargetPos(Vector3 targetPos, Vector3 origin)
    {
        Vector3 retPosition = myTransform.position;

        Vector3[] aroundPosition = new Vector3[8];
        aroundPosition[0] = targetPos + new Vector3(-Define.GridWidth, 0.0f, 0.0f);
        aroundPosition[1] = targetPos + new Vector3(-Define.GridWidthHalf, Define.GridHeightHalf, 0.0f);
        aroundPosition[2] = targetPos + new Vector3(0.0f, Define.GridHeight, 0.0f);
        aroundPosition[3] = targetPos + new Vector3(Define.GridWidthHalf, Define.GridHeightHalf, 0.0f);
        aroundPosition[4] = targetPos + new Vector3(Define.GridWidth, 0.0f, 0.0f);
        aroundPosition[5] = targetPos + new Vector3(Define.GridWidthHalf, -Define.GridHeightHalf, 0.0f);
        aroundPosition[6] = targetPos + new Vector3(0.0f, -Define.GridHeight, 0.0f);
        aroundPosition[7] = targetPos + new Vector3(-Define.GridWidthHalf, -Define.GridHeightHalf, 0.0f);

        float minDistance = 10000f;
        for (int i = 0; i < aroundPosition.Length;i++)
        {
            float distance = Vector3.Distance(aroundPosition[i], origin);
            if(distance < minDistance)
            {
                minDistance = distance;
                retPosition = aroundPosition[i];
            }
        }
        return retPosition;
    }

    public override void OnDamaged(int damage)
    {
        SoundManager.Instance.PlayOneShot((int)SoundList.Hit);
    }

    public override void DestroyEntity()
    {
        SoundManager.Instance.PlayOneShot((int)SoundList.Explosion);
        Destroy(gameObject);
    }



}
