using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;

namespace GenDesign;

[Transaction(TransactionMode.Manual)]
public class Main : IExternalCommand
{

    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elementSet)
    {
        return Result.Succeeded;
    }





}

