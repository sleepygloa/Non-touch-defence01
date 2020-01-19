using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : SingletonMonobehaviour<ResourceManager>
{
    public Dictionary<string, Object> ResourceContainer = new Dictionary<string, Object>();
    public Dictionary<string, GameObject[]> ResourcePool = new Dictionary<string, GameObject[]>();

    public static UnityEngine.Object Load(string path)
    {
        //resource.load
        return Resources.Load(path);
        //assetbundle.. load ->

    }

    public GameObject Instantiate(string path)
    {
        Object source = null;
        //기존에 로딩한적있는 리소스라면 바로 가져오고.
        if (ResourceContainer.ContainsKey(path) == true)
        {
            source = ResourceContainer[path];
        }
        else
        {   //
            source = ResourceManager.Load(path);
            ResourceContainer.Add(path, source);
        }
        if (source != null)
        {
            GameObject newObject = GameObject.Instantiate(source) as GameObject;
            return newObject;
        }
        else
        {
            Debug.LogWarning("Please Check Path Resource Load Faild :" + path);
            return null;
        }

    }

    public GameObject Instantiate(string path, Vector3 pos)
    {
        Object source = null;
        //기존에 로딩한적있는 리소스라면 바로 가져오고.
        if (ResourceContainer.ContainsKey(path) == true)
        {
            source = ResourceContainer[path];
        }
        else
        {   //
            source = ResourceManager.Load(path);
            ResourceContainer.Add(path, source);
        }
        if (source != null)
        {
            GameObject newObject = GameObject.Instantiate(source, pos, Quaternion.identity) as GameObject;
            return newObject;
        }
        else
        {
            Debug.LogWarning("Please Check Path Resource Load Faild :" + path);
            return null;
        }

    }
}
