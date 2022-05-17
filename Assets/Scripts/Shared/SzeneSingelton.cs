using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SzeneSingelton<T> : MonoBehaviour where T : MonoBehaviour
{
    static T _singelton;
    public static T Singelton
    {
        get {
            if (!_singelton)
            {
                T[] ts = FindObjectsOfType<T>();
                if (ts.Length == 0)
                {
                    _singelton = new GameObject(typeof(T).Name).AddComponent<T>();
                }
                else
                {
                    _singelton = ts[0];
                    if(ts.Length > 1)
                    {
                        Debug.LogWarning($"{ts.Length} objects of type {typeof(T)} found");
                    }
                }
            }
            return _singelton; 
        }
    }
}
