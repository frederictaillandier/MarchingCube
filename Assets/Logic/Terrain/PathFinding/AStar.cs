using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Node
{
    public Vector3 Position;
    public float GCost; // Cumulated distance to Start
    public float HCost; // Fly distance to destination
    public Node Parent;

    public float Cost
    {
        get
        {
            return GCost + HCost;
        }
    }
}


public class AStar
{
    static public List<Vector3> Run(Vector3 start, Vector3 destination)
    {
        var open = new List<Node>();
        var openByPosition = new Dictionary<Vector3, Node>();
        var closedByPosition = new Dictionary<Vector3, Node>();
        TerrainManager _terrain = TerrainManager.GetInstance();

        var startNode = new Node()
        {
            Position = start,
            GCost = 0,
            HCost = 0,
            Parent = null
        };

        open.Add(startNode);
        var current = startNode;
        openByPosition.Add(startNode.Position, startNode);

        var countBeforeFailure = Constants.MAX_NODE_PATHFINDING;

        while (current.Position != destination && (--countBeforeFailure) != 0)
        {            
            open.Sort(delegate (Node x, Node y)
            {
                var rez = (x.Cost) - (y.Cost);
                if (rez == 0) return 0;
                else if (rez < 0) return -1;
                else return 1;
            });
            current = open[0];
            open.RemoveAt(0);
            openByPosition.Remove(current.Position);
            closedByPosition.Add(current.Position, current);
            var neighbours = GenerateNeighbours(current);
            for (int j = 0; j < neighbours.Count; ++j)
            {
                var neighbour = neighbours[j];
                if (closedByPosition.ContainsKey(neighbour.Position))
                    continue;
                if (openByPosition.ContainsKey(neighbour.Position) && openByPosition[neighbour.Position].Cost > neighbour.Cost)
                {
                    var replaceNode = openByPosition[neighbour.Position];
                    replaceNode.GCost = neighbour.GCost;
                    replaceNode.Parent = neighbour.Parent;
                    // no need to replace Position nor HCost
                }
                else if (!openByPosition.ContainsKey(neighbour.Position))
                {
                    neighbour.HCost = Vector3.Distance(neighbour.Position, destination);
                    if (neighbour.HCost == 0) // It means we found the target
                    {
                        return GeneratePathFromNode(neighbour);
                    }
                    openByPosition.Add(neighbour.Position, neighbour);
                    open.Add(neighbour);
                }

            }
        }
        Debug.LogWarning("Path finding failed: start " + start + " destination " + destination);
        return null;
    }

    static List<Vector3> GeneratePathFromNode(Node node)
    {
        var result = new List<Vector3>();
        while (node != null)
        {
            result.Insert(0, node.Position);
            node = node.Parent;
        }
        return result;
    }

    static List<Node> GenerateNeighbours(Node node)
    {
        TerrainManager _terrain = TerrainManager.GetInstance();
        var neighbours = new List<Node>();
        for (int x = -1; x <= 1; ++x)
        {
            for (int y = -1; y <= 1; ++y)
            {
                for (int z = -1; z <= 1; ++z)
                {
                    if (!(x == 0 && y == 0 && z == 0)) // rulling out the same point 
                    {
                        var newDirection = new Vector3(x, y, z);
                        if (!_terrain.IsOutsideMap(node.Position + newDirection)) // rulling out the outside map
                        {
                            var cost = MovementManager.GetInstance().GetSegmentDistance(node.Position, node.Position + newDirection);
                            if (cost > 0) //rulling out the non accessible points
                            {
                                neighbours.Add(new Node()
                                {
                                    Position = node.Position + newDirection,
                                    GCost = node.GCost + cost,
                                    Parent = node
                                });
                            }
                        }
                    }
                }
            }
        }
        return neighbours;
    }
}
