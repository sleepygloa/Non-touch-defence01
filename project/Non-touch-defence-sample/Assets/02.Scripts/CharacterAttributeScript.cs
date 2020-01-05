using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAttributeScript : MonoBehaviour
{


    public float maxHp = 10;
    public float maxMp = 5;
    public float hp = 10;
    public float mp = 5;
    public float stamina = 100;

    public float attack = 2;
    public float defence = 0;

    public float STR = 10;
    public float INT = 10;
    public float DEX = 10;
    public float VIT = 10;
    public float CHA = 10;

    public float attackCoolTime = 3f;
    public float attackRange = 1;
    public bool isProtectileFlag = false;
    public float skill1CoolTime = 10f;
    public float skill1AfterDelay = 1f;
    public float skill2CoolTime = 25f;
    public float skill2AfterDelay = 2f;


    //애니메이션 플래그
    public bool isAttack = false;
    public bool isMoving = true;
    public bool isDamage = false;
    public bool isDie = false;
    public bool isSkill1 = false;
    public bool isSkill2 = false;
    public bool learnSkill1 = false;
    public bool learnSkiil2 = false;

    // Start is called before the first frame update
    void Start()
    {
        hp = maxHp;
        mp = maxMp;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float setDamage(float damage) {
        this.hp -= damage;
        return this.hp;
    }



}
