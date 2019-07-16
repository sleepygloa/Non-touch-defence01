using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroScrollViewScript : MonoBehaviour
{



    public void Open() { gameObject.SetActive(true); Debug.Log('d'); }

    public void Close() { gameObject.SetActive(false); }

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
