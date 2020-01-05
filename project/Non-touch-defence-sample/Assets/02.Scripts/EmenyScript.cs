using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EmenyScript : MonoBehaviour
{

    public float speed = 1.0f;

    


    public NavMeshAgent nav;
    public GameObject target;


    // Start is called before the first frame update
    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        target = GameObject.Find("Player");
    }

    // Start is called before the first frame update
    void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
        target = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        int speedRange = Random.Range(1, 2);
        transform.Translate(-speed * speedRange * Time.deltaTime, 0.0f, 0.0f);
        //if (nav.destination != target.transform.position)
        //{
        //    nav.SetDestination(target.transform.position);
        //}
        //else
        //{
        //    nav.SetDestination(transform.position);
        //}



    }
}
