using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Simple Object used to test the Unit Object
public class UnitPupetteer : MonoBehaviour
{
    Unit _unit;

    // Start is called before the first frame update
    public void Start()
    {
        _unit = new Unit();
        _unit.Initialize();
        InvokeRepeating("Tick", 1f, 1f);
    }

    // Update is called once per frame
    void Tick()
    {
        _unit.Tick();
    }
}
