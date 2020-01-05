using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//T = type
public class SingletonMonobehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    static T _instance = null;

    public static T Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = (T)FindObjectOfType(typeof(T));
                if(_instance == null)
                {
                    var _newGameObject = new GameObject(typeof(T).ToString());
                    _instance = _newGameObject.AddComponent<T>();
                }
                
            }
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if(_instance == null)
        {
            _instance = this as T;
        }
        //씬이 변경되어도 사라지지 않는 객체로 지정합니다.
        //가급적이면 트랜스폼 루트 게임오브젝트를 파라메터로 넘겨주는 것이 좋습니다.
        DontDestroyOnLoad(gameObject);
    }
}
