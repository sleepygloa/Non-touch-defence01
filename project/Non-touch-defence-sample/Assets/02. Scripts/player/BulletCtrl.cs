using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCtrl : MonoBehaviour
{

    //총알의 파괴력
    public float damage = 20.0f;
    //총알 발사 속도
    public float speed = 1000.0f;

    //컴포넌트를 저장할 변수
    private Transform tr;
    private Rigidbody rb;
    private TrailRenderer trail;

    private void Awake()
    {
        //컴포넌트 할당
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        trail = GetComponent<TrailRenderer>();
        //불러온 데이터 값을 damage 에 적용
        damage = GameManager.instance.gameData.damage;
    }

    private void OnEnable()
    {
        rb.AddForce(transform.forward * speed);
    }

    private void OnDisable()
    {
        //재활용된 총알의 여러 효과값을 초기화
        trail.Clear();
        tr.position = Vector3.zero;
        tr.rotation = Quaternion.identity;
        rb.Sleep();
    }

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody>().AddForce(transform.forward * speed);   
    }

}
