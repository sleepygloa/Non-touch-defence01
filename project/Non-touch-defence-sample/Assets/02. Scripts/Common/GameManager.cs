using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{

    public int Level; //스테이지 레벨 
    public int Gold; //플레이어의 골드 소지량
    public int Diamond; //플레이어의 다이아몬(캐시) 소지량 

    private int treeHp; //세계수 HP
    private int treeMaxHp; //세계수 HP 최대치

    private int enermyNum = 5;

    //세계수 정보 컨트롤
    public int treeLevel = 1;
    public int treeUpgradeNeedGold = 1000;


    //마을 정보 컨트롤
    public int cityLevel = 1;

    public ArrayList playerList;

    [SerializeField]
    public ObjectPool Pool {
        get; set;
    }


    //public static GameManager instance;

    private void Awake()
    {
        //instance = this;

        Pool = GetComponent<ObjectPool>();
    }

    //// Start is called before the first frame update
    //void Start()
    //{

    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    //Pool = GetComponent<ObjectPool>();
    //}


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
