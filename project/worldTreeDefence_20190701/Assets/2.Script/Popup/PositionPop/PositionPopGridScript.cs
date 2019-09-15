using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionPopGridScript : MonoBehaviour
{

    //선택된 전략 이미지
    public UISprite positionMap;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    //전략들을 클릭시 전략이미지에 저장하고 보여준다.
    public void onClick() {
        Debug.Log(this);
        Debug.Log(this.name);
    }




}
