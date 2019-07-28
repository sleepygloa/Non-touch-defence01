using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : Singleton<GameManager>
{

    Hashtable ht;


    // Start is called before the first frame update
    void Start()
    {
        ht = new Hashtable();

        ht.Add("count", 10);
        ht.Add("", "");

    }

    public Hashtable getEnemyManager() {

        return ht;
    }

}
