using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : Observable
{
    List<Vector3> path = null;
    Vector3 _nextStep;

    protected override string GetObserverPath()
    {
        return "ObserverPrefabs/UnitObserver";
    }

    private void UpdatePosition()
    {
        if (Kill()) { }
        else if (Fall()) { }
        else if (Walk()) { }
        Observe();
    }

    private bool Fall()
    {
        if (TerrainManager.GetInstance().GetValue(_position + (Vector3.one/2) + Vector3.down) == 0)
        {
            _position += Vector3.down;
            return true;
        }
        return false;
    }

    private bool Kill()
    {
        if (TerrainManager.GetInstance().GetValue(_position + (Vector3.one/2)) == 1)
        {
            _position += Vector3.up;
            return true;
        }
        return false;
    }

    private bool Walk()
    {
        if (path == null || path.Count == 0)
        {
            path = AStar.Run(_position, new Vector3((int)Random.Range(0, Constants.TERRAIN_SIZE), 2, (int)Random.Range(0, Constants.TERRAIN_SIZE)));
            _nextStep = path[0];
            path.RemoveAt(0);

        }
        if (path != null && path.Count > 0)
        {
            var deltaTime = Time.deltaTime;
            var deltaStep = Vector3.Distance(_nextStep, _position);

            if (deltaStep > deltaTime)
            {
                _position = _position + (_nextStep - _position).normalized * deltaTime;
            }
            else
            {
                _position = _nextStep;
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
