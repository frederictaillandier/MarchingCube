using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Observer : MonoBehaviour
{
    public static T Instantiate<T>(string path) where T : Observer
    {
        GameObject obj =  Instantiate(Resources.Load(path)) as GameObject;
        return obj.GetComponent<T>();
    }


    public void Subscribe(Observable observed) 
    {
        observed.SubscribeOnPositionUpdate(OnObservedMove);
    }

    void OnObservedMove(Vector3 pos) 
    {
       this.transform.position = pos;
    }
}
