using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSystem : MonoBehaviour
{
    public Shader shader = null;
    public float CameraGridDistance = 20;
    public GameObject BuildingGrid = null;
    public GameObject PrefabBuilder = null;

    bool _buildingActive = false;

    public void Start()
    {
        _buildingActive = true;
        BuildingSystemToggle();
    }


    public void BuildingSystemToggle()
    {
        _buildingActive = !_buildingActive;
        BuildingGrid.SetActive(_buildingActive);
        PrefabBuilder.SetActive(_buildingActive);
        Shader.SetGlobalFloat("g_DiggerEditHeight", 10000);
    }

    void Update()
    {
        if (_buildingActive)
        {
            Vector3 gridPosition = Camera.main.transform.position + Camera.main.transform.rotation * Vector3.forward * CameraGridDistance;
            gridPosition.y = Mathf.Round(gridPosition.y);
            BuildingGrid.transform.position = gridPosition;

            float prefabY = gridPosition.y;

            Vector3 mouseDirection = Camera.main.ScreenPointToRay(Input.mousePosition).direction;
            mouseDirection /= Mathf.Abs(mouseDirection.y); //we need to get y as 1 to multiply the vector as y is our reference
            Vector3 selectionPosition = mouseDirection * (Camera.main.transform.position.y - prefabY) + Camera.main.transform.position ;
           
            PrefabBuilder.transform.position = new Vector3(Mathf.Round(selectionPosition.x), prefabY, Mathf.Round(selectionPosition.z)) - new Vector3(0.5f, 0.5f, 0.5f);
            BuildingGrid.transform.position = PrefabBuilder.transform.position;
            Shader.SetGlobalFloat("g_DiggerEditHeight", PrefabBuilder.transform.position.y);
        }
    
    }
}
