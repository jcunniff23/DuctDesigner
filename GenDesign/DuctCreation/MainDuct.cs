using Autodesk.Revit.DB;
using Autodesk.Revit.UI;


namespace GenDesign.DuctCreation;

public class MainDuct
{
    public MainDuct(List<Element> ductElements)
    {

    }





}

internal class DuctSegment
{

    public XYZ Start;
    public XYZ End;
    public Line MainAxis => Line.CreateBound(Start, End);


    internal DuctSegment(XYZ start, XYZ end)
    {
        this.start = start;
        this.end = end;
    }
}
