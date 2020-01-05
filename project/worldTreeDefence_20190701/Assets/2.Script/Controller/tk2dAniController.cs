//For Lecture

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tk2dAniController : MonoBehaviour
{

    [HideInInspector]
    public Animator animator_main; //애니메이션을 재생할 애니메이터

    public Sprite sprite_main;

    protected string[] directionName; //방향 스트링 값 

    [HideInInspector]
    public int currDirection; //현재 방향
    [HideInInspector]
    public int currLevel = 1;


    protected Vector3 originScale = Vector3.zero; //초기 scale
    protected Vector3 flipScale = Vector3.zero; //좌우반전을 위한 scale(x스케일에 -1을 곱한다)
    //unit
    public Direction8Way direction8Type = Direction8Way.se;

    //building
    public Direction16Way direction16Type = Direction16Way.se;
    bool isSkip = false;
    public int sortLayer = 0; //레이어 

    public virtual void Init(EntityCategory category)
    {
        Transform[] childs = transform.GetComponentsInChildren<Transform>();
        for (int i = 0; i < childs.Length; i++)
        {
            if (childs[i].name == "sprite")
            {
                sprite_main = childs[i].GetComponent<Sprite>();
            }
            else if (childs[i].name == "ani_sprite")
            {
                animator_main = childs[i].GetComponent<Animator>();
                if (animator_main == null)
                {
                    animator_main = childs[i].gameObject.AddComponent<Animator>();
                }
            }
        }
        if (sprite_main == null && animator_main != null)
        {
            sprite_main = animator_main.GetComponent<Sprite>();
        }

    }

    /// <summary>
    /// 레이어세팅
    /// </summary>
    /// <param name="layer">소팅레이어</param>
    public virtual void SetLayer(int layer)
    {

    }

    public virtual void UpdateAnimation()
    {
        if (isSkip == true)
        {
            return;
        }
        if (animator_main == null)
        {
            Debug.LogWarning(gameObject.name + "의 animator_main이 null입니다.", gameObject); isSkip = true;
            return;
        }

        //animator_main.UpdateAnimatsion(Time.deltaTime);
    }

    /// <summary>
    /// 방향 변경
    /// </summary>
    /// <param name="type">방향</param>
    public virtual bool ChangeDirection(Direction8Way type)
    {
        if ((int)type < 0 || (int)type > 7)
        {
            Debug.LogError("ChangeDirection Error:" + type.ToString());
        }

        bool retValue = false;
        if (this.direction8Type != type)
        {
            retValue = true; //바꼈다.
        }

        this.direction8Type = type;
        if (this.direction8Type == Direction8Way.nw || this.direction8Type == Direction8Way.sw
            || this.direction8Type == Direction8Way.w)
        {
           // this.sprite_main.FlipX = true;
        }
        else
        {
            //this.sprite_main.FlipX = false;
        }
        currDirection = (int)type;
        return retValue;
    }

    /// <summary>
    /// 16방향애니메이션 처리.
    /// </summary>
    /// <param name="type"></param>
    public virtual bool ChangeDirection16Way(Direction16Way type)
    {
        if ((int)type < 0 || (int)type > 15)
        {
            Debug.LogError("ChangeDirection Error:" + type.ToString());
        }
        bool retValue = false;
        if (this.direction16Type != type)
        {
            retValue = true; //바꼈다.
        }
        this.direction16Type = type;
        currDirection = (int)type;
        return retValue;
    }

    /// <summary>
    /// 애니메이션 재생 - [현재애니메이션타입][현재방향]으로 구성된 문자열을 가지고 tk2d 애니메이터에서 클립을 재생한다.
    /// </summary>
    public virtual void PlayAnimation(AnimationType animType, bool bLoop = true)
    {
        if (bLoop)
        {
            //animator_main.CurrentClip.wrapMode = tk2dSpriteAnimationClip.WrapMode.Loop;
        }
        else
        {
           // animator_main.CurrentClip.wrapMode = tk2dSpriteAnimationClip.WrapMode.Once;
        }
    }
}
