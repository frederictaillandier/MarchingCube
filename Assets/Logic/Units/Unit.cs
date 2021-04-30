using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : Observable
{
    List<Vector3> _path = null;
    Vector3 _nextStep;

    public bool MoveTo(Vector3 target)
    {
        _path = AStar.Run(Position, target);
        _nextStep = Position;
        if (_path == null)
            return false;
        return true;
    }

    public override void Initialize()
    {
        base.Initialize();
    }

    protected override string GetObserverPath()
    {
        return "ObserverPrefabs/UnitObserver";
    }

    private void UpdatePosition()
    {
        if (Kill()) { }
        else if (Fall()) { }
        else if (Walk()) { }
    }

    private bool Fall()
    {
        if (TerrainManager.GetInstance().GetValue(Position + (Vector3.one / 2) + Vector3.down) == 0)
        {
            Position += Vector3.down;
            return true;
        }
        return false;
    }

    private bool Kill()
    {
        if (TerrainManager.GetInstance().GetValue(Position + (Vector3.one / 2)) == 1)
        {
            Position += Vector3.up;
            return true;
        }
        return false;
    }

    private bool Walk()
    {
        if (_nextStep != Position || (_path != null && _path.Count > 0))
        {
            var deltaTime = Time.deltaTime;
            var deltaStep = Vector3.Distance(_nextStep, Position);

            if (deltaStep > deltaTime)
            {
                Position = Position + (_nextStep - Position).normalized * deltaTime;
            }
            else
            {
                Position = _nextStep;
                if (_path != null && _path.Count > 0)
                {
                    _nextStep = _path[0];
                    _path.RemoveAt(0);
                    deltaTime -= deltaStep;
                    Position = Position + (_nextStep - Position).normalized * deltaTime;
                }
                else
                {
                    InvokeObservableEvent(ObservableEventType.DESTINATION_REACHED, new System.EventArgs());
                }
            }
            return true;
        }
        return false;
    }

    public void Tick()
    {
        UpdatePosition();
    }
}
