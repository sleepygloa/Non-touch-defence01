using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMoveScript : CharacterAttributeScript
{
    //이동속도
    public float movePower = 1f;

    //점프속도
    public float jumpPower = 1f;

    //점프 유무 
    bool isJumping = false;

    //플레이어 RIGID BODY
    Rigidbody2D rigid;

    //플레이어 위치
    Vector3 movement;

    //플레이어 애니메이션
    Animator animator;

    //플레이어 Collider
    private Collider2D collider;

    //공격 대상
    GameObject targetEnemy = null;
    //공격 대상 유무
    bool targetFlag = false;
    //적을 발견할 최대 거리
    public float findEnemyRange = 1750f;

    //충돌유무
    public bool isCollision = false;

    //발견한 적들 배열
    GameObject[] taggedEnemys = null;

    //유닛 캐릭터 정보
    CharacterAttributeScript enemyStatus;

    //적을 처음 찾은 시간과 두번째 찾은 시간 
    //float startFindTime = 0f;
    float endFindTime = 0f;

    //공격 시작시간과 경과시간
    private float startAttackTime = 0f;
    private float endAttackTime = 0f;

    //스킬1 사용시간과 경과시간
    private float startSkill1Time = 0f;
    private float endSkill1Time = 0f;
    //스킬2 사용시간과 경과시간 
    private float startSkill2Time = 0f;
    private float endSkill2Time = 0f;

    //피격 당한 시간과 경과 시간 
    private float startDamageTime = 0f;
    private float endDamageTime = 0f;
    private float statusDamageTime = 1f;

    //죽는 시간과 경과시간 
    private float startDieTime = 0f;
    private float endDieTime = 0f;
    private float statusDieTime = 3f;

    public Image imgHp;


    //이전위치
    public Vector2 prevPosition;
    //움직이는 속도
    public float speed;

    // Start is called before the first frame update
    void Start()
    {
        //캐릭터 Rigid body
        //rigid = gameObject.GetComponent<Rigidbody2D>();

        //캐릭터 collider
        //collider = gameObject.GetComponent<Collider2D>();

        //캐릭터 Animation
        //animator = gameObject.GetComponentInChildren<Animator>();

        //StartCoroutine("AI");

        StartCoroutine(idleMove());
    }

    // Update is called once per frame
    void Update()
    {

        //changeState();
    }

    private void changeState() {
        if (imgHp != null)
        {
            float playerHp = hp;
            imgHp.fillAmount = hp / maxHp;
        }


        //2-7. 캐릭터가 죽고있다면 
        if (isDie)
        {
            //죽었다면 죽는 시간 계산
            endDieTime += Time.deltaTime;

            //죽는시간이 변수를 초과할 경우 죽은상태로 애니메이션 변경
            if (endDieTime - startDieTime > statusDieTime)
            {
                animator.SetBool("isDie", true);

                //반복 루팅 제거 ***************
            }
            //죽고있다면, 아무일도 일어나지 않음 계속 죽고있다
            return;
        }

        //2-6. 캐릭터가 방금 죽었다면
        if (hp <= 0)
        {
            endDieTime += Time.deltaTime;
            animator.SetBool("isDie", true);
            isDie = true;

            return;
        }

        //2-5. 캐릭터가 피격 당했을때
        if (isDamage)
        {
            endDamageTime += Time.deltaTime;

            //피격 이후 변수 시간이 지나면 기본상태로 변환 
            if (endDamageTime - startDamageTime > statusDamageTime)
            {
                animator.SetBool("isDamage", false);
                fnStatusInit();
            }

            animator.SetBool("isDamage", true);
            //isDamage = true;

            return;
        }

        Debug.Log("======");

        if (!targetFlag)
        {
            //적을 찾아서 공격하는 모션 
            //적(태그로)을 모두 찾아 배열로 저장
            taggedEnemys = GameObject.FindGameObjectsWithTag("Enemy");

            int cnt = 0;
            //적의 수만큼 루프를 돔.
            foreach (GameObject taggedEnemy in taggedEnemys)
            {
                cnt++;
                Debug.Log("***** " + taggedEnemy.name + " *****");
                //찾은 적이 죽었다면 패스
                CharacterAttributeScript enermyCa = taggedEnemy.GetComponent<CharacterAttributeScript>();
                if (enermyCa.isDie)
                {
                    continue;
                }

                Debug.Log(taggedEnemy);
                //적의 위치
                Vector3 objectPos = taggedEnemy.transform.position;
                //적과 캐릭터의 거리 계산
                var dist = (objectPos - transform.position).sqrMagnitude;

                if ((int)dist > (int)findEnemyRange)
                {
                    cnt = 0;
                    Debug.Log("no enemy");
                    fnCharacterStatusInit();
                    continue;
                }
                else
                {
                    //적이 (캐릭터가 적을 발견할 거리변수)변수 안으로 들어왔을때
                    //발견
                    Debug.Log("find enemy");

                    //절 발견 플래그 변경
                    targetFlag = true;

                    //적 선택            
                    targetEnemy = taggedEnemy;
                    return;
                }

            }
            Debug.Log("target find" + targetFlag);
        }
        else
        {
            Debug.Log("target unfind" + targetFlag);
            if (targetEnemy != null)
            {
                //적의 상태창 조회
                enemyStatus = targetEnemy.GetComponent<CharacterAttributeScript>();

                //적을 찾은 시간
                endFindTime += Time.deltaTime;

                //1-1. 적이 죽었는지 파악, 죽었다면 처음부터(루프 종료) 
                if (enemyStatus.isDie)
                {
                    Debug.Log("die");
                    fnFindEnemyInit();
                    return;
                }

                //1-2. 적의 피가 0 이면 적을 다시 찾아야함 
                if (enemyStatus.hp <= 0)
                {
                    Debug.Log("0");
                    fnFindEnemyInit();
                    return;
                }

                //1-3. 적이 살아있다 !
                //1-3-3. 스킬2 을 배웠다면, 스킬이 쿨다운 상태인지 확인
                //배웠는지 확인
                if (learnSkiil2)
                {
                    //스킬 사용 중 아닐 때 
                    if (!isSkill2)
                    {
                        isSkill2 = true; //스킬사용상태로 변경
                        animator.SetBool("isSkill2", true); //스킬사용 애니메이션 변경
                    }

                    //스킬 사용 중 일 때
                    if (isSkill2)
                    {
                        endSkill2Time += Time.deltaTime; //스킬 사용 시간 체크
                        if (endSkill2Time - startSkill2Time > skill2CoolTime)
                        {
                            //스킬발동 

                            endSkill2Time = 0f;
                        }
                        if (endSkill2Time - startSkill2Time > skill2AfterDelay)
                        {
                            animator.SetBool("isStand", true);
                        }
                        return;
                    }
                }



                //1-3-3. 스킬1 을 배웠다면, 스킬이 쿨다운 상태인지 확인
                if (learnSkill1)
                {
                    if (!isSkill1)
                    {
                        isSkill1 = true;
                        animator.SetBool("isSkill1", true);
                    }


                    if (isSkill1)
                    {
                        endSkill1Time += Time.deltaTime;
                        if (endSkill1Time - startSkill1Time > skill1CoolTime)
                        {
                            //스킬발동 

                            endSkill1Time = 0f;
                        }
                        if (endSkill1Time - startSkill1Time > skill1AfterDelay)
                        {
                            animator.SetBool("isStand", true);
                        }
                        return;
                    }
                }


                //1-3-1. 캐릭터가 공격중이 아니라면 공격 시작
                if (!isAttack)
                {
                    isAttack = true;
                    animator.SetBool("isAttack", true);
                }

                //1-3-2. 캐릭터가 공격 중 
                if (isAttack)
                {

                    endAttackTime += Time.deltaTime;
                    //Debug.Log("000000");
                    //Debug.Log(endAttackTime);
                    //Debug.Log(attackCoolTime);
                    //Debug.Log("000000");

                    if (endAttackTime - startAttackTime < attackCoolTime)
                    {
                        return;
                    }

                    //피사체 유무
                    //피사체 유 : 피사체를 날리고 공격대상을 넘겨준다.
                    if (isProtectileFlag)
                    {
                        endAttackTime = 0f;
                        animator.SetBool("isAttack", true);
                        enemyStatus.setDamage(this.attack);
                    }
                    //피사체 무 : 바로 공격한다.
                    else
                    {
                        endAttackTime = 0f;
                        animator.SetBool("isAttack", true);
                        enemyStatus.setDamage(this.attack);
                    }
                }

            }


        }
    }


    //idle 상태에서 움직이기
    //Limit 에 맞춰서 움직임을 판단한다

    IEnumerator idleMove()
    {
        while (true)
        {
            
            float x = transform.position.x + Random.Range(-moveDistanceX, moveDistanceX);
            float y = transform.position.y + Random.Range(-moveDistanceY, moveDistanceY);

            Vector2 target = new Vector2(x, y);
            target = CheckTarget(target);

            prevPosition = transform.position;

            while (Vector2.Distance(transform.position, target) > 0.01f)
            {
                
                transform.position = Vector2.MoveTowards(transform.position, target, speed);
                yield return null;

            }
            yield return new WaitForSeconds(1f);
        }
    }

    float moveDistanceY = 0.05f;
    float moveDistanceX = 0.3f;

    Vector2 CheckTarget(Vector2 currentTarget)
    {
        Vector2 temp = currentTarget;

        //위치수정
        Debug.Log(currentTarget);
        
        Debug.Log(GameManager.gm);
        if (currentTarget.x < GameManager.gm.limitPoint1.x)
        {
            temp = new Vector2(currentTarget.x, temp.y);
        }
        else if (currentTarget.y > GameManager.gm.limitPoint2.x)
        {
            temp = new Vector2(currentTarget.x, temp.y);
        }

        if (currentTarget.y > GameManager.gm.limitPoint1.y)
        {
            temp = new Vector2(temp.x, currentTarget.y);
        }
        else if (currentTarget.y < GameManager.gm.limitPoint2.y)
        {
            temp = new Vector2(temp.x, currentTarget.y);
        }

        return temp;
    }













    //적찾는 행동 초기화
    private void fnFindEnemyInit() {
        targetFlag = false;
        taggedEnemys = null;
        targetEnemy = null;
        endFindTime = 0f;
        fnAniamtorInit();
    }

    //상태만 초기화
    private void fnStatusInit() {
        isAttack = false;
        isMoving = true;
        isDamage = false;
        isDie = false;
        isSkill1 = false;
        isSkill2 = false;
    }

    //스테이지시작시 전체 초기화
    private void fnCharacterStatusInit() {
        //공격 대상
        targetEnemy = null;
        //공격 대상 유무
        targetFlag = false;
        //적을 발견할 최대 거리

    
        endFindTime = 0f;
        endAttackTime = 0f;
        endSkill1Time = 0f;
        endSkill2Time = 0f;
        endDamageTime = 0f;
        endDieTime = 0f;

        isAttack = false;
        isMoving = true;
        isDamage = false;
        isDie = false;
        isSkill1 = false;
        isSkill2 = false;
}

    private void fnAniamtorInit() {
        animator.SetBool("isAttack", false);
        animator.SetBool("isMoving", true);
        animator.SetBool("isSkill1", false);
        animator.SetBool("isSkill2", false);
        animator.SetBool("isDie", false);
        animator.SetBool("isDamage", false);
    }


    private void FixedUpdate()
    {

        //AI();
        //Move();
        //Jump();
    }

    //IEnumerator AI() {
    //    animator.SetBool("isMoving", true);



    //    //적을 찾는 
    //    //enemyObject.Find
    //    var enemy = GameObject.FindGameObjectsWithTag("Enemy");

    //    if(enemy.Length != 0) {
    //        animator.SetBool("isMoving", false);
    //        animator.SetBool("isAttack", true);
    //    }
    //    else {
    //        animator.SetBool("isMoving", true);
    //        animator.SetBool("isAttack", false);
    //    }
    //    //enemyObject.FindGameObjectsWithTag("Enemy");

    //    //yield return new WaitForSeconds(2f);
    //    yield return new WaitForSeconds(0f);

    //    StartCoroutine("AI");

    //}

    //void Move() {
    //    Vector3 moveVelocity = Vector3.zero;

    //    if(Input.GetAxisRaw("Horizontal") < 0) {
    //        moveVelocity = Vector3.left;

    //        transform.localScale = new Vector3(-1, 1, 1);
    //    }
    //    else if(Input.GetAxisRaw("Horizontal") > 0) { 
    //        moveVelocity = Vector3.right;

    //        transform.localScale = new Vector3(1, 1, 1);
    //    }

    //    transform.position += moveVelocity * movePower * Time.deltaTime;

    //}

    //void Jump() {
    //    if (!isJumping) return;

    //    rigid.velocity = Vector2.zero;

    //    Vector2 jumpVelocity = new Vector2(0, jumpPower);
    //    rigid.AddForce(jumpVelocity, ForceMode2D.Impulse);

    //    isJumping = false;
    //}


    private void OnCollisionEnter2D(Collision2D collision)
    {
        //GetComponent<Rigidbody2D>().isKinematic = true;
        /*        if (collision.gameObject.tag == "Enemy")
                {
                    Debug.Log("collision player start");

                    //transform.position = targetTranform.position * movePower * Time.deltaTime;
                }


                isCollision = true;

                Debug.Log("collision start");*/
        if (collision.collider.CompareTag("Player") == true)
        {
            Collider2D col1 = this.gameObject.GetComponent<Collider2D>();
            Collider2D col2 = collision.collider;
            Physics2D.IgnoreCollision(col1, col2);

        }
    }


}
