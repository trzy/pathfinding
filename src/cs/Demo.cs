using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinder
{
  class Program
  {
    static void Main(string[] args)
    {
      string myMap = string.Join("\n",
        "################################################",
        "#                   #         #         #",
        "#                #  #         #         #",
        "#                #  #         #         #",
        "#                ####         ############z##",
        "#                   #         #",
        "#         #         #         #",
        "#   ##############a #         #",
        "#         #         #         #",
        "#         #         #         #",
        "#         #         #######   #",
        "###########               #   #",
        "#         #               #   #",
        "#         #               #####",
        "#         #               #   #",
        "###############################");

      Map map = new Map(myMap);
      Map.Tile start = map.FindFirstOf('a');
      Map.Tile destination = map.FindFirstOf('z');
      Console.WriteLine("Map:");
      map.Print();
      Console.WriteLine();

      Graph<Map.Tile> graph = map.BuildGraph();
      BreadthFirstSearch<Map.Tile>.ComputeAllPaths(graph, destination);
      Console.WriteLine("All paths to " + destination.type + ':');
      map.PrintPaths(graph, destination);
      Console.WriteLine();

      Console.WriteLine("Path from " + start.type + " to " + destination.type + ':');
      map.PrintPath(graph, start, destination);
      Console.WriteLine();
    }
  }
}
