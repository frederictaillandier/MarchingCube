using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    public static GameObject prefab = null;
    public static GameObject cubeDrawer = null;
    public static GameObject sphereDrawer = null;
    OctoTree<int> _terrain;

    #region Test_And_Debug
    void CustomTerrainGenerationForTest()
    {
        _terrain.SetValue(new Vector3(10, 5, 10), 1);
        _terrain.SetValue(new Vector3(11, 5, 10), 0);
        _terrain.SetValue(new Vector3(10, 5, 11), 1);
        _terrain.SetValue(new Vector3(11, 5, 11), 1);
        _terrain.SetValue(new Vector3(10, 6, 10), 0);
        _terrain.SetValue(new Vector3(11, 6, 10), 0);
        _terrain.SetValue(new Vector3(10, 6, 11), 0);
        _terrain.SetValue(new Vector3(11, 6, 11), 1);

        Vector3 center = new Vector3(30, 3, 50);
        Vector3 center2 = new Vector3(20, 2, 20);

        for (int x = -20; x < 20; ++x)
            for (int y = -20; y < 20; ++y)
                for (int z = -20; z < 20; ++z)
                {
                    if ((x * x + y * y * 6 + z * z) < 64)
                    {
                        _terrain.SetValue(center + new Vector3(x, y, z), 0);
                        _terrain.SetValue(center2 + new Vector3(x, y, z), 1);
                    }
                }

        for (int x = -20; x < 20; ++x)
        {
            _terrain.SetValue(center2 + new Vector3(x, 0, 1), 0);
            _terrain.SetValue(center2 + new Vector3(x, 0, 0), 0);
        }


    }


    #endregion

    void Start()
    {
        //Dummy Initialize
        _terrain = new OctoTree<int>(0, Constants.TERRAIN_SIZE);
        for (int i = 0; i < Constants.TERRAIN_SIZE * Constants.TERRAIN_SIZE; ++i)
        {
            _terrain.SetValue(new Vector3(i / Constants.TERRAIN_SIZE, 0, i % Constants.TERRAIN_SIZE), 1);
            _terrain.SetValue(new Vector3(i / Constants.TERRAIN_SIZE, 1, i % Constants.TERRAIN_SIZE), 1);
        }

        //Only for testing
        CustomTerrainGenerationForTest();
        //Build Terrain
        MarchingCubeBuilder.Build(_terrain);
    }
}
