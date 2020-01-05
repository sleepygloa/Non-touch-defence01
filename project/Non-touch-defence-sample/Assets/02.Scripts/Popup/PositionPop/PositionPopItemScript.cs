using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionPopItemScript : MonoBehaviour
{

    // 아이템 이름,
    private string itemName;
    // 아이콘 표시할 스프라이트 이름.
    private string spriteName;
    // 아이콘 이미지
    public UISprite spriteImg;
       
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 정보를 설정하는 함수 입니다.
    public void SetInfo(string spriteName)
    {
        // 같은 아틀라스에 있으니 스프라이트 이름 찾아 넣어주면 이미지가 바껴요.
        spriteImg.spriteName = spriteName;
        // 이름도 설정 합시다.(확인 위해 이름설정하는거)
        spriteName = spriteName;
    }

    // 터치 하면 발생하는 이벤트입니다.
    // 전에 Button을 썼지만 OnClick으로 사용하겠습니다.
    // OnClick은 NGUI에서 제공하는 함수로 터치하면 발생됩니다.
    void OnClick()
    {
        // 확인 위해 로그 찍어봅니다.
        Debug.Log(itemName + " 이 클릭되었습니다.");
    }

}
