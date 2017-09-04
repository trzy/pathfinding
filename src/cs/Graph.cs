/*
 * A graph of nodes with connectivity and path information that wrap a user-
 * defined node type. Lookups and insertions tend to be done using the user
 * nodes, and paths and results are conveyed with the internal graph nodes.
 */

using System;
using System.Collections.Generic;

namespace PathFinder
{
  public class Graph<T>
  {
    public class Node
    {
      public T userNode;
      public List<Node> neighbors;
      public Node pathNext = null;  // set by search algorithms

      public void AddNeighbors(List<Node> newNeighbors)
      {
        neighbors.Capacity = neighbors.Capacity + newNeighbors.Count;
        foreach (Node newNeighbor in newNeighbors)
        {
          if (newNeighbor != null && !neighbors.Contains(newNeighbor))
            neighbors.Add(newNeighbor);
        }
      }

      public Node(T inUserNode)
      {
        userNode = inUserNode;
        neighbors = new List<Node>();
      }
    }

    private Dictionary<T, Node> m_nodes = new Dictionary<T, Node>();

    public Dictionary<T, Node>.ValueCollection Nodes
    {
      get { return m_nodes.Values; }
    }

    public Node AddNode(T userNode)
    {
      if (m_nodes.ContainsKey(userNode))
        return m_nodes[userNode];
      Node node = new Node(userNode);
      m_nodes.Add(userNode, node);
      return node;
    }

    public Node Find(T userNode)
    {
      Node node;
      if (userNode != null && m_nodes.TryGetValue(userNode, out node))
        return node;
      return null;
    }

    public void AddNeighbors(T userNode, params T[] userNeighbors)
    {
      Node node;
      if (m_nodes.TryGetValue(userNode, out node))
      {
        // Lookup the corresponding internal node for each of the user nodes
        List<Node> newNeighbors = new List<Node>(userNeighbors.Length);
        foreach (T userNeighbor in userNeighbors)
        {
          Node neighbor;
          if (userNeighbor != null && m_nodes.TryGetValue(userNeighbor, out neighbor))
            newNeighbors.Add(neighbor);
        }

        // Add them as neighbors
        node.AddNeighbors(newNeighbors);
      }
    }

    public List<Node> GetPathFrom(T userNode)
    {
      List<Node> path = new List<Node>();
      Node node = Find(userNode);
      while (node != null)
      {
        path.Add(node);
        if (node.pathNext == node)
          break;
        node = node.pathNext;
      }
      return path;
    }

    public Graph()
    {
    }
  }
}