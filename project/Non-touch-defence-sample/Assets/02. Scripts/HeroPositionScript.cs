using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroPositionScript : MonoBehaviour
{
    private SpriteRenderer sr;
    public Sprite[] sprite;

    public bool pickFlag = false;


    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = sprite[0];
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Click() {
        if (!pickFlag)
        {
            sr.sprite = sprite[1];
            pickFlag = true;
        }
        else
        {
            sr.sprite = sprite[0];
            pickFlag = false;
        }
    }
}
