 // See https://aka.ms/new-console-template for more information
using GenDesign;
using GenDesign.DataStructures;



Console.WriteLine("Hello, World!");

Node start = new Node(0, 0, 0, true);
Node end = new Node(75, 75, 0, true);

var solver = new Solver(start, end, 0.995, 1000, 50);
var path = solver.Run();
Console.WriteLine(path);