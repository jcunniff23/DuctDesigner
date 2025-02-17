using Autodesk.Revit.DB;

namespace GenDesign.DataStructures;

public class NodeFactory
{

    //Factory class responsible for converting Revit XYZ -> to generic coordinate system of nodes
    public static Node GetNode(XYZ position)
    {
        Node node = null;
        node = new Node(position.X, position.Y, position.Z, true);


        return node;
    }
}