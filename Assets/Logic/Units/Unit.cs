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
        this._position += new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1));
        Observe();
    }

    public void Tick()
    {        
        UpdatePosition();
    }
}
