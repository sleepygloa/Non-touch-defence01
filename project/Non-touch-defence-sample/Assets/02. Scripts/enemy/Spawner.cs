using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

    public GameObject slime;
    public float interval;
    public float range = 3.0f;

	// Use this for initialization
    IEnumerator Start () {
        while (true){

            transform.position = new Vector3(Random.Range(0, range), transform.position.y,
                                             transform.position.z);

            Instantiate(slime, transform.position, transform.rotation);
            yield return new WaitForSeconds(interval);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
