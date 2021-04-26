using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Observable
{
    public delegate void PositionUpdateDelegate(Vector3 pos);
    PositionUpdateDelegate _onPositionUpdate;
    private Observer _observer = null;
    protected Vector3 _position;
    protected abstract string GetObserverPath();

    public void Initialize()
    {
        if (_observer == null)
        {
            _observer = Observer.Instantiate<Observer>(GetObserverPath());
            _observer.Subscribe(this);
        }
    }

    public void SubscribeOnPositionUpdate(PositionUpdateDelegate function)
    {
        _onPositionUpdate += function;
    }

    public void UnSubscribeOnPositionUpdate(PositionUpdateDelegate function)
    {
        _onPositionUpdate -= function;
    }

    public void UnAllSubscribeOnPositionUpdate()
    {
        _onPositionUpdate = null;
    }

    public void Observe() 
    {
        if (_onPositionUpdate != null)
        {
            _onPositionUpdate.Invoke(_position);
        }
    }
}
