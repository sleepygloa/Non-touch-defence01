using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherScript : MonoBehaviour
{

    public float rangeRadius; //아쳐가 발사 할 수 있는 최대 거리
    public float reloadTime; //다음 발사가 가능하기 까지 걸리는 시간
    public GameObject projectilePrefab; //빨사체 타입
    private float elapsedTime; //발사 직후 흐른 시간



    public int upgradeLevel; //레벨
    public Sprite[] upgradeSprites; //컵케이크 타워 레벨에 따른 스프라이트

    //업그레이드 가능 여부 체크
    public bool isUpgradable = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(elapsedTime >= reloadTime) {
            //흐른 시간을 리셋
            elapsedTime = 0;
            //콜라이더 범위안에 게임 오브젝트가 있는지 체크
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, rangeRadius);
            //적어도 하나 이상의 게임오브젝트가 있는지 확인
            if(hitColliders.Length != 0) {
                //모든 게임 오브젝트를 대상으로 컵케이크 타워에서 가장 가까운 곳에 잇는 적을 판별하는 루ㅡ를 돌린다
                float min = int.MaxValue;
                int index = -1;
                for(int i = 0; i < hitColliders.Length; i++) { 
                    if(hitColliders[i].tag == "Enemy") {
                        float distance = Vector2.Distance(hitColliders[i].transform.position, transform.position);
                        if(distance < min) {
                            index = i;
                            min = distance;
                        }
                    }
                }

                if(index != -1) {
                    return;
                }


                //타킷의 방향을 찾음
                Transform target = hitColliders[index].transform;
                //Vector2 direction = (target.position - transform.position).normalized;
                Vector2 direction = (transform.position).normalized;


                //발사체 생성
                GameObject projectTile = GameObject.Instantiate(projectilePrefab, transform.position, Quaternion.identity) as GameObject;
                projectTile.GetComponent<ProjectTileScript>().direction = direction;

            }
        }


        elapsedTime += Time.deltaTime;







        //레벨업 관련///////////////////////////
        //업그레이드 가능한지 확인
        if (!isUpgradable)
        {
            return;
        }

        //레벨업
        upgradeLevel++;

        //타워의 레벨이 최대치 인지 확인
        if (upgradeLevel < upgradeSprites.Length)
        {
            isUpgradable = false;
        }

        //스탯 업
        rangeRadius += 1f;
        reloadTime -= 0.5f;

        //캐릭터 그래픽 변경
        GetComponent<SpriteRenderer>().sprite = upgradeSprites[upgradeLevel];
    }
}
