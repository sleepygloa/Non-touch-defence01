using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroReadySciprt : MonoBehaviour
{


    public Image clickHeroReadyImg;
    public Image clickHeroReadyImgClicked;
    private bool clickFlag = false;

    private void Start()
    {
        clickHeroReadyImgClicked.enabled = false;
    }

    //히어로 전투 대기 클릭 이벤트
    public void clickHeroReady()
    {
        if (!clickFlag) {
            clickFlag = true;
            clickHeroReadyImg.enabled = false;
            clickHeroReadyImgClicked.enabled = true;
        }
        else {
            clickFlag = false;
            clickHeroReadyImg.enabled = true;
            clickHeroReadyImgClicked.enabled = false;
        }
    }
}
