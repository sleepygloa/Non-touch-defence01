using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public class UnitAniController 
    public class UnitAniController : tk2dAniController
{

    public Dictionary<AnimationType, string[]> animationNameList = new Dictionary<AnimationType, string[]>();
    public AnimationType CurrentAnimation;

    public void Init(EntityCategory category)
    {
        base.Init(category);

        string[] walkList = new string[8];
        string[] attackList = new string[8];

        for(int i =  0; i < 8; i++)
        {
            //lv1_idle_e, lv1_idle_s,....
            walkList[i] = "lv" + currLevel.ToString() + "_idle_" + ((Direction8Way)i).ToString();
            attackList[i] = "lv" + currLevel.ToString() + "_attack_" + ((Direction8Way)i).ToString();

        }
        animationNameList.Add(AnimationType.Walk, walkList);
        animationNameList.Add(AnimationType.Attack, attackList);

        currDirection = (int)Direction8Way.se;

    }

    public  void PlayAnimation(AnimationType animType, bool bLoop = true)
    {
        if(animator_main == null)
        {
            return;
        }
        //animator_main.Stop();
        animator_main.Play(animationNameList[animType][currDirection]);
    
        base.PlayAnimation(animType, bLoop);
    }

}
