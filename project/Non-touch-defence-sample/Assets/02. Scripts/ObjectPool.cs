using UnityEngine;
using System.Collections;

public class ObjectPool : MonoBehaviour
{
    [SerializeField]
    private GameObject[] objectPrefab;

    private int enermyNum = 0;

    public GameObject GetObject(string type) 
    { 


        for (int i = 0; i < objectPrefab.Length; i++)
        { 
            if(objectPrefab[i].name == type) 
            {
                enermyNum++;

                GameObject  newObject = Instantiate(objectPrefab[i]);
                newObject.name = type+"_"+ enermyNum;


                int spawnPosition = Random.Range(-50, 15);
                newObject.transform.position = new Vector2(newObject.transform.position.x + spawnPosition, newObject.transform.position.y);
                return newObject;
            }
        }

        return null;
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
