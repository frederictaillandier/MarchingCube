using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

/*
   * Cube indexes
      6 -- 7       y  z
     4 -- 5|       | /
     |    |3       0--x
     0 -- 1
*/

class OctoNode<T>
{
    Vector3 _position;
    T _value;
    int _size;
    List<OctoNode<T>> _children = null;

    public OctoNode(T value, int size, Vector3 position)
    {
        _value = value;
        _size = size;
        _position = position;
        _children = null;
    }

    public int Size
    {
        get
        {
            return _size;
        }
    }

    public Vector3 Position
    {
        get
        {
            return _position;
        }

    }

    public List<OctoNode<T>> Children
    {
        get {
            return _children;
        }
    }


    public T GetValue(Vector3 v)
    {
        if (_children == null)
            return _value;
        else
        {
            return _children[GetIndex(v)].GetValue(v);
        }
    }

    int GetIndex(Vector3 v)
    {
        int index = 0;

        index |= (v.x >= _position.x + _size / 2) ? 1 : 0;
        index |= (v.y >= _position.y + _size / 2) ? 4 : 0;
        index |= (v.z >= _position.z + _size / 2) ? 2 : 0;
        return index;
    }

    public void SetValue(Vector3 v, T value)
    {
        if (_size == 1 && !_value.Equals(value))
        {
            _value = value;
        }
        else if (_children == null && _value.Equals(value))
            return;
        else
        {
            if (_children == null)
            {
                _children = new List<OctoNode<T>>();
                for (int i = 0; i < 8; ++i)
                {
                    _children.Add(new OctoNode<T>(_value, _size / 2, _position + _size / 2 * new Vector3(i % 2, Mathf.Floor(i / 4), Mathf.Floor(i / 2) % 2)));
                }
            }
            var index = GetIndex(v);
            _children[index].SetValue(v, value);
            Clean(value);
        }
    }

    public void Clean(T value)
    {
        if (_children != null)
        {
            for (int i = 0; i < 8; ++i)
            {
                if (_children[i]._children != null || !_children[i]._value.Equals(value))
                    return;
            }
            _value = value;
            _children = null;
        }
    }

    /* EXPERIMENTAL TO DELETE */

    public void Show(T val)
    {
        if (_children == null && _value.Equals(val))
        {
            if (TerrainManager.cubeDrawer == null)
                TerrainManager.cubeDrawer = new GameObject("cubeDrawer");
            var g = GameObject.Instantiate(TerrainManager.prefab);
            g.transform.position = _position;
            g.transform.localScale *= _size;
            g.transform.name = string.Format("{0}{1}{2}", _position.x, _position.y, _position.z);
            g.transform.parent = TerrainManager.cubeDrawer.transform;
        }
        else if (_children != null)
        {
            for (int i = 0; i < _children.Count; ++i)
            {
                _children[i].Show(val);            
            }        
        }    
    }
    /* END EXPERIMENTAL TO DELETE */

}

class MarchingCubeBuilder
{
    Dictionary<Vector3, uint> _intersectionsToCompute = new Dictionary<Vector3, uint>();
    OctoTree<int> _octoTree = null;
    GameObject _buble = null;

    public MarchingCubeBuilder(OctoTree<int> octoTree, GameObject buble)
    {
        _octoTree = octoTree;
        _buble = buble;
    }

    public uint InspectIntersection(Vector3 pos)
    {
        uint value = 0;

        if (_octoTree.GetValue(pos + new Vector3(+0, +0, +0)) == 1) // right - up - back
            value = (value | (1<<0));
        if (_octoTree.GetValue(pos + new Vector3(-1, +0, +0)) == 1) // left - up - back
            value = (value | (1 <<1));
        if (_octoTree.GetValue(pos + new Vector3(+0, +0, -1)) == 1) // right - up - front
            value = (value | (1 <<2));
        if (_octoTree.GetValue(pos + new Vector3(-1, +0, -1)) == 1)
            value = (value | (1 <<3));
        if (_octoTree.GetValue(pos + new Vector3(+0, -1, +0)) == 1)
            value = (value | (1 <<4));
        if (_octoTree.GetValue(pos + new Vector3(-1, -1, +0)) == 1)
            value = (value | (1 <<5));
        if (_octoTree.GetValue(pos + new Vector3(+0, -1, -1)) == 1)
            value = (value | (1 <<6));
        if (_octoTree.GetValue(pos + new Vector3(-1, -1, -1)) == 1)
            value = (value | (1 <<7));
        return value;
    }

    public void FillShellIntersections(OctoNode<int> node)
    {
        for (int i = 0; i <= node.Size; ++i)
        {
            for (int j = 0; j <= node.Size; ++j)
            {
                //Front
                uint val = InspectIntersection(node.Position + new Vector3(i, j, 0));
                if (val != 0 && val != 255)
                    _intersectionsToCompute.Add(node.Position + new Vector3(i, j, 0), val);
                //Back
                val = InspectIntersection(node.Position + new Vector3(i, j, node.Size));
                if (val != 0 && val != 255)
                    _intersectionsToCompute.Add(node.Position + new Vector3(i, j, node.Size), val);


                if (j != 0 && j != node.Size) //Do not recompute the edges
                {
                    //Left
                    val = InspectIntersection(node.Position + new Vector3(0, i, j));
                    if (val != 0 && val != 255)
                        _intersectionsToCompute.Add(node.Position + new Vector3(0, i, j), val);
                    //Right
                    val = InspectIntersection(node.Position + new Vector3(node.Size, i, j));
                    if (val != 0 && val != 255)
                        _intersectionsToCompute.Add(node.Position + new Vector3(node.Size, i, j), val);
                }

                if (j != 0 && j != node.Size && i != node.Size && i != 0) //Do not recompute the edges
                {
                    //Bottom
                    val = InspectIntersection(node.Position + new Vector3(i, 0, j));
                    if (val != 0 && val != 255)
                        _intersectionsToCompute.Add(node.Position + new Vector3(i, 0, j), val);
                    //Top
                    val = InspectIntersection(node.Position + new Vector3(i, node.Size, j));
                    if (val != 0 && val != 255)
                        _intersectionsToCompute.Add(node.Position + new Vector3(i, node.Size, j), val);
                }
            }
        }    
    }

    public void FillNodeIntersections(OctoNode<int> node)
    {
        uint val = 0;

        for (int i = 1; i < node.Size; ++i)
        {
            if (i != node.Size / 2)
            {
                for (int j = 1; j < node.Size; ++j)
                {
                    if (j != node.Size / 2)
                    {
                        //x Face
                        val = InspectIntersection(node.Position + new Vector3(node.Size / 2, i, j));
                        if (val != 0 && val != 255)
                            _intersectionsToCompute.Add(node.Position + new Vector3(node.Size / 2, i, j), val);
                        //y Face
                        val = InspectIntersection(node.Position + new Vector3(i, node.Size / 2, j));
                        if (val != 0 && val != 255)
                            _intersectionsToCompute.Add(node.Position + new Vector3(i, node.Size / 2, j), val);
                        //z Face
                        val = InspectIntersection(node.Position + new Vector3(i, j, node.Size / 2));
                        if (val != 0 && val != 255)
                            _intersectionsToCompute.Add(node.Position + new Vector3(i, j, node.Size / 2), val);
                    }

                }
                //x Segment
                val = InspectIntersection(node.Position + new Vector3(i, node.Size / 2, node.Size / 2));
                if (val != 0 && val != 255)
                    _intersectionsToCompute.Add(node.Position + new Vector3(i, node.Size / 2, node.Size / 2), val);
                //y Segment
                val = InspectIntersection(node.Position + new Vector3(node.Size / 2, i, node.Size / 2));
                if (val != 0 && val != 255)
                    _intersectionsToCompute.Add(node.Position + new Vector3(node.Size / 2, i, node.Size / 2), val);
                //z Segment
                val = InspectIntersection(node.Position + new Vector3(node.Size / 2, node.Size / 2, i));
                if (val != 0 && val != 255)
                    _intersectionsToCompute.Add(node.Position + new Vector3(node.Size / 2, node.Size / 2, i), val);
            }
        }
        //Middle
        val = InspectIntersection(node.Position + new Vector3(node.Size / 2, node.Size / 2, node.Size / 2));
        if (val != 0 && val != 255)
            _intersectionsToCompute.Add(node.Position + new Vector3(node.Size/2, node.Size/2, node.Size/2), val);
    }

    public void DetectIntersections(OctoNode<int> octoNode)
    {
        if (octoNode.Children != null)
        {
            FillNodeIntersections(octoNode);
            for (int i = 0; i < 8; ++i)
            {
                DetectIntersections(octoNode.Children[i]);
            }        
        }
    }

    public void DrawIntersections()
    {
        foreach (var item in _intersectionsToCompute)
        {
            if (TerrainManager.sphereDrawer == null)
                TerrainManager.sphereDrawer = new GameObject("SphereDrawer");
            var buble = GameObject.Instantiate(_buble, item.Key, Quaternion.identity);
            buble.transform.parent = TerrainManager.sphereDrawer.transform;

        }
    }

    public void BuildMesh()
    {
        // Reversed List (values as indexes and indexes as value to use the hashmap)
        // in order to build the vertices and keep its references and avoid find()
        Dictionary<Vector3, int> verticesAndRefs = new Dictionary<Vector3, int>();
        List<int> triangles = new List<int>();


        foreach (var intersect in _intersectionsToCompute)
        {
            var march = MarchingCubeLibrary.GetMarch(intersect.Value);

            if (march != null)
            {
                Dictionary<int, int> indexTranslator = new Dictionary<int, int>();//allows translation of vertices' index between march and global mesh

                for (int i = 0; i < march.vertices.Count; ++i)
                {
                    var verticeGlobalIndex = 0;
                    if (!verticesAndRefs.ContainsKey(march.vertices[i] + intersect.Key))
                    {
                        verticeGlobalIndex = verticesAndRefs.Count;
                        verticesAndRefs.Add(march.vertices[i] + intersect.Key, verticeGlobalIndex);
                    }
                    else
                    {
                        verticeGlobalIndex = verticesAndRefs[march.vertices[i] + intersect.Key];
                    }
                    indexTranslator[i] = verticeGlobalIndex;
                }
                for (int i = 0; i < march.tris.Count; ++i)
                {
                    triangles.Add(indexTranslator[march.tris[i]]);
                }
            }
          

        }
        GameObject terrain = new GameObject("Terrain");
        MeshRenderer meshRenderer = terrain.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = new Material(Shader.Find("Standard"));

        MeshFilter meshFilter = terrain.AddComponent<MeshFilter>();

        Mesh mesh = new Mesh();

        mesh.vertices = verticesAndRefs.Keys.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;
    }

}

class OctoTree<T>
{
    OctoNode<T> _root = null;

    public OctoTree(T value, int size)
    {
        _root = new OctoNode<T>(value, size, Vector3.zero);
    }

    public T GetValue(Vector3 v)
    {
        if (v.x < 0)
            v.x = 0;
        else if (v.x >= _root.Size)
            v.x = _root.Size;

        if (v.y < 0)
            v.y = 0;
        else if (v.y >= _root.Size)
            v.y = _root.Size;

        if (v.z < 0)
            v.z = 0;
        else if (v.z >= _root.Size)
            v.z = _root.Size;
        return _root.GetValue(v);
    }

    public void SetValue(Vector3 v, T value)
    {
        _root.SetValue(v, value);
    }

    public OctoNode<T> Root
    {
        get
        {
            return _root;
        }
    }

    public void ShowCubes(T value)
    {
        _root.Show(value);
    }
}
public class TerrainManager : MonoBehaviour
{
    public static GameObject prefab = null;
    public GameObject bubblePrefab = null;
    public static GameObject cubeDrawer = null;
    public static GameObject sphereDrawer = null;

    public GameObject OctoNodePredab;
    OctoTree<int> _terrain;


    void TestGenerateTerrain()
    {

       

    }

    void DebugTerrain()
    { 
    
    
    }

    void Start()
    {
        int mapSize = 64;
       

        prefab = OctoNodePredab;
        _terrain = new OctoTree<int>(0, mapSize);


        for (int i = 0; i < mapSize * mapSize; ++i)
        {
            _terrain.SetValue(new Vector3(i / mapSize, 0, i % mapSize), 1);
            _terrain.SetValue(new Vector3(i / mapSize, 1, i % mapSize), 1);

        }

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

        TestGenerateTerrain();

        MarchingCubeBuilder marchingCubeBuilder = new MarchingCubeBuilder(_terrain, bubblePrefab);
        marchingCubeBuilder.FillShellIntersections(_terrain.Root);
        marchingCubeBuilder.DetectIntersections(_terrain.Root);
        //_terrain.ShowCubes(1);
        //     marchingCubeBuilder.DrawIntersections();
        marchingCubeBuilder.BuildMesh();
    }
}
