/*
 * Holds a simple map of walkable tiles of type '#' or an alphabetic character.
 * All others are considered non-walkable and are stored as nulls.
 */

using System;
using System.Collections.Generic;

namespace PathFinder
{
  public class Map
  {
    public class Tile
    {
      public int x;
      public int y;
      public char type;

      override public string ToString()
      {
        return "(" + type + '|' + x + ',' + y + ')';
      }

      public Tile(int ix, int iy, char itype)
      {
        x = ix;
        y = iy;
        type = itype;
      }
    }

    private Tile[,] m_map;
    private int m_width = 0;
    private int m_height = 0;

    public Graph<Tile> BuildGraph()
    {
      Graph<Tile> graph = new Graph<Tile>();

      // Add all tiles as graph nodes
      for (int y = 0; y < m_height; y++)
      {
        for (int x = 0; x < m_width; x++)
        {
          if (m_map[y, x] != null)
            graph.AddNode(m_map[y, x]);
        }
      }

      // Connect up all neighbors
      for (int y = 0; y < m_height; y++)
      {
        for (int x = 0; x < m_width; x++)
        {
          if (m_map[y, x] != null)
          {
            Tile top = (y > 0) ? m_map[y - 1, x] : null;
            Tile bottom = (y < m_height - 1) ? m_map[y + 1, x] : null;
            Tile left = (x > 0) ? m_map[y, x - 1] : null;
            Tile right = (x < m_width - 1) ? m_map[y, x + 1] : null;
            graph.AddNeighbors(m_map[y, x], top, bottom, left, right);
          }
        }
      }

      return graph;
    }

    private char PathSymbol(Graph<Tile>.Node node)
    {
      var from = node.userNode;
      var to = node.pathNext.userNode;
      if (to.x > from.x)
        return '>';
      else if (to.x < from.x)
        return '<';
      else if (to.y > from.y)
        return 'V';
      else if (to.y < from.y)
        return '^';
      return '?';
    }

    // Assumes that the graph has already been searched for destination
    public void PrintPaths(Graph<Tile> graph, Tile destination)
    {
      for (int y = 0; y < m_height; y++)
      {
        for (int x = 0; x < m_width; x++)
        {
          char symbol = '?';
          Tile tile = m_map[y, x];
          if (tile == null)
            symbol = ' ';
          else if (tile == destination)
            symbol = tile.type;
          else
          {
            var node = graph.Find(tile);
            if (node.pathNext != null)
              symbol = PathSymbol(node);
          }
          Console.Write(symbol);
        }
        Console.WriteLine();
      }
    }

    // Assumes that the graph has already been searched for destination
    public void PrintPath(Graph<Tile> graph, Tile start, Tile destination)
    {
      var path = graph.GetPathFrom(start);
      for (int y = 0; y < m_height; y++)
      {
        for (int x = 0; x < m_width; x++)
        {
          char symbol;
          Tile tile = m_map[y, x];
          var node = graph.Find(tile);
          if (tile == null)
            symbol = ' ';
          else if (tile != start && tile != destination && path.Contains(node) && node.pathNext != null)
            symbol = PathSymbol(node);
          else
            symbol = tile.type;
          Console.Write(symbol);
        }
        Console.WriteLine();
      }
    }

    public void Print()
    {
      for (int y = 0; y < m_height; y++)
      {
        for (int x = 0; x < m_width; x++)
        {
          if (m_map[y, x] == null)
            Console.Write(' ');
          else
            Console.Write(m_map[y, x].type);
        }
        Console.WriteLine();
      }
    }

    public Tile FindFirstOf(char type)
    {
      foreach (Tile tile in m_map)
      {
        if (tile != null && tile.type == type)
          return tile;
      }
      return null;
    }

    public Map(string map)
    {
      // Find dimensions of map
      int lineWidth = 0;
      for (int i = 0; i < map.Length; i++)
      {
        m_height += (lineWidth == 0) ? 1 : 0; // robust line detection
        if (map[i] == '\n')
        {
          m_width = Math.Max(m_width, lineWidth);
          lineWidth = 0;
        }
        else
          lineWidth++;
      }

      // Allocate 2D array
      m_map = new Tile[m_height,m_width];

      // Populate the map
      int x = 0;
      int y = 0;
      for (int i = 0; i < map.Length; i++)
      {
        if (map[i] == '\n')
        {
          x = 0;
          y++;
        }
        else
        {
          if (map[i] == '#' || char.IsLetter(map[i]))
            m_map[y, x] = new Tile(x, y, map[i]);
          else
            m_map[y, x] = null;
          x++;
        }
      }
    }
  }
}