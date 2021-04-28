using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementManager : MonoBehaviour 
{
    static MovementManager _instance = null;

    public static MovementManager GetInstance()
    {
        return _instance;
    }

    public void Awake()
    {
        _instance = this;
    }

    public float GetSegmentDistance(Vector3 a, Vector3 b) 
    {
        // Making sure we are withing 1 sq distance 
        if ((b.x - a.x) * (b.x - a.x) <= 1.1f && 
            (b.y - a.y) * (b.y - a.y) <= 1.1f &&
            (b.z - a.z) * (b.z - a.z) <= 1.1f)
        {
            if (TerrainManager.GetInstance().GetValue(b) == 0 &&
            TerrainManager.GetInstance().GetValue(b + Vector3.up) == 0 && 
            TerrainManager.GetInstance().GetValue(b + Vector3.down) == 1)
            {
                return Vector3.Distance(a, b);
            }
        }
        return -1f;
    }
}
