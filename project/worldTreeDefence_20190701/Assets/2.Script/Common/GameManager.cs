using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    //게스트의 아이디를 기본으로 설정
    private string playerId = "newbi";
    private string facebookId = "newbi";
    private string googleId = "";
    //이후 게임 설정이나 로그인시 아이디를 입력하면 그 DB 에서 확인하여 그정보를 불러온다

    //게임에 진행에 관한 정보를 세팅
    private int chapter = 1; //현재 스테이지 
    private int Level = 1; //스테이지 레벨 
    private int Gold = 0; //플레이어의 골드 소지량
    private int Diamond = 0; //플레이어의 다이아몬(캐시) 소지량 
    //게임 시작시 데이터베이스에서 저장된 정보를 불러온다 

    //캐릭터 정보를 2차원 배열로 저장
    private Dictionary<string, Dictionary<string, string>> playInfo;
    private string[] strArray = { "swordman", "thief", "mage", "acolight" };

    //세계수의 정보를 2차원 배열로 저장
    private Dictionary<string, string> treeInfo;

    //플레이어 위치 정보
    // "" 이면, 캐릭터가 없는 것 
    string playPosition1 = "";
    string playPosition2 = "";
    string playPosition3 = "";
    string playPosition4 = "";
    string playPosition5 = "";


    private int enermyNum = 5;
    //마을 정보 컨트롤
    public int cityLevel = 1;

    public ArrayList playerList;

    [SerializeField]
    public ObjectPool Pool {
        get; set;
    }



    //캐릭터 
    public GameObject heroSelectListPrefab;



    //public static GameManager instance;

    private void Awake()
    {
        //instance = this;

        Pool = GetComponent<ObjectPool>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //유닛 기본 세팅 
        infoSetting();


        //영웅 선택 리스트 창 세팅
        for (int i = 0; i < strArray.Length; i++)
        {
            string str = strArray[i];
            Debug.Log(str);
            //playInfo[str];
            //ht["swordman"];

            //추가할 오브젝트 생성
            GameObject newHeroListObj = Instantiate(heroSelectListPrefab);
            //Sprite newHeroLxistObjImg =  newHeroListObj.GetComponentInChildren<Sprite>();



            //게임 오브젝트 위치 변경 
            GameObject parent = GameObject.Find("HeroScrollViewList");
            //Debug.Log(parent);

            newHeroListObj.transform.parent = parent.transform;

            //위치와 스케일 초기화
            newHeroListObj.transform.localPosition = new Vector3(0, 120 - ( 50 * i), 0); ;
            newHeroListObj.transform.localScale = Vector3.one;

            //게임 오브젝트 내용 추가
            //newHeroListObj.GetComponentInChildren<Text>().text = str;



            UILabel newHeroListObjTxt = newHeroListObj.GetComponentInChildren<UILabel>();
            //Debug.Log(newHeroListObjTxt);
            newHeroListObjTxt.text = str;
            //Debug.Log(newHeroListObjTxt);




        }
    }

    private void infoSetting() {

        playInfo = new Dictionary<string, Dictionary<string, string>>();

        //플레이어 기본정보 세팅
        for (int i = 0; i < strArray.Length; i++)
        {
            Debug.Log("=== GameManager ... Hero Info Settings.... ===");
            string str = strArray[i];


            Dictionary<string, string> newHt;
    
            newHt = new Dictionary<string, string>();

            newHt.Add("name", str);//이름, 이름이 곧 직업
            newHt.Add("level", "1");
            newHt.Add("exp", "0");
            newHt.Add("cost", "1000");
            newHt.Add("hp", "10");
            newHt.Add("mp", "5");
            newHt.Add("speed", "3"); //1.매우 느림 2. 느림 3보통 4 빠름 5매우빠름

            newHt.Add("attack", "1");
            newHt.Add("defence", "1");
            newHt.Add("attackCoolTime", "2");
            newHt.Add("range", "1750");

            newHt.Add("skill1Yn", "N");
            newHt.Add("skill1Attack", "5");
            newHt.Add("skiil1CollTime", "10");

            newHt.Add("skill2Yn", "N");
            newHt.Add("skill2Attack", "15");
            newHt.Add("skiil2CollTime", "25");

            //특징
            newHt.Add("attackType", "1"); //1 단거리 2 중거리 3 장거리
            //newHt.Add("cost", "1000");
            //newHt.Add("cost", "1000");
            //newHt.Add("cost", "1000");
            //newHt.Add("cost", "1000");
            //newHt.Add("cost", "1000");
            //newHt.Add("cost", "1000");
            //newHt.Add("cost", "1000");
            //newHt.Add("cost", "1000");
            //newHt.Add("cost", "1000");

            playInfo.Add(str, newHt);
        }

        treeInfo = new Dictionary<string, string>();

        //세계수 정보 세팅
        treeInfo.Add("level", "1");
        treeInfo.Add("hp", "20");
        treeInfo.Add("mp", "20");
        treeInfo.Add("cost", "10000");


    }

    public void StartWave()
    {
        StartCoroutine(SpawnWave());
    }

    private IEnumerator SpawnWave()
    {
        Debug.Log("StartWave");

        //스테이지에서 스폰할 몬스터 개체 수 
        for(int i = 0; i < enermyNum; i++) {
            int monsterIndex = Random.Range(0, 4);

            string type = string.Empty;

            switch (monsterIndex)
            {
                case 0:
                    type = "slime";
                    break;
                case 1:
                    type = "slime";
                    break;
                case 2:
                    type = "slime";
                    break;
                case 3:
                    type = "slime";
                    break;
            }

            Pool.GetObject(type);
        }

        Debug.Log("End Wave");
        yield return new WaitForSeconds(2.5f);
    }

    //영웅 정보창 보이기
    public Canvas heroPickCanvas;
    private bool heroPickCanvasFlag = false;
    public void viewHeroPick() 
    {
        if (!heroPickCanvasFlag) {
            heroPickCanvas.transform.position = new Vector3(heroPickCanvas.transform.position.x, heroPickCanvas.transform.position.y, -5);
            heroPickCanvasFlag = true;
        }
        else {
            heroPickCanvas.transform.position = new Vector3(heroPickCanvas.transform.position.x, heroPickCanvas.transform.position.y, -11);
            heroPickCanvasFlag = false;
        }
    }




}
