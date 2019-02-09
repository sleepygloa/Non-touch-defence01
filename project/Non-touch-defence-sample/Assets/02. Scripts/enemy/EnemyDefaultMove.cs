using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDefaultMove : MonoBehaviour {

    public float speed;

	// Use this for initialization
	void Start () {
        Destroy(gameObject, 10f);
	}
	
	// Update is called once per frame
	void Update () {
        transform.Translate(-speed * Time.deltaTime, 0, 0);
	}
}
