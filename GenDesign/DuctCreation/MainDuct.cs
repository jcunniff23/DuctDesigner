using Autodesk.Revit.DB;
using Autodesk.Revit.UI;


namespace GenDesign.DuctCreation;

public class MainDuct
{
    public MainDuct(List<Element> ductElements)
    {

    }





}

public class DuctSegment
{

    public XYZ Start;
    public XYZ End;
    public Line MainAxis => Line.CreateBound(Start, End);


    public DuctSegment(XYZ start, XYZ end)
    {
        this.Start = start;
        this.End = end;
    }
}
