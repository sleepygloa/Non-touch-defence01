using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public float jumpPower;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if(Input.GetButtonDown("Jump")){
            GetComponent<Rigidbody>().velocity = new Vector3(0, jumpPower, 0);
        }	
	}

    private void OnCollisionEnter(Collision collision)
    {
        Application.LoadLevel(Application.loadedLevel);
    }
}
