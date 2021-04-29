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

    public virtual void Subscribe(Observable observed) 
    {
        observed.SubscribeObservableEvent(ObservableEventType.POSITION_UPDATE, OnPositionUpdate);
    }

    public void OnPositionUpdate(System.EventArgs args)
    {
        transform.position = (args as PositionUpdateArgs).Position;
    }
}
