using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroDetailViewScript : MonoBehaviour
{


    public void Open() {
        //창 보이기 
        gameObject.SetActive(true);

        Text tx = gameObject.GetComponentInChildren<Text>();

        Debug.Log(tx);

        Debug.Log(tx.ToString());
    }

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
