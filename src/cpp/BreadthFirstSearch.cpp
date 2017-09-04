/*
 * Given a map consisting of pathways denoted by '#' and locations denoted with
 * alphabetical characters, finds the path to a given location using a breadth-
 * first search.
 *
 * The map is a grid and each node can have up to four non-diagonal neighbors.
 */

#include <iostream>
#include <vector>
#include <unordered_map>
#include <unordered_set>
#include <queue>
#include <cstring>

class Grid
{
private:
  size_t m_width = 0;
  size_t m_height = 0;
  std::vector<char> m_grid;

  void FindDimensions(const char *map)
  {
    size_t width = 0;
    for (size_t i = 0; map[i] != '\0'; i++)
    {
      if (map[i] == '\n')
      {
        m_height++;
        m_width = std::max(m_width, width);
        width = 0;
      }
      else
        width++;
    }
  }
  
  void Populate(const char *map)
  {
    size_t x = 0;
    size_t y = 0;
    for (size_t i = 0; map[i] != '\0'; i++)
    {
      if (map[i] == '\n')
      {
        x = 0;
        y++;
      }
      else
      {
        m_grid[y * m_width + x] = map[i];
        x++;
      }
    }
  }

public:
  size_t Width() const
  {
    return m_width;
  }
  
  size_t Height() const
  {
    return m_height;
  }

  char Get(size_t x, size_t y) const
  {
    if (x >= m_width || y >= m_height)
      return 0;
    return m_grid[y * m_width + x];
  }

  void Print() const
  {
    for (size_t i = 0; i < m_height; i++)
    {
      std::cout.write(&m_grid[i * m_width], m_width);
      std::cout << std::endl;
    }
  }
  
  Grid(const char *map)
  {
    FindDimensions(map);
    m_grid.resize(m_width * m_height);
    Populate(map);
  }
};

class Graph
{
public:
  struct Node
  {
    int x;
    int y;
    char value;
    
    Node(int ix, int iy, char ivalue)
      : x(ix),
        y(iy),
        value(ivalue)
    {}
  };

  const std::vector<Node> &Nodes() const
  {
    return m_nodes;
  }

  const std::vector<int> &Neighbors(int node_id) const
  {
    auto it = m_neighbors.find(node_id);
    return it == m_neighbors.end() ? m_empty : it->second;
  }

  int FindFirstOf(char node_value) const
  {
    for (size_t i = 0; i < m_nodes.size(); i++)
    {
      if (m_nodes[i].value == node_value)
        return i;
    }
    return -1;
  }
  
  int FindNodeAt(size_t x, size_t y) const
  {
    auto it = m_location_to_node_id.find(y * m_width + x);
    return it == m_location_to_node_id.end() ? -1 : it->second;
  }

  Graph(const Grid &grid)
    : m_height(grid.Height()),
      m_width(grid.Width())
  {
    // Identify all nodes on grid
    int node_id = 0;
    for (size_t y = 0; y < grid.Height(); y++)
    {
      for (size_t x = 0; x < grid.Width(); x++)
      {
        char c = grid.Get(x, y);
        if (isalpha(c) || c == '#')
        {
          m_nodes.emplace_back(x, y, c);
          m_location_to_node_id[y * grid.Width() + x] = node_id++;
        }
      }
    }
    
    // Find neighbors of each node
    for (size_t i = 0; i < m_nodes.size(); i++)
    {
      Node &node = m_nodes[i];

      // Look up possible neighbors
      int t = LocationIndex(node.x, node.y - 1);
      int b = LocationIndex(node.x, node.y + 1);
      int l = LocationIndex(node.x - 1, node.y);
      int r = LocationIndex(node.x + 1, node.y);
      auto it_t = m_location_to_node_id.find(t);
      auto it_b = m_location_to_node_id.find(b);
      auto it_l = m_location_to_node_id.find(l);
      auto it_r = m_location_to_node_id.find(r);
      
      // Add neighbors to connectivity map
      std::vector<int> neighbors;
      if (it_t != m_location_to_node_id.end())
        neighbors.push_back(it_t->second);
      if (it_b != m_location_to_node_id.end())
        neighbors.push_back(it_b->second);
      if (it_l != m_location_to_node_id.end())
        neighbors.push_back(it_l->second);
      if (it_r != m_location_to_node_id.end())
        neighbors.push_back(it_r->second);
        
      m_neighbors[i] = neighbors;
    }
  }

private:
  const size_t m_height;
  const size_t m_width;
  std::vector<Node> m_nodes;
  std::unordered_map<int, std::vector<int>> m_neighbors;
  const std::vector<int> m_empty;
  std::unordered_map<int, int> m_location_to_node_id;
    
  int LocationIndex(int x, int y) const
  {
    if (x < 0 || x >= int(m_width) || y < 0 || y >= int(m_height))
      return -1;
    return y * m_width + x;
  }
};

void BreadthFirstSearch(std::unordered_map<int, int> *transitions_out, const Graph &graph, char node_type)
{
  int dest_node = graph.FindFirstOf(node_type);
  if (dest_node < 0)
    return;
    
  auto &transitions = *transitions_out;
  std::queue<int> frontier;
  frontier.push(dest_node);
  transitions[dest_node] = dest_node;
  
  while (!frontier.empty())
  {
    int node = frontier.front();
    frontier.pop();
    
    for (int neighbor: graph.Neighbors(node))
    {
      bool already_visited = transitions.find(neighbor) != transitions.end();
      if (already_visited)
        continue;

      transitions[neighbor] = node;
      frontier.push(neighbor);
    }
  }
}

static void PrintTransitionDirection(const Graph::Node &from, const Graph::Node &to)
{
  // Print direction of transition -- there are only 5 possibilities, including
  // the final destination that transitions to itself
  if (to.x > from.x)
    std::cout << '>';
  else if (to.x < from.x)
    std::cout << '<';
  else if (to.y > from.y)
    std::cout << 'V';
  else if (to.y < from.y)
    std::cout << '^';
  else
    std::cout << from.value;
}

void PrintAllPaths(const Grid &grid, const std::unordered_map<int, int> &transitions, const Graph &graph, const char start)
{
  int start_node = graph.FindFirstOf(start);
  if (start_node < 0)
    return;

  auto &nodes = graph.Nodes();
  for (size_t y = 0; y < grid.Height(); y++)
  {
    for (size_t x = 0; x < grid.Width(); x++)
    {
      int current = graph.FindNodeAt(x, y);
      if (current >= 0 && current != start_node)
      {
        // Find where this node transitions to
        auto it = transitions.find(current);
        if (it != transitions.end())
          PrintTransitionDirection(nodes[current], nodes[it->second]);
        else
          std::cout << '?';
      }
      else
        std::cout << grid.Get(x, y);
    }
    std::cout << std::endl;
  }
}

void PrintPath(const Grid &grid, const std::unordered_map<int, int> &transitions, const Graph &graph, const char start)
{
  int start_node = graph.FindFirstOf(start);
  if (start_node < 0)
    return;

  auto &nodes = graph.Nodes();
  std::unordered_set<int> on_path;
  int current = start_node;
  while (true)
  {
    auto it = transitions.find(current);
    if (it == transitions.end())
      break;
    if (it->second == current)  // final transition forms a loop
      break;
    on_path.insert(current);
    current = it->second;
  }
  
  for (size_t y = 0; y < grid.Height(); y++)
  {
    for (size_t x = 0; x < grid.Width(); x++)
    {
      int current = graph.FindNodeAt(x, y);
      if (on_path.count(current) > 0 && current != start_node)
      {
        // Find where this node transitions to
        auto it = transitions.find(current);
        if (it != transitions.end())
          PrintTransitionDirection(nodes[current], nodes[it->second]);
        else
          std::cout << '?';
      }
      else
        std::cout << grid.Get(x, y);
    }
    std::cout << std::endl;
  }
}

int main(int argc, char **argv)
{
  const char *map = 
    "################################################\n"
    "#                   #         #         #\n"
    "#                #  #         #         #\n"
    "#                #  #         #         #\n"
    "#                ####         ############z##\n"
    "#                   #         #\n"
    "#         #         #         #\n"
    "#   ############### #         #\n"
    "#         #         #         #\n"
    "#         #         #         #\n"
    "#         #         #######   #\n"
    "###########               #   #\n"
    "#         #               #   #\n"
    "#         #               #####\n"
    "#         #               #   #\n"
    "#################a#############\n";
  
  Grid grid(map);
  std::cout << "Map:" << std::endl;
  grid.Print();
  
  Graph graph(grid);

  std::unordered_map<int, int> transitions;
  BreadthFirstSearch(&transitions, graph, 'z');
  std::cout << std::endl << "All paths:" << std::endl;
  PrintAllPaths(grid, transitions, graph, 'a');
  std::cout << std::endl << "Path from a -> z:" << std::endl;
  PrintPath(grid, transitions, graph, 'a');

  return 0;
}