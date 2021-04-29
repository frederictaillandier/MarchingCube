using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : Observable
{
    List<Vector3> path = null;
    Vector3 _nextStep;

    public void Initialize()
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
        ObservePosition();
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
        if (path == null || path.Count == 0)
        {
            path = AStar.Run(Position, new Vector3((int)Random.Range(0, Constants.TERRAIN_SIZE), 2, (int)Random.Range(0, Constants.TERRAIN_SIZE)));
            if (path != null && path.Count > 0)
            {
                _nextStep = path[0];
                path.RemoveAt(0);
            }
        }
        if (path != null && path.Count > 0)
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
                _nextStep = path[0];
                path.RemoveAt(0);
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
