using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectTileScript : MonoBehaviour
{

    public float damage; //적이 받는 데미지의 양
    public float speed = 1f;//발사체의 속도
    public Vector3 direction;//발사체가 향하고 있는 방향
    public float lifeDurtion = 10f;//발사체가 자폭하기 전까지 살아있는 시간

    // Start is called before the first frame update
    void Start()
    {
        //방향을 정규화한다
        direction = direction.normalized;
        //회전 값을 고친다
        float angle = Mathf.Atan2(-direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        //자폭용 타이머를 맞춘다
        Destroy(gameObject, lifeDurtion);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += direction * Time.deltaTime * speed;
    }
}
