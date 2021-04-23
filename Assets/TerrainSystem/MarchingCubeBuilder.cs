using System.Collections.Generic;
using System.Linq;
using UnityEngine;

class MarchingCubeBuilder
{
    Dictionary<Vector3, uint> _intersectionsToCompute = new Dictionary<Vector3, uint>();
    OctoTree<int> _octoTree = null;


    private MarchingCubeBuilder(OctoTree<int> octoTree)
    {
        _octoTree = octoTree;
    }

    public static GameObject Build(OctoTree<int> octoTree)
    {
        MarchingCubeBuilder builder = new MarchingCubeBuilder(octoTree);
        builder.FillShellIntersections(octoTree.Root);
        builder.DetectIntersections(octoTree.Root);
        return builder.BuildMeshSharp();
    }

    private uint InspectIntersection(Vector3 pos)
    {
        uint value = 0;

        if (_octoTree.GetValue(pos + new Vector3(+0, +0, +0)) == 1) // right - up - back
            value = (value | (1 << 0));
        if (_octoTree.GetValue(pos + new Vector3(-1, +0, +0)) == 1) // left - up - back
            value = (value | (1 << 1));
        if (_octoTree.GetValue(pos + new Vector3(+0, +0, -1)) == 1) // right - up - front
            value = (value | (1 << 2));
        if (_octoTree.GetValue(pos + new Vector3(-1, +0, -1)) == 1)
            value = (value | (1 << 3));
        if (_octoTree.GetValue(pos + new Vector3(+0, -1, +0)) == 1)
            value = (value | (1 << 4));
        if (_octoTree.GetValue(pos + new Vector3(-1, -1, +0)) == 1)
            value = (value | (1 << 5));
        if (_octoTree.GetValue(pos + new Vector3(+0, -1, -1)) == 1)
            value = (value | (1 << 6));
        if (_octoTree.GetValue(pos + new Vector3(-1, -1, -1)) == 1)
            value = (value | (1 << 7));
        return value;
    }

    private void FillShellIntersections(OctoNode<int> node)
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

    private void FillNodeIntersections(OctoNode<int> node)
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
            _intersectionsToCompute.Add(node.Position + new Vector3(node.Size / 2, node.Size / 2, node.Size / 2), val);
    }

    private void DetectIntersections(OctoNode<int> octoNode)
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


    private GameObject BuildMeshSharp()
    {

        var vertices = new List<Vector3>();
        var triangles = new List<int>();
        int indexStart = 0;

        foreach (var intersect in _intersectionsToCompute)
        {
            var march = MarchingCubeLibrary.GetMarch(intersect.Value);

            if (march != null)
            {    
                foreach (var vertice in march.vertices)
                {
                    vertices.Add(vertice + intersect.Key);
                }
                foreach (var triangle in march.tris)
                {
                    triangles.Add(triangle + indexStart);
                }
                indexStart = vertices.Count();

            }
        }


        GameObject terrain = new GameObject("Terrain");
        MeshRenderer meshRenderer = terrain.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = new Material(Shader.Find("Standard"));

        MeshFilter meshFilter = terrain.AddComponent<MeshFilter>();

        Mesh mesh = new Mesh();

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;
        return terrain;
    }

    private GameObject BuildMesh()
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
        return terrain;
    }
}
