using System;
class P
{
    static void Main(string[] args)
    {
        var inputs = Console.ReadLine().Split(' ');
        int exitFloor = int.Parse(inputs[3]); // floor on which the exit is found
        int exitPos = int.Parse(inputs[4]); // position of the exit on its floor
        int nbElevators = int.Parse(inputs[7]); // number of elevators
        var elevatorPoss = new int[nbElevators + 1];
        for (int i = 0; i < nbElevators; i++)
        {
            inputs = Console.ReadLine().Split(' ');
            int elevatorFloor = int.Parse(inputs[0]); // floor on which this elevator is found
            int elevatorPos = int.Parse(inputs[1]); // position of the elevator on its floor
            elevatorPoss[elevatorFloor] = elevatorPos;
        }

        // game loop
        while (true)
        {
            inputs = Console.ReadLine().Split(' ');
            int cloneFloor = int.Parse(inputs[0]); // floor of the leading clone
            int clonePos = int.Parse(inputs[1]); // position of the leading clone on its floor
            var direction = inputs[2]; // direction of the leading clone: LEFT or RIGHT

            // Write an action using Console.WriteLine()
            // To debug: Console.Error.WriteLine("Debug messages...");

            Console.WriteLine(direction == "NONE" ? "WAIT" : direction == "LEFT" && ((cloneFloor == exitFloor ? exitPos : elevatorPoss[cloneFloor]) > clonePos) || direction == "RIGHT" && ((cloneFloor == exitFloor ? exitPos : elevatorPoss[cloneFloor]) < clonePos) ? "BLOCK" : "WAIT");
        }
    }
}