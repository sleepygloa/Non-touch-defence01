using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroPopManager : MonoBehaviour
{

    GameObject gm = null;

    UISprite heroImage = null;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);

        //gm = GameObject.Find("GameManager");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Open() {
        Debug.Log("click");
        //gameObject.SetActive(true);
    }
    public void Close() {
        gameObject.SetActive(false);
    }

    //영웅리스트를 클릭 했을 때 정보를 불러온다.

    public void HeroOnClick(string heroName, UISprite image) {

        //클릭시 키 확인.
        Debug.Log(heroName, image);

        //GameManger Script를 불러옴.
        GameManager gameManagerScript = gm.GetComponent<GameManager>();

        //영웅 정보를 가져와 현재 스크립트의 영웅 정보로 저장.
        Dictionary<string, string> heroInfo = gameManagerScript.playInfo[heroName];
        Debug.Log(heroInfo["Level"]);

        //영웅의 이미지를 큰 이미지로 저장.
        //영웅 큰 이미지 오브젝트를 찾음.
        GameObject heroImageSprite = GameObject.Find("HeroImageSprite");
        //클릭시 가져오는 UISprite 를 이용해. 큰 이미지에 저장.
        UISprite ui = heroImageSprite.GetComponents<UISprite>()[0];
        ui.spriteName = image.GetAtlasSprite().name;

        //캐릭터 상세 속성 세팅
        //UI 파일이름과, GameManaer 기본 속성 이름과 아래 하드코딩 명이 같아야함.
        GameObject.Find("HeroInfo" + "Name").GetComponent<UILabel>().text = heroInfo["Name"];

        GameObject.Find("HeroInfo" + "Hp").GetComponent<UILabel>().text = heroInfo["Hp"];
        GameObject.Find("HeroInfo" + "Attack").GetComponent<UILabel>().text = heroInfo["Attack"];
        GameObject.Find("HeroInfo" + "Def").GetComponent<UILabel>().text = heroInfo["Def"];







    }
}
