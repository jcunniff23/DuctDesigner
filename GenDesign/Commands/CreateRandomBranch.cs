using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using GenDesign.DuctCreation;
using GenDesign.Filters;
using GenDesign.Optimize;

namespace GenDesign.Commands;

[Transaction(TransactionMode.Manual)]
public class CreateRandomBranch : IExternalCommand
{

    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elementSet)
    {

        //document boilerplate
        UIDocument uiDoc = commandData.Application.ActiveUIDocument;
        Document doc = uiDoc.Document;



        //create anneal object
        int maxPoints = 4;
        Anneal solver = new Anneal(0.02, 500, 50, maxPoints);


        //duct selection
        //terminal selection
        List<Element> SelectedDuct = new List<Element>();
        List<Element> SelectedTerminal = new List<Element>();

        TaskDialog.Show("CreateRandomBranch", "NOW SELECT A DUCT");

        try
        {
            IList<Reference> selectedRef = uiDoc.Selection.PickObjects(ObjectType.Element, new DuctSelectionFilter(), "SELECT DUCTS PLEASE");
            var ductIds = selectedRef.Select(r => r.ElementId).ToList();
            var duct = ductIds.Select(id => uiDoc.Document.GetElement(id)).ToList();
            SelectedDuct.AddRange(duct);
        }
        catch (Exception e)
        {

            TaskDialog.Show("CreateRandomBranch", $"ERROR: {e.Message}");
            throw;

        }

        TaskDialog.Show("CreateRandomBranch", "NOW SELECT A TERMINAL");

        try
        {
            Reference selectedRef = uiDoc.Selection.PickObject(ObjectType.Element, new DuctSelectionFilter(), "SELECT DUCTS PLEASE");
            var terminal = uiDoc.Document.GetElement(selectedRef.ElementId);
            SelectedTerminal.Add(terminal);
        }
        catch (Exception e)
        {

            TaskDialog.Show("CreateRandomBranch", $"ERROR: {e.Message}");
            throw;

        }

        
        //set up anneal obj
        Element ductElement = SelectedDuct[^1];
        Element terminalElement = SelectedTerminal[^1];

        LocationPoint terminalPos = (LocationPoint)terminalElement.Location;

        LocationCurve ductLocCurve = (LocationCurve)ductElement.Location;
        Curve ductCurve = ductLocCurve.Curve;
        DuctSegment segment = new DuctSegment(ductCurve.GetEndPoint(0), ductCurve.GetEndPoint(1));

        TakeoffDuct takeoff = solver.InitialSolution(segment, terminalPos.Point);

        //transaction block for creating placeholder ducts

        using (Transaction transaction = new Transaction(doc))
        {

            transaction.Start();
            var takeoffTap = Duct.CreatePlaceholder(doc, , ductElement.GetTypeId(), ductElement.LevelId, 
                                                    takeoff.StartPoint, takeoff.MidPoints[0]);

            for (int i = 0; i < takeoff.MidPoints.Count; i++)
            {
                var ductLine = Duct.CreatePlaceholder(doc, , ductElement.GetTypeId(), ductElement.LevelId,
                                                    takeoff.StartPoint, takeoff.MidPoints[0]);

            }



        }



            return Result.Succeeded;    
    }



}
