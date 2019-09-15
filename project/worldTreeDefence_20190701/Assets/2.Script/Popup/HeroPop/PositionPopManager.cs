using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionPopManager : MonoBehaviour
{

    GameObject gm = null;

    UISprite heroImage = null;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);

        gm = GameObject.Find("GameManager");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Open() {
        gameObject.SetActive(true);
    }
    public void Close() {
        gameObject.SetActive(false);
    }

}
