using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObservableEventType
{
    POSITION_UPDATE, 
    DESTINATION_REACHED
}

public abstract class Observable
{
    public delegate void ObservableEvent(System.EventArgs args);

    Dictionary<ObservableEventType, ObservableEvent> _observableEvents;
    private Observer _observer = null;
    protected abstract string GetObserverPath();

    public virtual void Initialize()
    {
        if (_observer == null)
        {
            _observableEvents = new Dictionary<ObservableEventType, ObservableEvent>();
            _observer = Observer.Instantiate<Observer>(GetObserverPath());
            _observer.Subscribe(this);
        }
    }

#region POSITION
    private Vector3 _position;

    public Vector3 Position
    {
        get 
        {
            return _position;
        }
        set 
        {
            _position = value;
            ObservePosition();
        }        
    }

    public void ObservePosition() 
    {
        if (_observableEvents.ContainsKey(ObservableEventType.POSITION_UPDATE))
        {
            _observableEvents[ObservableEventType.POSITION_UPDATE].Invoke(new PositionUpdateArgs() { Position = Position });
        }
    }

#endregion // POSITION
#region Events
    public void InvokeObservableEvent(ObservableEventType type, System.EventArgs args)
    {
        if (_observableEvents.ContainsKey(type))
            _observableEvents[type].Invoke(args);
    }
    
    public void SubscribeObservableEvent(ObservableEventType type, ObservableEvent function) 
    {
        if (!_observableEvents.ContainsKey(type))
            _observableEvents[type] = function;
        else 
            _observableEvents[type] += function;
    }

    public void UnSubscribeObservableEvent(ObservableEventType type, ObservableEvent function) 
    {
        _observableEvents[type] -= function;
    }

    public void UnSubscribeAllObservableEvent()
    {
        _observableEvents = null;
    }
#endregion // Events  
}
