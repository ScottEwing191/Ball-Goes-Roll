using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.Log(typeof(T).ToString() + " is NULL.");
            }
            return instance;
        }
    }
    protected virtual void Awake()
    {
        instance = this as T;
        Init();
    }

    public virtual void Init()
    {
        // Optional to overide
    }
    
}
