using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class damageTextDestoryTimeScript : MonoBehaviour
{

    public float DestroyTime = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, DestroyTime);
    }

}
