using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterListScript : MonoBehaviour
{
    GameObject go = GameObject.Find("HeroDetailView");

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Open() {
        Debug.Log(go);
        go.SetActive(true);
    }

    public void Close() {
        go.SetActive(false);
    }


}
