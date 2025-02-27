using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace GenDesign.DuctCreation;

    internal class TakeoffDuct
    {
        //public double 
        public bool StraightRun { get; set; }

        public TakeoffDuct(XYZ startPoint, XYZ terminalPosition, List<XYZ> midPoints)
        {
            //create on level position equivalent to terminal XZ



        }


    }

