using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : Observable
{
    protected override string GetObserverPath()
    {
        return "ObserverPrefabs/UnitObserver";
    }

    private void UpdatePosition()
    {
        if (TerrainManager.GetInstance().GetValue(_position + Vector3.down) == 0)
        {
            _position += Vector3.down;
            return;
        }
        if (TerrainManager.GetInstance().GetValue(_position) == 1)
        {
            _position += Vector3.up;
            return;
        }

        var newDirectionList = new List<Vector3>();
        for (int x = -1; x <= 1; ++x)
        {
            for (int y = -1; y <= 1; ++y)
            {
                for (int z = -1; z <= 1; ++z)
                {
                    var newDirection = new Vector3(x, y, z);
                    if (!TerrainManager.GetInstance().IsOutsideMap(_position + newDirection) &&
                        MovementManager.GetInstance().GetSegmentDistance(_position, _position + newDirection) > 0)
                    {
                        newDirectionList.Add(newDirection);    
                    }
                }
            }
        }
        this._position += newDirectionList[Random.Range(0,newDirectionList.Count)];
        Observe();
    }

    public void Tick()
    {        
        UpdatePosition();
    }
}
