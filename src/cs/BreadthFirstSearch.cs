/*
 * Breadth-first search of a graph. Records paths into the graph's internal
 * nodes.
 */

using System;
using System.Collections.Generic;

namespace PathFinder
{
  public class BreadthFirstSearch<T>
  {
    // Find all paths leading to a destination. Returns false if completed.
    public static bool ComputeAllPaths(Graph<T> graph, T userDestination)
    {
      var destination = graph.Find(userDestination);
      if (destination == null)
        return true;

      // Clear current path
      foreach (var node in graph.Nodes)
      {
        node.pathNext = null;
      }

      // Perform search from destination outwards
      Queue<Graph<T>.Node> frontier = new Queue<Graph<T>.Node>();
      frontier.Enqueue(destination);
      destination.pathNext = destination;

      while (frontier.Count > 0)
      {
        var node = frontier.Dequeue();
        foreach (var neighbor in node.neighbors)
        {
          if (neighbor.pathNext != null)
            continue; // already visited
          neighbor.pathNext = node;
          frontier.Enqueue(neighbor);
        }
      }

      return false;
    }
  }
}