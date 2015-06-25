using System;
using System.Collections.Generic;
using System.Linq;

/**
 * Auto-generated code below aims at helping you parse
 * the standard input according to the problem statement.
 **/

public class Node<T> where T : struct
{
    public Node<T> Parent { get; set; }
    public List<Node<T>> Children { get; set; }

    public int Depth
    {
        get
        {
            Node<T> parent = this.Parent;
            int i = 0;
            while (parent != null)
            {
                i++;
                parent = parent.Parent;
            }
            return i;
        }
    }

    public Node<T> Child(int index)
    {
        try
        {
            return Children[index];
        }
        catch (IndexOutOfRangeException ex)
        {
            return null;
        }
    }

    public T Value;

    public Node(T value)
    {
        this.Value = value;
        this.Children = new List<Node<T>>();
    }

    public Node<T> AddChild(T item)
    {
        Node<T> child = new Node<T>(item);
        this.Children.Add(child);
        child.Parent = this;
        return child;
    }
}
public class Tree<T> where T : struct
{
    public Node<T> Root { get; set; }

    public Tree(T item)
    {
        this.Root = new Node<T>(item);
    }
}

class Player
{
    static void Main(string[] args)
    {
        List<int> gateways = new List<int>();
        List<Tree<int>> gatewayTrees = new List<Tree<int>>();
        List<Tuple<int, int>> links = new List<Tuple<int, int>>();

        string[] inputs;
        inputs = Console.ReadLine().Split(' ');
        int N = int.Parse(inputs[0]); // the total number of nodes in the level, including the gateways
        int L = int.Parse(inputs[1]); // the number of links
        int E = int.Parse(inputs[2]); // the number of exit gateways
        for (int i = 0; i < L; i++)
        {
            inputs = Console.ReadLine().Split(' ');
            links.Add(new Tuple<int, int>(int.Parse(inputs[0]), int.Parse(inputs[1])));
            links.Add(new Tuple<int, int>(int.Parse(inputs[1]), int.Parse(inputs[0])));
        }
        for (int i = 0; i < E; i++)
        {
            int EI = int.Parse(Console.ReadLine()); // the index of a gateway node
            gateways.Add(EI);
            gatewayTrees.Add(new Tree<int>(EI));
        }

        foreach (Tree<int> gatewayTree in gatewayTrees)
        {
            CreateTree(gatewayTree, links, gateways);
        }

        // game loop
        while (true)
        {
            int SI = int.Parse(Console.ReadLine()); // The index of the node on which the Skynet agent is positioned this turn

            Node<int> nodeToBeSevered = gatewayTrees
                .Select(gatewayTree => FindInfectedNode(gatewayTree.Root, SI))
                .Where(infectedNode => infectedNode != null)
                .OrderBy(infectedNode => infectedNode.Depth)
                .FirstOrDefault();

            if (nodeToBeSevered != null)
            {
                SeverNode(gatewayTrees, nodeToBeSevered.Parent.Value, SI);

                if (links.Contains(new Tuple<int, int>(nodeToBeSevered.Parent.Value, nodeToBeSevered.Value)))
                {
                    Console.WriteLine("{0} {1}", nodeToBeSevered.Parent.Value, nodeToBeSevered.Value);
                }
                else
                {
                    Console.WriteLine("{0} {1}", nodeToBeSevered.Value, nodeToBeSevered.Parent.Value);
                }
            }

            // Write an action using Console.WriteLine()
            // To debug: Console.Error.WriteLine("Debug messages...");
        }
    }

    private static Tree<int> CreateTree(int rootValue, List<Tuple<int, int>> links, List<int> gateways)
    {
        Tree<int> tree = new Tree<int>(rootValue);

        List<Tuple<int, int>> addedLinks = new List<Tuple<int, int>>();
        Queue<Node<int>> nodesToBeVisited = new Queue<Node<int>>(new[] { tree.Root });

        while (nodesToBeVisited.Count > 0)
        {
            Node<int> node = nodesToBeVisited.Dequeue();
            foreach (Tuple<int, int> link in links)
            {
                if (link.Item1 == node.Value && !gateways.Contains(link.Item2))
                {
                    if (!addedLinks.Contains(link))
                    {
                        Node<int> childNode = node.AddChild(link.Item2);
                        nodesToBeVisited.Enqueue(childNode);
                        addedLinks.Add(link);
                    }
                }
                else if (link.Item2 == node.Value && !gateways.Contains(link.Item1))
                {
                    if (!addedLinks.Contains(link))
                    {
                        Node<int> childNode = node.AddChild(link.Item1);
                        nodesToBeVisited.Enqueue(childNode);
                        addedLinks.Add(link);
                    }
                }
            }
        }

        return tree;
    }

    private static void SeverNode(List<Tree<int>> gatewayTrees, int parentNodeValue, int infectedNodeValue)
    {
        foreach (Tree<int> gatewayTree in gatewayTrees)
        {
            Queue<Node<int>> nodesToBeVisited = new Queue<Node<int>>(new[] { gatewayTree.Root });

            while (nodesToBeVisited.Count > 0)
            {
                Node<int> node = nodesToBeVisited.Dequeue();

                for (int i = node.Children.Count - 1; i >= 0; i--)
                {
                    if (node.Value == parentNodeValue && node.Children[i].Value == infectedNodeValue)
                    {
                        {
                            node.Children.RemoveAt(i);
                        }
                    }
                    else
                    {
                        nodesToBeVisited.Enqueue(node.Children[i]);
                    }
                }
            }
        }
    }

    private static Node<int> FindInfectedNode(Node<int> rootNode, int infectedNodeValue)
    {
        Queue<Node<int>> nodesToBeVisited = new Queue<Node<int>>(new[] { rootNode });

        while (nodesToBeVisited.Count > 0)
        {
            Node<int> node = nodesToBeVisited.Dequeue();
            foreach (Node<int> child in node.Children)
            {
                if (child.Value == infectedNodeValue)
                {
                    {
                        return child;
                    }
                }
                else
                {
                    nodesToBeVisited.Enqueue(child);
                }
            }
        }

        return null;
    }
}