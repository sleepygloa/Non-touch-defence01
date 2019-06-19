using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMoveScript : CharacterAttributeScript
{

    private float destoryTime;

    public float movePower = 1f;
    private Transform targetTranform;
    private Transform myTranform;

    private Collider2D collider;
    private Rigidbody2D rigid;


    Animator animator;


    //애니메이션 플래그
    bool isAttack = false;
    bool isMoving = true;
    bool isDamage = false;
    bool idDie = false;

    [SerializeField]
    private CanvasGroup healthGroup;

    public override Transform Select() {
        healthGroup.alpha = 1;
        return base.Select();
    }

    public override void DeSelect()
    {
        healthGroup.alpha = 0;

        base.DeSelect();
    }

    // Start is called before the first frame update
    void Start()
    {
        rigid = gameObject.GetComponent<Rigidbody2D>();

        collider = gameObject.GetComponent<Collider2D>();

        animator = gameObject.GetComponentInChildren<Animator>();
    }


    void Update()
    {
        Vector3 velocity = Vector3.zero;
        velocity = new Vector3(-5f, 0, 0);

        //적 존재 확인
        var player = GameObject.FindGameObjectsWithTag("Player");

        if (player.Length != 0)
        {
            targetTranform = player[0].transform;

            ////내위치
            myTranform = transform;

            var distance = targetTranform.position - myTranform.position;

            transform.position += velocity * movePower * Time.deltaTime;
        }
        else
        {
            transform.position += velocity * movePower * Time.deltaTime;
        }

        if(hp <= 0) {
            animator.SetBool("isDie", true);

            animator.SetBool("isRemove", true);
            destoryTime += Time.deltaTime;

            if(destoryTime >= 5f) {
                Destroy(gameObject);
            }


        }
        else {
            animator.SetBool("isDie", false);
        }




    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player") {
            Debug.Log("collision enemy start");

            movePower = 0;
        }



        //isCollision = true;


    }

    private void OnCollisionStay(Collision collision)
    {
        //isCollision = true;

        Debug.Log("collision stay");
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        //isCollision = false;

        Debug.Log("collision end");
    }

}
