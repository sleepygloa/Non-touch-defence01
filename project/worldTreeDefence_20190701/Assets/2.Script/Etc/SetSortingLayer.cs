using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetSortingLayer : MonoBehaviour
{
    public string layerName;
    public Renderer setRenderer;

    // Use this for initialization
    void Start()
    {
        if(setRenderer != null)
        {
            setRenderer.sortingLayerName = layerName;
        }
    }


}
