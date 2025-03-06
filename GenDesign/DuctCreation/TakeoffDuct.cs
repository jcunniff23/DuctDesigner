using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace GenDesign.DuctCreation;

    public class TakeoffDuct
    {
        //public double 
        public bool StraightRun { get; set; }
        public List<XYZ> MidPoints { get; set; }
        public XYZ StartPoint { get; set; }
        public TakeoffDuct(XYZ startPoint, XYZ terminalPosition, List<XYZ> midPoints)
        {
            MidPoints = midPoints;
            StartPoint = startPoint;
            

        }


    }

