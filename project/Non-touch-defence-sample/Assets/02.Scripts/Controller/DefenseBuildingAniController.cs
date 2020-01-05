using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public class DefenseBuildingAniController
public class DefenseBuildingAniController : tk2dAniController
{
    public Dictionary<AnimationType, string> animationNameList = new Dictionary<AnimationType, string>();

    public override void Init(EntityCategory category)
    {
        base.Init(category);
        this.animationNameList.Add(AnimationType.Attack, "lv" + currLevel.ToString() + "_attack");
    }

    public override void PlayAnimation(AnimationType animType, bool bLoop = true)
    {
        //this.animator_main.Stop();
        this.animator_main.Play(animationNameList[animType]);

        base.PlayAnimation(animType, bLoop);
    }

}
