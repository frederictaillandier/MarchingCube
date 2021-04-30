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
        _unit.SubscribeObservableEvent(ObservableEventType.DESTINATION_REACHED, OnDestinationReached);
        _unit.Position = new Vector3(2,2,2);
        ForceNewTarget();
    }

    // Update is called once per frame
    void Update()
    {
        _unit.Tick();
    }

    void OnDestinationReached(System.EventArgs args)
    {
        ForceNewTarget();
    }

    void ForceNewTarget()
    {
        while (!_unit.MoveTo(new Vector3((int)Random.Range(0, Constants.TERRAIN_SIZE), 2, (int)Random.Range(0, Constants.TERRAIN_SIZE)))) {}
    }

}
