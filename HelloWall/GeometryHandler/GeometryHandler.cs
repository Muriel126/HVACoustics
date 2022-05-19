using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.Common.Geometry;
using Xbim.Ifc;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.ProductExtension;
using Xbim.Ifc4.ProfileResource;
using Xbim.ModelGeometry.Scene;

namespace HVACoustics
{
    class GeometryHandler
    {

        public void GetMeasurementsSweptSolid(IfcProduct space)
        {
            var rep = space.Representation.Representations;
            var shape = rep[0];
            var body = shape.Items[0];
            double X = ((IfcExtrudedAreaSolid)body).Position.Location.X;
            double Y = ((IfcExtrudedAreaSolid)body).Position.Location.Y;
            double XDim = ((IfcRectangleProfileDef)((IfcExtrudedAreaSolid)body).SweptArea).XDim;
            double YDim = ((IfcRectangleProfileDef)((IfcExtrudedAreaSolid)body).SweptArea).YDim;
        }

        public Enums.TypeDirec GetDirection(double X1, double X2, double Y1, double Y2)
        {
            if (Y2 > Y1)
            {
                return Enums.TypeDirec.Ypos;
            }
            if (Y1 > Y2)
            {
                return Enums.TypeDirec.Yneg;
            }
            if (X2 > X1)
            {
                return Enums.TypeDirec.Xpos;
            }
            if (X1 > X2)
            {
                return Enums.TypeDirec.Xneg;
            }
            return Enums.TypeDirec.sameCor;
        }
        public XbimRect3D CreateBoundingBoxAroundSpace(IfcStore model, IIfcSpace space)
        {
            //https://github.com/xBimTeam/XbimAnalysis/blob/develop/Xbim.Analysis/Spatial/XbimAABBoxAnalyser.cs
            Xbim3DModelContext context = new Xbim3DModelContext(model);
            context.CreateContext();

            XbimRect3D prodBox = XbimRect3D.Empty;
            foreach (var shp in context.ShapeInstancesOf(space))
            {
                //bounding boxes are lightweight and are produced when geometry is created at first place

                //get or cast to BBox
                var bb = shp.BoundingBox;
                bb = XbimRect3D.TransformBy(bb, shp.Transformation);
                if (prodBox.IsEmpty) prodBox = bb; else prodBox.Union(bb);

                //add every BBox to the world to get the size and position of the world
            }
            return prodBox;
        }
    }

    
    /* List<double> cornerX1 = new List<double>();
 cornerX1.Add(X1);
 cornerX1.Add(X1+XDim1);
 List<double> cornerY1 = new List<double>();
 cornerY1.Add(Y1);
 cornerY1.Add(Y1+YDim1);
 var minValX1 = cornerX1.Min();
 var minValY1 = cornerY1.Min();
 //Console.WriteLine(minValX1);*/

    /*  List<Vector2d> listVec = new List<Vector2d>();
      Vector2d v = new Vector2d(X1, Y1);
      Vector2d v2 = new Vector2d(X1 + XDim1, Y1);
      Vector2d v3 = new Vector2d(X1 + XDim1, Y1 + YDim1);
      Vector2d v4 = new Vector2d(X1, Y1 + YDim1);
      listVec.Add(v);
      listVec.Add(v2);
      listVec.Add(v3);
      listVec.Add(v4);
      Polygon2d poly1 = new Polygon2d(listVec); */


    /* List<double> cornerX2 = new List<double>();
     cornerX2.Add(X2);
     cornerX2.Add(X2 + XDim2);
     List<double> cornerY2 = new List<double>();
     cornerY2.Add(Y2);
     cornerY2.Add(Y2 + YDim2);
     var minValX2 = cornerX2.Min();
     var minValY2 = cornerY2.Min();
     //Console.WriteLine(minValX2); */
}
