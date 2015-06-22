using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

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
            var parent = this.Parent;
            var i = 0;
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
        List<Tree<int>> gatewayTrees = new List<Tree<int>>();

        string[] inputs;
        inputs = Console.ReadLine().Split(' ');
        int N = int.Parse(inputs[0]); // the total number of nodes in the level, including the gateways
        int L = int.Parse(inputs[1]); // the number of links
        int E = int.Parse(inputs[2]); // the number of exit gateways
        List<Tuple<int, int>> links = new List<Tuple<int, int>>();
        for (int i = 0; i < L; i++)
        {
            inputs = Console.ReadLine().Split(' ');
            links.Add(new Tuple<int, int>(int.Parse(inputs[0]), int.Parse(inputs[1])));
        }
        for (int i = 0; i < E; i++)
        {
            int EI = int.Parse(Console.ReadLine()); // the index of a gateway node
            gatewayTrees.Add(new Tree<int>(EI));
        }

        while (true)
        {
            foreach (var tuple in links)
            {
                if (tuple.Item1 == x)
                {
                    gatewayTrees.First(x => x.Root.Value == tuple.Item1).Root.AddChild(x)
                }
                else if (tuple.Item2 == x)
                {
                     
                }
            }
        }

        // game loop
        while (true)
        {
            int SI = int.Parse(Console.ReadLine()); // The index of the node on which the Skynet agent is positioned this turn

            // Write an action using Console.WriteLine()
            // To debug: Console.Error.WriteLine("Debug messages...");

            Console.WriteLine("0 1"); // Example: 0 1 are the indices of the nodes you wish to sever the link between
        }
    }
}