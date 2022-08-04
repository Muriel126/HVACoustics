using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.Ifc;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.ProductExtension;
using Xbim.Ifc4.ProfileResource;
using g3;
using Xbim.Common;
using Xbim.Common.Collections;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.SharedBldgElements;
using Xbim.ModelGeometry.Scene;
using Xbim.Common.Geometry;
using System.IO;
using Xbim.Common.XbimExtensions;
using NestedDictionaryLib;

namespace HVACoustics
{
    class RoomConfiguration
    {
        private static double tolerance = 0.5;
        private static double eqWallThick = 0.4;

        public static double X1 { get; set; }
        public static double Y1 { get; set; }
        public static double X1rounded { get; set; }
        public static double Y1rounded { get; set; }
        public static double XDim1 { get; set; }
        public static double YDim1 { get; set; }
        public static double X1EndRounded { get; set; }
        public static double Y1EndRounded { get; set; }

        public static double X2 { get; set; }
        public static double Y2 { get; set; }
        public static double X2rounded { get; set; }
        public static double Y2rounded { get; set; }
        public static double XDim2 { get; set; }
        public static double YDim2 { get; set; }
        public static double X2EndRounded { get; set; }
        public static double Y2EndRounded { get; set; }

        public static string globalIdStorey1 { get; set; }
        public static string globalIdStorey2 { get; set; }
        public static int indexStorey1 { get; set; }
        public static int indexStorey2 { get; set; }
        public static int indexSpaceX1 { get; set; }
        public static int indexSpaceY1 { get; set; }
        public static int indexSpaceX2 { get; set; }
        public static int indexSpaceY2 { get; set; }
        public static Enums.TypeOfPoint typeOfPoint { get; set; }

        public void GetSpecificGeometrySpace(IfcStore model, string senderID, string recieverID)
        {
            var geo = new GeometryHandler();

            IIfcSpace sender = model.Instances.FirstOrDefault<IIfcSpace>(d => d.GlobalId == senderID);
            IIfcSpace reciever = model.Instances.FirstOrDefault<IIfcSpace>(d => d.GlobalId == recieverID);

            XbimRect3D prodBox1 = geo.CreateBoundingBoxAroundSpace(model, sender);
            XbimRect3D prodBox2 = geo.CreateBoundingBoxAroundSpace(model, reciever);
            X1 = Math.Round(prodBox1.Location.X, 2);
            X1rounded = Math.Round(prodBox1.Location.X, 0);
            Y1 = Math.Round(prodBox1.Location.Y, 2);
            Y1rounded = Math.Round(prodBox1.Location.Y, 0);
            X2 = Math.Round(prodBox2.Location.X, 2);
            X2rounded = Math.Round(prodBox2.Location.X, 0);
            Y2 = Math.Round(prodBox2.Location.Y, 2);
            Y2rounded = Math.Round(prodBox2.Location.Y, 0);

            var rep1 = sender.Representation.Representations;
            var shape1 = rep1[0];
            var body1 = shape1.Items[0];

            XDim1 = Math.Round(((IIfcRectangleProfileDef)((IIfcExtrudedAreaSolid)body1).SweptArea).XDim, 2);
            YDim1 = Math.Round(((IIfcRectangleProfileDef)((IIfcExtrudedAreaSolid)body1).SweptArea).YDim, 2);

            var rep2 = reciever.Representation.Representations;
            var shape2 = rep2[0];
            var body2 = shape2.Items[0];

            XDim2 = Math.Round(((IIfcRectangleProfileDef)((IIfcExtrudedAreaSolid)body2).SweptArea).XDim, 2);
            YDim2 = Math.Round(((IIfcRectangleProfileDef)((IIfcExtrudedAreaSolid)body2).SweptArea).YDim, 2);

            X1EndRounded = X1rounded + Math.Round(XDim1, 0);
            Y1EndRounded = Y1rounded + Math.Round(YDim1, 0);
            X2EndRounded = X2rounded + Math.Round(XDim2, 0);
            Y2EndRounded = Y2rounded + Math.Round(YDim2, 0);

            /*Console.WriteLine(direcX1);
            Console.WriteLine(direcY1);
            Console.WriteLine(direcX2);
            Console.WriteLine(direcY2);

            Console.WriteLine(X1);
            Console.WriteLine(Y1);
            Console.WriteLine(XDim1);
            Console.WriteLine(YDim1);
            Console.WriteLine(X2);
            Console.WriteLine(Y2);
            Console.WriteLine(XDim2);
            Console.WriteLine(YDim2);*/
        }

        public static List<double> GetRoomsAlongAxis(Enums.TypeDirec typeDirec, Enums.TypeOfPoint typeOfPoint, NestedDictionary<string, double, double> dictCoors, 
            NestedDictionary<string, double, double> dictCoorsWithXDim, NestedDictionary<string, double, double> dictCoorsWithYDim , double X1rounded, double Y1rounded)
        {
            List<double> listCoorsAlongAxis = new List<double>();

            if (typeOfPoint == Enums.TypeOfPoint.start)
            {
                if (typeDirec == Enums.TypeDirec.Xpos || typeDirec == Enums.TypeDirec.Xneg)
                {
                    foreach (KeyValuePair<string, NestedDictionary<double, double>> kvp1 in dictCoors)
                    {
                        foreach (KeyValuePair<double, double> kvp2 in kvp1.Value)
                        {
                            if (kvp2.Value == Y1rounded)
                            {
                                listCoorsAlongAxis.Add(kvp2.Key);
                            }
                        }
                    }
                }
                if (typeDirec == Enums.TypeDirec.Ypos || typeDirec == Enums.TypeDirec.Yneg)
                {
                    foreach (KeyValuePair<string, NestedDictionary<double, double>> kvp1 in dictCoors)
                    {
                        foreach (KeyValuePair<double, double> kvp2 in kvp1.Value)
                        {
                            if (kvp2.Key == X1rounded)
                            {
                                listCoorsAlongAxis.Add(kvp2.Value);
                            }
                        }
                    }
                }
            }
            if (typeOfPoint == Enums.TypeOfPoint.end)
            {
                if (typeDirec == Enums.TypeDirec.Xpos || typeDirec == Enums.TypeDirec.Xneg)
                {
                    foreach (KeyValuePair<string, NestedDictionary<double, double>> kvp1 in dictCoorsWithYDim)
                    {
                        foreach (KeyValuePair<double, double> kvp2 in kvp1.Value)
                        {
                            if (kvp2.Value == Y1EndRounded)
                            {
                                listCoorsAlongAxis.Add(kvp2.Key);
                            }
                        }
                    }
                }
                if (typeDirec == Enums.TypeDirec.Ypos || typeDirec == Enums.TypeDirec.Yneg)
                {
                    foreach (KeyValuePair<string, NestedDictionary<double, double>> kvp1 in dictCoorsWithXDim)
                    {
                        foreach (KeyValuePair<double, double> kvp2 in kvp1.Value)
                        {
                            
                            if (kvp2.Key == X1EndRounded)
                            {
                                listCoorsAlongAxis.Add(kvp2.Value);
                            }
                        }
                    }
                }
            }
            return listCoorsAlongAxis;
        }

        public static Enums.TypeRoomConfig GetRoomConfiguration(IfcStore model, string senderID, string recieverID)
        {
            var geo = new GeometryHandler();
            var config = new RoomConfiguration();
            var helper = new Helper();

            List<double> listCoorsInOffsetXDirec = new List<double>();
            NestedDictionary<string,double,double> dictCoors = new NestedDictionary<string, double, double>();
            NestedDictionary<string, double, double> dictCoorsWithXDim = new NestedDictionary<string, double, double>();
            NestedDictionary<string, double, double> dictCoorsWithYDim = new NestedDictionary<string, double, double>();


            var building = model.Instances.FirstOrDefault<IIfcBuilding>();
            //first get sender and reciever space
            IIfcSpace sender = model.Instances.FirstOrDefault<IIfcSpace>(d => d.GlobalId == senderID);
            IIfcSpace reciever = model.Instances.FirstOrDefault<IIfcSpace>(d => d.GlobalId == recieverID);

            //next get the storey each space is located in
            var storey1 = sender.Decomposes.Select(x => x.RelatingObject as IIfcBuildingStorey);
            foreach (var element in storey1)
            {
                var listStoreys = model.Instances.OfType<IIfcBuildingStorey>().ToList();
                globalIdStorey1 = element.GlobalId;
                //get the spaces of the elements and sort the X- and Y-dim of their corners in a list to check if the rooms are in first or a higher dimension to each other later on
                var listRooms = element.Spaces;
                foreach (var v in listRooms)
                {
                    var bbSpace = geo.CreateBoundingBoxAroundSpace(model, v);
                    dictCoors.Add(v.GlobalId, Math.Round(bbSpace.X, 0), Math.Round(bbSpace.Y, 0));
                    dictCoorsWithXDim.Add(v.GlobalId, Math.Round(bbSpace.X, 0) + Math.Round(bbSpace.SizeX,0), Math.Round(bbSpace.Y, 0));
                    dictCoorsWithYDim.Add(v.GlobalId, Math.Round(bbSpace.X, 0) , Math.Round(bbSpace.Y, 0) + Math.Round(bbSpace.SizeY, 0));
                }

                //Helper.PrintNestedDictionary(dictCoors);
                foreach (var x in listCoorsInOffsetXDirec)
                {
                    Console.WriteLine("list X1 {0}", x);
                }
                indexStorey1 = listStoreys.IndexOf(element);
            }
            
            var storey2 = reciever.Decomposes.Select(x => x.RelatingObject as IIfcBuildingStorey);
            foreach (var element in storey2)
            {
                var listStoreys = model.Instances.OfType<IIfcBuildingStorey>().ToList();
                globalIdStorey2 = element.GlobalId;
                
                indexStorey2 = listStoreys.IndexOf(element);
            }

            config.GetSpecificGeometrySpace(model, senderID, recieverID);

            Enums.TypeRoomConfig roomConfig = new Enums.TypeRoomConfig();

            if (indexStorey1 +2 == indexStorey2 || indexStorey1 == indexStorey2 + 2)
            {
                roomConfig = config.TryVerticalConfigsSecondDimension(X1, Y1, XDim1, YDim1, X2, Y2, XDim2, YDim2);
            }
            else if (indexStorey1 + 3 == indexStorey2 || indexStorey1 == indexStorey2 + 3)
            {
                roomConfig = config.TryVerticalConfigsThirdimension(X1, Y1, XDim1, YDim1, X2, Y2, XDim2, YDim2);
            }
            else if (globalIdStorey1 == globalIdStorey2)
            {
                roomConfig = config.TryHorizontalConfigs(X1, Y1, XDim1, YDim1, X2, Y2, XDim2, YDim2, dictCoors, dictCoorsWithXDim, dictCoorsWithYDim);
            }
            else if (globalIdStorey1 != globalIdStorey2)
            {
                roomConfig = config.TryVerticalConfigsFirstDimension(X1, Y1, XDim1, YDim1, X2, Y2, XDim2, YDim2);
            }

            return roomConfig;
        }

        public Enums.TypeRoomConfig TryHorizontalConfigs(double X1, double Y1, double XDim1, double YDim1, double X2, double Y2, double XDim2, double YDim2, 
            NestedDictionary<string, double, double> dictCoors, NestedDictionary<string, double, double> dictCoorsWithXDim, NestedDictionary<string, double, double> dictCoorsWithYDim)
        {
            var geo = new GeometryHandler();
            Enums.TypeRoomConfig roomConfig = new Enums.TypeRoomConfig();
            List<double> listCoorsAlongAxis = new List<double>();
            var direcX = geo.GetDirection(X1, X2, 0, 0);
            var direcY = geo.GetDirection(0, 0, Y1, Y2);

            if (X1 - tolerance < X2 && X1 + tolerance > X2 || X2 - tolerance < X1 && X2 + tolerance > X1) //==
            {              
                if (direcY == Enums.TypeDirec.Ypos)
                {  
                    if ((Y1 + YDim1 + eqWallThick) > Y2)
                    {
                        //Raum 1. Ord.
                        if (XDim1 + tolerance == XDim2 || XDim1 == XDim2 + tolerance)
                        {
                            return Enums.TypeRoomConfig.hor;
                        }
                        else
                        {
                            return Enums.TypeRoomConfig.horNegOff;
                        }
                    }
                    else
                    {
                        //Raum höherer Ord.
                        listCoorsAlongAxis = GetRoomsAlongAxis(direcX, Enums.TypeOfPoint.start, dictCoors, dictCoorsWithXDim, dictCoorsWithYDim, X1rounded, Y1rounded);
                        indexSpaceY1 = listCoorsAlongAxis.IndexOf(Y1rounded);
                        indexSpaceY2 = listCoorsAlongAxis.IndexOf(Y2rounded);

                        if (indexSpaceY1 + 2 == indexSpaceY2)
                        {
                            return Enums.TypeRoomConfig.hor1Room;
                        }
                        if (indexSpaceY1 + 3 == indexSpaceY2)
                        {
                            return Enums.TypeRoomConfig.hor2Room;
                        }
                    }
                }

                if (direcY == Enums.TypeDirec.Yneg)
                {
                    if ((Y2 + YDim2 + eqWallThick) > Y1)
                    {
                        //Raum 1. Ord.
                        if (XDim1 + tolerance == XDim2 || XDim1 == XDim2 + tolerance)
                        {
                            return Enums.TypeRoomConfig.hor;
                        }
                        else
                        {
                            return Enums.TypeRoomConfig.horNegOff;
                        }
                    }
                    else
                    {
                        //Raum höherer Ord.
                        listCoorsAlongAxis = GetRoomsAlongAxis(direcX, Enums.TypeOfPoint.start, dictCoors, dictCoorsWithXDim, dictCoorsWithYDim, X1rounded, Y1rounded);
                        indexSpaceY1 = listCoorsAlongAxis.IndexOf(Y1rounded);
                        indexSpaceY2 = listCoorsAlongAxis.IndexOf(Y2rounded);

                        if (indexSpaceY1 == indexSpaceY2 + 2)
                        {
                            return Enums.TypeRoomConfig.hor1Room;
                        }
                        if (indexSpaceY1 == indexSpaceY2 + 3)
                        {
                            return Enums.TypeRoomConfig.hor2Room;
                        }
                    }
                }
            }
            else if (X1 + XDim1 - tolerance < X2 + XDim2 && X1 + XDim1 + tolerance > X2 + XDim2 || X2 + XDim2 - tolerance < X1 + XDim1 && X2 + XDim2 + tolerance > X1 + XDim1) //gleicher Endpunkt
            {
                //bedeutet dass X1 != X2, sonst wäre es in erste Abfrage gegangen und daher haben Räume unterschiedliche Länge
                if (direcY == Enums.TypeDirec.Ypos)
                {
                    if ((Y1 + YDim1 + eqWallThick) > Y2 || Y1 - tolerance < Y2 && Y1 + tolerance > Y2 || Y2 - tolerance < Y1 && Y2 + tolerance > Y1)
                    {
                        //Raum 1. Ord.
                        return Enums.TypeRoomConfig.horNegOff;
                    }
                    else
                    {
                        //Raum höherer Ord.
                        listCoorsAlongAxis = GetRoomsAlongAxis(direcY, Enums.TypeOfPoint.end, dictCoors, dictCoorsWithXDim, dictCoorsWithYDim, X1EndRounded, Y1EndRounded);
                        indexSpaceY1 = listCoorsAlongAxis.IndexOf(Y1rounded);
                        indexSpaceY2 = listCoorsAlongAxis.IndexOf(Y2rounded);

                        if (indexSpaceY1 + 2 == indexSpaceY2)
                        {
                            return Enums.TypeRoomConfig.hor1Room;
                        }
                        if (indexSpaceY1 + 3 == indexSpaceY2)
                        {
                            return Enums.TypeRoomConfig.hor2Room;
                        }
                    }
                }
                if (direcY == Enums.TypeDirec.Yneg)
                {
                    if ((Y2 + YDim2 + eqWallThick) > Y1)
                    {
                        //Raum 1. Ord.
                        return Enums.TypeRoomConfig.horNegOff;
                    }
                    else
                    {
                        //Raum höherer Ord.
                        listCoorsAlongAxis = GetRoomsAlongAxis(direcX, Enums.TypeOfPoint.end, dictCoors, dictCoorsWithXDim, dictCoorsWithYDim, X1EndRounded, Y1EndRounded);
                        indexSpaceY1 = listCoorsAlongAxis.IndexOf(Y1rounded);
                        indexSpaceY2 = listCoorsAlongAxis.IndexOf(Y2rounded);

                        if (indexSpaceY1 == indexSpaceY2 + 2)
                        {
                            return Enums.TypeRoomConfig.hor1Room;
                        }
                        if (indexSpaceY1 == indexSpaceY2 + 3)
                        {
                            return Enums.TypeRoomConfig.hor2Room;
                        }
                    }
                }
            }
            else if (Y1 + YDim1 - tolerance < Y2 + YDim2 && Y1 + YDim1 + tolerance > Y2 + YDim2 || Y2 + YDim2 - tolerance < Y1 + YDim1 && Y2 + YDim2 + tolerance > Y1 + YDim1) //gleicher Endpunkt
            {
                //bedeutet dass X1 != X2 sonst wäre es in erste Abfrage gegangen und daher haben Räume unterschiedliche Länge
                if (direcX == Enums.TypeDirec.Xpos)
                {
                    if ((X1 + XDim1 + eqWallThick) > X2)
                    {
                        //Raum 1. Ord.
                        return Enums.TypeRoomConfig.horNegOff;
                    }
                    else
                    {

                        //Raum höherer Ord.
                        listCoorsAlongAxis = GetRoomsAlongAxis(direcX, Enums.TypeOfPoint.end, dictCoors, dictCoorsWithXDim, dictCoorsWithYDim, X1EndRounded, Y1EndRounded);
                        indexSpaceX1 = listCoorsAlongAxis.IndexOf(X1rounded);
                        indexSpaceX2 = listCoorsAlongAxis.IndexOf(X2rounded);

                        if (indexSpaceX1 + 2 == indexSpaceX2)
                        {
                            return Enums.TypeRoomConfig.hor1Room;
                        }
                        if (indexSpaceX1 + 3 == indexSpaceX2)
                        {
                            return Enums.TypeRoomConfig.hor2Room;
                        }
                    }
                }

                if (direcX == Enums.TypeDirec.Xneg)
                {
                    if ((X2 + XDim2 + eqWallThick) > X1)
                    {
                        //Raum 1. Ord.
                        return Enums.TypeRoomConfig.horNegOff;
                    }
                    else
                    {
                        //Raum höherer Ord.
                        listCoorsAlongAxis = GetRoomsAlongAxis(direcX, Enums.TypeOfPoint.end, dictCoors, dictCoorsWithXDim, dictCoorsWithYDim, X1EndRounded, Y1EndRounded);
                        indexSpaceX1 = listCoorsAlongAxis.IndexOf(X1rounded);
                        indexSpaceX2 = listCoorsAlongAxis.IndexOf(X2rounded);

                        if (indexSpaceX1 == indexSpaceX2 + 2)
                        {
                            return Enums.TypeRoomConfig.hor1Room;
                        }
                        if (indexSpaceX1 == indexSpaceX2 + 3)
                        {
                            return Enums.TypeRoomConfig.hor2Room;
                        }
                    }
                }
            }
            else if (X1 != X2)
            {
                if (Y1 - tolerance < Y2 && Y1 + tolerance > Y2 || Y2 - tolerance < Y1 && Y2 + tolerance > Y1) //==
                {
                    if (direcX == Enums.TypeDirec.Xpos)
                    {
                        if ((X1 + XDim1 + eqWallThick) > X2)
                        {
                            //Raum 1. Ord.
                            if (XDim1 + tolerance == XDim2 || XDim1 == XDim2 + tolerance)
                            {
                                return Enums.TypeRoomConfig.hor;
                            }
                            else
                            {
                                return Enums.TypeRoomConfig.horNegOff;
                            }
                        }
                        else
                        {
                            //Raum höherer Ord.
                            listCoorsAlongAxis = GetRoomsAlongAxis(direcX, Enums.TypeOfPoint.start, dictCoors,dictCoorsWithXDim, dictCoorsWithYDim, X1rounded, Y1rounded);
                            indexSpaceX1 = listCoorsAlongAxis.IndexOf(X1rounded);
                            indexSpaceX2 = listCoorsAlongAxis.IndexOf(X2rounded);

                            if (indexSpaceX1 + 2 == indexSpaceX2)
                            {
                                return Enums.TypeRoomConfig.hor1Room;
                            }
                            if (indexSpaceX1 + 3 == indexSpaceX2)
                            {
                                return Enums.TypeRoomConfig.hor2Room;
                            }
                        }
                    }
                    else if (direcX == Enums.TypeDirec.Xneg)
                    {
                        if ((X2 + XDim2 + eqWallThick) > X1)
                        {
                            //Raum 1. Ord.
                            if (XDim1 + tolerance == XDim2 || XDim1 == XDim2 + tolerance)
                            {
                                return Enums.TypeRoomConfig.hor;
                            }
                            else
                            {
                                return Enums.TypeRoomConfig.horNegOff;
                            }
                        }
                        else
                        {
                            //Raum höherer Ord.
                            listCoorsAlongAxis = GetRoomsAlongAxis(direcX, Enums.TypeOfPoint.start, dictCoors, dictCoorsWithXDim, dictCoorsWithYDim, X1rounded, Y1rounded);
                            indexSpaceX1 = listCoorsAlongAxis.IndexOf(X1rounded);
                            indexSpaceX2 = listCoorsAlongAxis.IndexOf(X2rounded);

                            if (indexSpaceX1 == indexSpaceX2 + 2)
                            {
                                return Enums.TypeRoomConfig.hor1Room;
                            }
                            if (indexSpaceX1 == indexSpaceX2 + 3)
                            {
                                return Enums.TypeRoomConfig.hor2Room;
                            }
                        }
                    }
                }
                else if (Y1 != Y2)
                {
                    if (direcX == Enums.TypeDirec.Xpos && direcY == Enums.TypeDirec.Ypos)
                    {
                        if (((Y1 + YDim1 + eqWallThick) > Y2) && ((Y1 + YDim1) < Y2) && ((X1 + XDim1 + eqWallThick) > X2) && ((X1 + XDim1) < X2))
                        {
                            return Enums.TypeRoomConfig.horOff;
                        }
                        else if (((X1 + XDim1 + eqWallThick) > X2) && ((X1 + XDim1) < X2))
                        {
                            if (Y1 + YDim1 + eqWallThick < Y2)  // || Y2 + YDim2 + eqWallThick < Y1)
                            {
                                return Enums.TypeRoomConfig.horPosOff;
                            }
                            else
                            {
                                return Enums.TypeRoomConfig.horNegOff;
                            }
                        }
                        else if (((Y1 + YDim1 + eqWallThick) > Y2) && ((Y1 + YDim1) < Y2))
                        {
                            if (X1 + XDim1 + eqWallThick < X2)  // || X2 + XDim2 + eqWallThick < X1)
                            {
                                return Enums.TypeRoomConfig.horPosOff;
                            }
                            else
                            {
                                return Enums.TypeRoomConfig.horNegOff;
                            }
                        }
                    }
                    else if (direcX == Enums.TypeDirec.Xpos && direcY == Enums.TypeDirec.Yneg)
                    {
                        if (((Y2 + YDim2 + eqWallThick) > Y1) && ((Y2 + YDim2) < Y1) && ((X1 + XDim1 + eqWallThick) > X2) && ((X1 + XDim1) < X2))
                        {
                            return Enums.TypeRoomConfig.horOff;
                        }
                        else if ((X1 + XDim1 + eqWallThick) > X2 && ((X1 + XDim1) < X2))
                        {
                            if (Y2 + YDim2 + eqWallThick < Y1)
                            {
                                return Enums.TypeRoomConfig.horPosOff;
                            }
                            else
                            {
                                return Enums.TypeRoomConfig.horNegOff;
                            }
                        }
                        else if ((Y2 + YDim2 + eqWallThick) > Y1 && ((Y2 + YDim2) < Y1))
                        {
                            if (X1 + XDim1 + eqWallThick < X2) 
                            {
                                return Enums.TypeRoomConfig.horPosOff;
                            }
                            else
                            {
                                return Enums.TypeRoomConfig.horNegOff;
                            }
                        }
                    }
                    else if (direcX == Enums.TypeDirec.Xneg && direcY == Enums.TypeDirec.Ypos)
                    {
                        if (((Y1 + YDim1 + eqWallThick) > Y2) && ((Y1 + YDim1) < Y2) && ((X2 + XDim2 + eqWallThick) > X1) && ((X2 + XDim2) < X1))
                        {
                            return Enums.TypeRoomConfig.horOff;
                        }
                        else if (((X2 + XDim2 + eqWallThick) > X1) && (X2 + XDim2) < X1)
                        {
                            if (Y2 + XDim2 + eqWallThick < Y1) 
                            {
                                return Enums.TypeRoomConfig.horPosOff;
                            }
                            else
                            {
                                return Enums.TypeRoomConfig.horNegOff;
                            }
                        }
                        else if (((Y1 + YDim1 + eqWallThick) > Y2) && ((Y1 + YDim1) < Y2))
                        {
                            if (X1 + XDim1 + eqWallThick < X2) 
                            {
                                return Enums.TypeRoomConfig.horPosOff;
                            }
                            else
                            {
                                return Enums.TypeRoomConfig.horNegOff;
                            }
                        }
                    }
                    else if (direcX == Enums.TypeDirec.Xneg && direcY == Enums.TypeDirec.Yneg)
                    {
                        if (((Y2 + YDim2 + eqWallThick) > Y1) && ((Y2 + YDim2) < Y1) && ((X2 + XDim2 + eqWallThick) > X1) && ((X2 + XDim2) < X1))
                        {
                            return Enums.TypeRoomConfig.horOff;
                        }
                        else if (((X2 + XDim2 + eqWallThick) > X1) && (X2 + XDim2) < X1)
                        {
                            if (Y2 + YDim2 + eqWallThick < Y1) 
                            {
                                return Enums.TypeRoomConfig.horPosOff;
                            }
                            else
                            {
                                return Enums.TypeRoomConfig.horNegOff;
                            }
                        }
                        else if (((Y2 + YDim2 + eqWallThick) > Y1) && ((Y2 + YDim2) < Y1))
                        {
                            if (X2 + XDim2 + eqWallThick < X1)
                            {
                                return Enums.TypeRoomConfig.horPosOff;
                            }
                            else
                            {
                                return Enums.TypeRoomConfig.horNegOff;
                            }
                        }
                    }
                } 
            }
            return roomConfig;
        }
        public Enums.TypeRoomConfig TryVerticalConfigsFirstDimension(double X1, double Y1, double XDim1, double YDim1, double X2, double Y2, double XDim2, double YDim2)
        {
            var geo = new GeometryHandler();
            Enums.TypeRoomConfig roomConfig = new Enums.TypeRoomConfig();

            var direcX = geo.GetDirection(X1, X2, 0, 0);
            var direcY = geo.GetDirection(0, 0, Y1, Y2);

            if (X1 - tolerance < X2 && X1 + tolerance > X2 || X2 - tolerance < X1 && X2 + tolerance > X1) //==
            {
                if (Y1 - tolerance < Y2 && Y1 + tolerance > Y2 || Y2 - tolerance < Y1 && Y2 + tolerance > Y1) //==
                {
                    if (XDim1 - tolerance < XDim2 && XDim1 + tolerance > XDim2 || XDim2 - tolerance < XDim1 && XDim2 + tolerance > XDim1) //== 
                    {
                        if (YDim1 - tolerance < YDim2 && YDim1 + tolerance > YDim2 || YDim2 - tolerance < YDim1 && YDim2 + tolerance > YDim1) //==
                        {
                            return Enums.TypeRoomConfig.ver;
                        }
                        else
                        {
                            return Enums.TypeRoomConfig.verNegOff;
                        }
                    }
                    else
                    {
                        return Enums.TypeRoomConfig.verNegOff;
                    }
                }
                else if (Y1 != Y2)
                {
                    if (direcY == Enums.TypeDirec.Ypos)
                    {
                        if (Y1 + YDim1 - tolerance > Y2)
                        {
                            return Enums.TypeRoomConfig.verNegOff;
                        }
                        else
                        {
                            if ((Y1 + YDim1 + eqWallThick) > Y2)
                            {
                                return Enums.TypeRoomConfig.verOff;
                            }
                        }
                    }
                    else if (direcY == Enums.TypeDirec.Yneg)
                    {
                        if (Y2 + YDim2 -tolerance > Y1)
                        {
                            return Enums.TypeRoomConfig.verNegOff;
                        }
                        else
                        {
                            if ((Y2 + YDim2 + eqWallThick) > Y1)
                            {
                                return Enums.TypeRoomConfig.verOff;
                            }
                        }
                    }
                }
            }
            else if (X1 + XDim1 - tolerance < X2 + XDim2 && X1 + XDim1 + tolerance > X2 + XDim2 || X2 + XDim2 - tolerance < X1 + XDim1 && X2 + XDim2 + tolerance > X1 + XDim1) //==
            {
                //bedeutet dass X1 != X2 sonst wäre es in erste Abfrage gegangen
                if (Y1 - tolerance < Y2 && Y1 + tolerance > Y2 || Y2 - tolerance < Y1 && Y2 + tolerance > Y1) //==
                {
                    return Enums.TypeRoomConfig.verNegOff;
                }
                else if (direcY == Enums.TypeDirec.Ypos)
                {
                    if (Y1 + YDim1 - tolerance > Y2)
                    {
                        return Enums.TypeRoomConfig.verNegOff;
                    }
                    else
                    {
                        if ((Y1 + YDim1 + eqWallThick) > Y2)
                        {
                            return Enums.TypeRoomConfig.verOff;
                        }
                    }
                }
                else if (direcY == Enums.TypeDirec.Yneg)
                {
                    if (Y2 + YDim2 - tolerance > Y1)
                    {
                        return Enums.TypeRoomConfig.verNegOff;
                    }
                    else
                    {
                        if ((Y2 + YDim2 + eqWallThick) > Y1)
                        {
                            return Enums.TypeRoomConfig.verOff;
                        }                 
                    }
                }
            }
            else if (Y1 + YDim1 - tolerance < Y2 + YDim2 && Y1 + YDim1 + tolerance > X2 + YDim2 || Y2 + YDim2 - tolerance < Y1 + YDim1 && Y2 + YDim2 + tolerance > Y1 + YDim1) //==
            {
                //bedeutet dass X1 != X2 sonst wäre es in erste Abfrage gegangen
                if (direcX == Enums.TypeDirec.Xpos)
                {
                    if (X1 + XDim1 - tolerance > X2)
                    {
                        return Enums.TypeRoomConfig.verNegOff;
                    }
                    else
                    {
                        if ((X1 + XDim1 + eqWallThick) > X2)
                        {
                            return Enums.TypeRoomConfig.verOff;
                        }
                    }
                }
                else if (direcX == Enums.TypeDirec.Xneg)
                {
                    if (X2 + XDim2 - tolerance > X1)
                    {
                        return Enums.TypeRoomConfig.verNegOff;
                    }
                    else
                    {
                        if ((X2 + XDim2 + eqWallThick) > X1)
                        {
                            return Enums.TypeRoomConfig.verOff;
                        }
                    }
                }
            }
            else if (X1 != X2)
            {
                if (Y1 - tolerance < Y2 && Y1 + tolerance > Y2 || Y2 - tolerance < Y1 && Y2 + tolerance > Y1) //==
                {
                    if (direcX == Enums.TypeDirec.Xpos)
                    {
                        if (X1 + XDim1 - tolerance > X2)
                        {
                            return Enums.TypeRoomConfig.verNegOff;
                        }
                        else
                        {
                            if ((X1 + XDim1 + eqWallThick) > X2)
                            {
                                return Enums.TypeRoomConfig.verOff;
                            }
                        }
                    }
                    if (direcX == Enums.TypeDirec.Xneg)
                    {
                        if (X2 + XDim2 - tolerance > X1)
                        {
                            return Enums.TypeRoomConfig.verNegOff;
                        }
                        else
                        {
                            if ((X2 + XDim2 + eqWallThick) > X1)
                            {
                                return Enums.TypeRoomConfig.verOff;
                            }
                        }
                    }
                }
                else if (Y1 != Y2)
                {
                    if (direcX == Enums.TypeDirec.Xpos && direcY == Enums.TypeDirec.Ypos)
                    {
                        if (((Y1 + YDim1 + eqWallThick) > Y2) && ((Y1 + YDim1) < Y2) && ((X1 + XDim1 + eqWallThick) > X2) && ((X1 + XDim1) < X2))
                        {
                            return Enums.TypeRoomConfig.verOff;
                        }
                        else if ((Y1 + YDim1) > Y2 && (X1 + XDim1) > X2)
                        {
                            return Enums.TypeRoomConfig.verNegOff;
                        }
                        else if (((X1 + XDim1 + eqWallThick) > X2) && ((X1 + XDim1) < X2))
                        {
                            if (Y1 + YDim1 + eqWallThick < Y2) 
                            {
                                return Enums.TypeRoomConfig.verPosOff;
                            }
                            else
                            {
                                return Enums.TypeRoomConfig.verNegOff;
                            }
                        }
                        else if (((Y1 + YDim1 + eqWallThick) > Y2) && ((Y1 + YDim1) < Y2))
                        {
                            if (X1 + XDim1 + eqWallThick < X2) 
                            {
                                return Enums.TypeRoomConfig.verPosOff;
                            }
                            else
                            {
                                return Enums.TypeRoomConfig.verNegOff;
                            }
                        }
                    }




                    else if (direcX == Enums.TypeDirec.Xpos && direcY == Enums.TypeDirec.Yneg)
                    {
                        if (((Y2 + YDim2 + eqWallThick) > Y1) && ((Y2 + YDim2) < Y1) && ((X1 + XDim1 + eqWallThick) > X2) && ((X1 + XDim1) < X2))
                        {
                            return Enums.TypeRoomConfig.verOff;
                        }
                        else if ((Y2 + YDim2) > Y1 && (X1 + XDim1) > X2)
                        {
                            return Enums.TypeRoomConfig.verNegOff;
                        }
                        else if ((X1 + XDim1 + eqWallThick) > X2 && ((X1 + XDim1) < X2))
                        {
                            if (Y1 + YDim1 + eqWallThick < Y2) 
                            {
                                return Enums.TypeRoomConfig.verPosOff;
                            }
                            else
                            {
                                return Enums.TypeRoomConfig.verNegOff;
                            }
                        }
                        else if ((Y2 + YDim2 + eqWallThick) > Y1 && ((Y2 + YDim2) < Y1))
                        {
                            if (X2 + XDim2 + eqWallThick < X1) 
                            {
                                return Enums.TypeRoomConfig.verPosOff;
                            }
                            else
                            {
                                return Enums.TypeRoomConfig.verNegOff;
                            }
                        }
                    }
                    else if (direcX == Enums.TypeDirec.Xneg && direcY == Enums.TypeDirec.Ypos)
                    {
                        if (((Y1 + YDim1 + eqWallThick) > Y2) && ((Y1 + YDim1) < Y2) && ((X2 + XDim2 + eqWallThick) > X2) && ((X2 + XDim2) < X2))
                        {
                            return Enums.TypeRoomConfig.verOff;
                        }
                        else if ((Y1 + YDim1) > Y2 && (X2 + XDim2) > X1)
                        {
                            return Enums.TypeRoomConfig.verNegOff;
                        }
                        else if (((X2 + XDim2 + eqWallThick) > X1) && (X2 + XDim2) < X1)
                        {
                            if (Y2 + XDim2 + eqWallThick < Y1) 
                            {
                                return Enums.TypeRoomConfig.verPosOff;
                            }
                            else
                            {
                                return Enums.TypeRoomConfig.verNegOff;
                            }
                        }
                        else if (((Y1 + YDim1 + eqWallThick) > Y2) && ((Y1 + YDim1) < Y2))
                        {
                            if (X1 + XDim1 + eqWallThick < X2) 
                            {
                                return Enums.TypeRoomConfig.verPosOff;
                            }
                            else
                            {
                                return Enums.TypeRoomConfig.verNegOff;
                            }
                        }
                    }
                    else if (direcX == Enums.TypeDirec.Xneg && direcY == Enums.TypeDirec.Yneg)
                    {
                        if (((Y2 + YDim2 + eqWallThick) > Y1) && ((Y2 + YDim2) < Y1) && ((X2 + XDim2 + eqWallThick) > X2) && ((X2 + XDim2) < X2))
                        {
                            return Enums.TypeRoomConfig.verOff;
                        }
                        else if ((Y2 + YDim2) > Y1 && (X2 + XDim2) > X1)
                        {
                            return Enums.TypeRoomConfig.verNegOff;
                        }
                        else if (((X2 + XDim2 + eqWallThick) > X1) && (X2 + XDim2) < X1)
                        {
                            if (Y2 + YDim2 + eqWallThick < Y1 || Y1 + XDim1 + eqWallThick < Y2)
                            {
                                return Enums.TypeRoomConfig.verPosOff;
                            }
                            else
                            {
                                return Enums.TypeRoomConfig.verNegOff;
                            }
                        }
                        else if (((Y2 + YDim2 + eqWallThick) > Y1) && ((Y2 + YDim2) < Y1))
                        {
                            if (X1 + XDim1 + eqWallThick < X2 || X2 + XDim2 + eqWallThick < X1)
                            {
                                return Enums.TypeRoomConfig.verPosOff;
                            }
                            else
                            {
                                return Enums.TypeRoomConfig.verNegOff;
                            }
                        }
                    }
                }
            }
            return roomConfig;
        }
        public Enums.TypeRoomConfig TryVerticalConfigsSecondDimension(double X1, double Y1, double XDim1, double YDim1, double X2, double Y2, double XDim2, double YDim2)
        {
            var geo = new GeometryHandler();
            Enums.TypeRoomConfig roomConfig = new Enums.TypeRoomConfig();

            var direcX = geo.GetDirection(X1, X2, 0, 0);
            var direcY = geo.GetDirection(0, 0, Y1, Y2);

            if (X1 - tolerance < X2 && X1 + tolerance > X2 || X2 - tolerance < X1 && X2 + tolerance > X1) //==
            {
                if (Y1 - tolerance < Y2 && Y1 + tolerance > Y2 || Y2 - tolerance < Y1 && Y2 + tolerance > Y1) //==
                {
                    if (XDim1 - tolerance < XDim2 && XDim1 + tolerance > XDim2 || XDim2 - tolerance < XDim1 && XDim2 + tolerance > XDim1) //== 
                    {
                        if (YDim1 - tolerance < YDim2 && YDim1 + tolerance > YDim2 || YDim2 - tolerance < YDim1 && YDim2 + tolerance > YDim1) //==
                        {
                            return Enums.TypeRoomConfig.ver1Room;
                        }
                        else
                        {
                            return Enums.TypeRoomConfig.ver1Room;
                        }
                    }
                    else
                    {
                        return Enums.TypeRoomConfig.ver1Room;
                    }
                }
                else if (Y1 != Y2)
                {
                    if (direcY == Enums.TypeDirec.Ypos)
                    {
                        if (Y1 + YDim1 - tolerance > Y2)
                        {
                            return Enums.TypeRoomConfig.ver1Room;
                        }
                        else
                        {
                            if ((Y1 + YDim1 + eqWallThick) > Y2)
                            {
                                return Enums.TypeRoomConfig.invalid;
                            }
                            else
                            {
                                return Enums.TypeRoomConfig.invalid;
                            }
                        }
                    }
                    else if (direcY == Enums.TypeDirec.Yneg)
                    {
                        if (Y2 + YDim2 - tolerance > Y1)
                        {
                            return Enums.TypeRoomConfig.ver1Room;
                        }
                        else
                        {
                            if ((Y2 + YDim2 + eqWallThick) > Y1)
                            {
                                return Enums.TypeRoomConfig.invalid;
                            }
                            else
                            {
                                return Enums.TypeRoomConfig.invalid;
                            }
                        }
                    }
                }
            }
            else if (X1 + XDim1 - tolerance < X2 + XDim2 && X1 + XDim1 + tolerance > X2 + XDim2 || X2 + XDim2 - tolerance < X1 + XDim1 && X2 + XDim2 + tolerance > X1 + XDim1) //==
            {
                //bedeutet dass X1 != X2 sonst wäre es in erste Abfrage gegangen
                if (Y1 - tolerance < Y2 && Y1 + tolerance > Y2 || Y2 - tolerance < Y1 && Y2 + tolerance > Y1) //==
                {
                    return Enums.TypeRoomConfig.ver1Room;
                }
                else if (direcY == Enums.TypeDirec.Ypos)
                {
                    if (Y1 + YDim1 - tolerance > Y2)
                    {
                        return Enums.TypeRoomConfig.ver1Room;
                    }
                    else
                    {
                        if ((Y1 + YDim1 + eqWallThick) > Y2)
                        {
                            return Enums.TypeRoomConfig.invalid;
                        }
                        else
                        {
                            return Enums.TypeRoomConfig.invalid;
                        }
                    }
                }
                else if (direcY == Enums.TypeDirec.Yneg)
                {
                    if (Y2 + YDim2 - tolerance > Y1)
                    {
                        return Enums.TypeRoomConfig.ver1Room;
                    }
                    else
                    {
                        if ((Y2 + YDim2 + eqWallThick) > Y1)
                        {
                            return Enums.TypeRoomConfig.invalid;
                        }
                        else
                        {
                            return Enums.TypeRoomConfig.invalid;
                        }
                    }
                }
            }
            else if (Y1 + YDim1 - tolerance < Y2 + YDim2 && Y1 + YDim1 + tolerance > X2 + YDim2 || Y2 + YDim2 - tolerance < Y1 + YDim1 && Y2 + YDim2 + tolerance > Y1 + YDim1) //==
            {
                //bedeutet dass X1 != X2 sonst wäre es in erste Abfrage gegangen
                if (direcX == Enums.TypeDirec.Xpos)
                {
                    if (X1 + XDim1 - tolerance > X2)
                    {
                        return Enums.TypeRoomConfig.ver1Room;
                    }
                    else
                    {
                        if ((X1 + XDim1 + eqWallThick) > X2)
                        {
                            return Enums.TypeRoomConfig.invalid;
                        }
                        else
                        {
                            return Enums.TypeRoomConfig.invalid;
                        }
                    }
                }
                else if (direcX == Enums.TypeDirec.Xneg)
                {
                    if (X2 + XDim2 - tolerance > X1)
                    {
                        return Enums.TypeRoomConfig.ver1Room;
                    }
                    else
                    {
                        if ((X2 + XDim2 + eqWallThick) > X1)
                        {
                            return Enums.TypeRoomConfig.invalid;
                        }
                        else
                        {
                            return Enums.TypeRoomConfig.invalid;
                        }
                    }
                }
            }
            else if (X1 != X2)
            {
                if (Y1 - tolerance < Y2 && Y1 + tolerance > Y2 || Y2 - tolerance < Y1 && Y2 + tolerance > Y1) //==
                {
                    if (direcX == Enums.TypeDirec.Xpos)
                    {
                        if (X1 + XDim1 - tolerance > X2)
                        {
                            return Enums.TypeRoomConfig.ver1Room;
                        }
                        else
                        {
                            if ((X1 + XDim1 + eqWallThick) > X2)
                            {
                                return Enums.TypeRoomConfig.invalid;
                            }
                            else
                            {
                                return Enums.TypeRoomConfig.invalid;
                            }
                        }
                    }
                    if (direcX == Enums.TypeDirec.Xneg)
                    {
                        if (X2 + XDim2 - tolerance > X1)
                        {
                            return Enums.TypeRoomConfig.ver1Room;
                        }
                        else
                        {
                            if ((X2 + XDim2 + eqWallThick) > X1)
                            {
                                return Enums.TypeRoomConfig.invalid;
                            }
                            else
                            {
                                return Enums.TypeRoomConfig.invalid;
                            }
                        }
                    }
                }
                else if (Y1 != Y2)
                {
                    if (direcX == Enums.TypeDirec.Xpos && direcY == Enums.TypeDirec.Ypos)
                    {
                        if (((Y1 + YDim1 + eqWallThick) > Y2) && ((Y1 + YDim1) < Y2) && ((X1 + XDim1 + eqWallThick) > X2) && ((X1 + XDim1) < X2))
                        {
                            return Enums.TypeRoomConfig.invalid;
                        }
                        else if ((Y1 + YDim1) > Y2 && (X1 + XDim1) > X2)
                        {
                            return Enums.TypeRoomConfig.ver1Room;
                        }
                        else if (((X1 + XDim1 + eqWallThick) > X2) && ((X1 + XDim1) < X2))
                        {
                            if (Y1 + YDim1 + eqWallThick < Y2) 
                            {
                                return Enums.TypeRoomConfig.invalid;
                            }
                            else
                            {
                                return Enums.TypeRoomConfig.ver1Room;
                            }
                        }
                        else if (((Y1 + YDim1 + eqWallThick) > Y2) && ((Y1 + YDim1) < Y2))
                        {
                            if (X1 + XDim1 + eqWallThick < X2) 
                            {
                                return Enums.TypeRoomConfig.invalid;
                            }
                            else
                            {
                                return Enums.TypeRoomConfig.ver1Room;
                            }
                        }
                    }
                    else if (direcX == Enums.TypeDirec.Xpos && direcY == Enums.TypeDirec.Yneg)
                    {
                        if (((Y2 + YDim2 + eqWallThick) > Y1) && ((Y2 + YDim2) < Y1) && ((X1 + XDim1 + eqWallThick) > X2) && ((X1 + XDim1) < X2))
                        {
                            return Enums.TypeRoomConfig.invalid;
                        }
                        else if ((Y2 + YDim2) > Y1 && (X1 + XDim1) > X2)
                        {
                            return Enums.TypeRoomConfig.ver1Room;
                        }
                        else if ((X1 + XDim1 + eqWallThick) > X2 && ((X1 + XDim1) < X2))
                        {
                            if (Y1 + YDim1 + eqWallThick < Y2) 
                            {
                                return Enums.TypeRoomConfig.invalid;
                            }
                            else
                            {
                                return Enums.TypeRoomConfig.ver1Room;
                            }
                        }
                        else if ((Y2 + YDim2 + eqWallThick) > Y1 && ((Y2 + YDim2) < Y1))
                        {
                            if (X2 + XDim2 + eqWallThick < X1) 
                            {
                                return Enums.TypeRoomConfig.invalid;
                            }
                            else
                            {
                                return Enums.TypeRoomConfig.ver1Room;
                            }
                        }
                    }
                    else if (direcX == Enums.TypeDirec.Xneg && direcY == Enums.TypeDirec.Ypos)
                    {
                        if (((Y1 + YDim1 + eqWallThick) > Y2) && ((Y1 + YDim1) < Y2) && ((X2 + XDim2 + eqWallThick) > X2) && ((X2 + XDim2) < X2))
                        {
                            return Enums.TypeRoomConfig.invalid;
                        }
                        else if ((Y1 + YDim1) > Y2 && (X2 + XDim2) > X1)
                        {
                            return Enums.TypeRoomConfig.ver1Room;
                        }
                        else if (((X2 + XDim2 + eqWallThick) > X1) && (X2 + XDim2) < X1)
                        {
                            if (Y2 + XDim2 + eqWallThick < Y1) 
                            {
                                return Enums.TypeRoomConfig.invalid;
                            }
                            else
                            {
                                return Enums.TypeRoomConfig.ver1Room;
                            }
                        }
                        else if (((Y1 + YDim1 + eqWallThick) > Y2) && ((Y1 + YDim1) < Y2))
                        {
                            if (X1 + XDim1 + eqWallThick < X2) 
                            {
                                return Enums.TypeRoomConfig.invalid;
                            }
                            else
                            {
                                return Enums.TypeRoomConfig.ver1Room;
                            }
                        }
                    }
                    else if (direcX == Enums.TypeDirec.Xneg && direcY == Enums.TypeDirec.Yneg)
                    {
                        if (((Y2 + YDim2 + eqWallThick) > Y1) && ((Y2 + YDim2) < Y1) && ((X2 + XDim2 + eqWallThick) > X2) && ((X2 + XDim2) < X2))
                        {
                            return Enums.TypeRoomConfig.invalid;
                        }
                        else if ((Y2 + YDim2) > Y1 && (X2 + XDim2) > X1)
                        {
                            return Enums.TypeRoomConfig.ver1Room;
                        }
                        else if (((X2 + XDim2 + eqWallThick) > X1) && (X2 + XDim2) < X1)
                        {
                            if (Y2 + YDim2 + eqWallThick < Y1 || Y1 + XDim1 + eqWallThick < Y2)
                            {
                                return Enums.TypeRoomConfig.invalid;
                            }
                            else
                            {
                                return Enums.TypeRoomConfig.ver1Room;
                            }
                        }
                        else if (((Y2 + YDim2 + eqWallThick) > Y1) && ((Y2 + YDim2) < Y1))
                        {
                            if (X1 + XDim1 + eqWallThick < X2 || X2 + XDim2 + eqWallThick < X1)
                            {
                                return Enums.TypeRoomConfig.invalid;
                            }
                            else
                            {
                                return Enums.TypeRoomConfig.ver1Room;
                            }
                        }
                    }
                }
            }
            return roomConfig;
        }
        public Enums.TypeRoomConfig TryVerticalConfigsThirdimension(double X1, double Y1, double XDim1, double YDim1, double X2, double Y2, double XDim2, double YDim2)
        {
            var geo = new GeometryHandler();
            Enums.TypeRoomConfig roomConfig = new Enums.TypeRoomConfig();

            var direcX = geo.GetDirection(X1, X2, 0, 0);
            var direcY = geo.GetDirection(0, 0, Y1, Y2);

            if (X1 - tolerance < X2 && X1 + tolerance > X2 || X2 - tolerance < X1 && X2 + tolerance > X1) //==
            {
                if (Y1 - tolerance < Y2 && Y1 + tolerance > Y2 || Y2 - tolerance < Y1 && Y2 + tolerance > Y1) //==
                {
                    if (XDim1 - tolerance < XDim2 && XDim1 + tolerance > XDim2 || XDim2 - tolerance < XDim1 && XDim2 + tolerance > XDim1) //== 
                    {
                        if (YDim1 - tolerance < YDim2 && YDim1 + tolerance > YDim2 || YDim2 - tolerance < YDim1 && YDim2 + tolerance > YDim1) //==
                        {
                            return Enums.TypeRoomConfig.ver2Room;
                        }
                        else
                        {
                            return Enums.TypeRoomConfig.ver2Room;
                        }
                    }
                    else
                    {
                        return Enums.TypeRoomConfig.ver2Room;
                    }
                }
                else if (Y1 != Y2)
                {
                    if (direcY == Enums.TypeDirec.Ypos)
                    {
                        if (Y1 + YDim1 - tolerance > Y2)
                        {
                            return Enums.TypeRoomConfig.ver2Room;
                        }
                        else
                        {
                            if ((Y1 + YDim1 + eqWallThick) > Y2)
                            {
                                return Enums.TypeRoomConfig.invalid;
                            }
                            else
                            {
                                return Enums.TypeRoomConfig.invalid;
                            }
                        }
                    }
                    else if (direcY == Enums.TypeDirec.Yneg)
                    {
                        if (Y2 + YDim2 - tolerance > Y1)
                        {
                            return Enums.TypeRoomConfig.ver2Room;
                        }
                        else
                        {
                            if ((Y2 + YDim2 + eqWallThick) > Y1)
                            {
                                return Enums.TypeRoomConfig.invalid;
                            }
                            else
                            {
                                return Enums.TypeRoomConfig.invalid;
                            }
                        }
                    }
                }
            }
            else if (X1 + XDim1 - tolerance < X2 + XDim2 && X1 + XDim1 + tolerance > X2 + XDim2 || X2 + XDim2 - tolerance < X1 + XDim1 && X2 + XDim2 + tolerance > X1 + XDim1) //==
            {
                //bedeutet dass X1 != X2 sonst wäre es in erste Abfrage gegangen
                if (Y1 - tolerance < Y2 && Y1 + tolerance > Y2 || Y2 - tolerance < Y1 && Y2 + tolerance > Y1) //==
                {
                    return Enums.TypeRoomConfig.ver2Room;
                }
                else if (direcY == Enums.TypeDirec.Ypos)
                {
                    if (Y1 + YDim1 - tolerance > Y2)
                    {
                        return Enums.TypeRoomConfig.ver2Room;
                    }
                    else
                    {
                        if ((Y1 + YDim1 + eqWallThick) > Y2)
                        {
                            return Enums.TypeRoomConfig.invalid;
                        }
                        else
                        {
                            return Enums.TypeRoomConfig.invalid;
                        }
                    }
                }
                else if (direcY == Enums.TypeDirec.Yneg)
                {
                    if (Y2 + YDim2 - tolerance > Y1)
                    {
                        return Enums.TypeRoomConfig.ver2Room;
                    }
                    else
                    {
                        if ((Y2 + YDim2 + eqWallThick) > Y1)
                        {
                            return Enums.TypeRoomConfig.invalid;
                        }
                        else
                        {
                            return Enums.TypeRoomConfig.invalid;
                        }
                    }
                }
            }
            else if (Y1 + YDim1 - tolerance < Y2 + YDim2 && Y1 + YDim1 + tolerance > X2 + YDim2 || Y2 + YDim2 - tolerance < Y1 + YDim1 && Y2 + YDim2 + tolerance > Y1 + YDim1) //==
            {
                //bedeutet dass X1 != X2 sonst wäre es in erste Abfrage gegangen
                if (X1 - tolerance < X2 && X1 + tolerance > X2 || X2 - tolerance < X1 && X2 + tolerance > X1) //==
                {
                    return Enums.TypeRoomConfig.ver2Room;
                }
                else if (direcX == Enums.TypeDirec.Xpos)
                {
                    if (X1 + XDim1 - tolerance > X2)
                    {
                        return Enums.TypeRoomConfig.ver2Room;
                    }
                    else
                    {
                        if ((X1 + XDim1 + eqWallThick) > X2)
                        {
                            return Enums.TypeRoomConfig.invalid;
                        }
                        else
                        {
                            return Enums.TypeRoomConfig.invalid;
                        }
                    }
                }
                else if (direcX == Enums.TypeDirec.Xneg)
                {
                    if (X2 + XDim2 - tolerance > X1)
                    {
                        return Enums.TypeRoomConfig.ver2Room;
                    }
                    else
                    {
                        if ((X2 + XDim2 + eqWallThick) > X1)
                        {
                            return Enums.TypeRoomConfig.invalid;
                        }
                        else
                        {
                            return Enums.TypeRoomConfig.invalid;
                        }
                    }
                }
            }
            else if (X1 != X2)
            {
                if (Y1 - tolerance < Y2 && Y1 + tolerance > Y2 || Y2 - tolerance < Y1 && Y2 + tolerance > Y1) //==
                {
                    if (direcX == Enums.TypeDirec.Xpos)
                    {
                        if (X1 + XDim1 - tolerance > X2)
                        {
                            return Enums.TypeRoomConfig.ver2Room;
                        }
                        else
                        {
                            if ((X1 + XDim1 + eqWallThick) > X2)
                            {
                                return Enums.TypeRoomConfig.invalid;
                            }
                            else
                            {
                                return Enums.TypeRoomConfig.invalid;
                            }
                        }
                    }
                    if (direcX == Enums.TypeDirec.Xneg)
                    {
                        if (X2 + XDim2 - tolerance > X1)
                        {
                            return Enums.TypeRoomConfig.ver2Room;
                        }
                        else
                        {
                            if ((X2 + XDim2 + eqWallThick) > X1)
                            {
                                return Enums.TypeRoomConfig.invalid;
                            }
                            else
                            {
                                return Enums.TypeRoomConfig.invalid;
                            }
                        }
                    }
                }
                else if (Y1 != Y2)
                {
                    if (direcX == Enums.TypeDirec.Xpos && direcY == Enums.TypeDirec.Ypos)
                    {
                        if (((Y1 + YDim1 + eqWallThick) > Y2) && ((Y1 + YDim1) < Y2) && ((X1 + XDim1 + eqWallThick) > X2) && ((X1 + XDim1) < X2))
                        {
                            return Enums.TypeRoomConfig.invalid;
                        }
                        else if ((Y1 + YDim1) > Y2 && (X1 + XDim1) > X2)
                        {
                            return Enums.TypeRoomConfig.ver1Room;
                        }
                        else if (((X1 + XDim1 + eqWallThick) > X2) && ((X1 + XDim1) < X2))
                        {
                            if (Y1 + YDim1 + eqWallThick < Y2) // || Y2 + YDim2 + eqWallThick < Y1)
                            {
                                return Enums.TypeRoomConfig.invalid;
                            }
                            else
                            {
                                return Enums.TypeRoomConfig.ver2Room;
                            }
                        }
                        else if (((Y1 + YDim1 + eqWallThick) > Y2) && ((Y1 + YDim1) < Y2))
                        {
                            if (X1 + XDim1 + eqWallThick < X2) // || X2 + XDim2 + eqWallThick < X1)
                            {
                                return Enums.TypeRoomConfig.invalid;
                            }
                            else
                            {
                                return Enums.TypeRoomConfig.ver2Room;
                            }
                        }
                    }
                    else if (direcX == Enums.TypeDirec.Xpos && direcY == Enums.TypeDirec.Yneg)
                    {
                        if (((Y2 + YDim2 + eqWallThick) > Y1) && ((Y2 + YDim2) < Y1) && ((X1 + XDim1 + eqWallThick) > X2) && ((X1 + XDim1) < X2))
                        {
                            return Enums.TypeRoomConfig.invalid;
                        }
                        else if ((Y2 + YDim2) > Y1 && (X1 + XDim1) > X2)
                        {
                            return Enums.TypeRoomConfig.ver2Room;
                        }
                        else if ((X1 + XDim1 + eqWallThick) > X2 && ((X1 + XDim1) < X2))
                        {
                            if (Y1 + YDim1 + eqWallThick < Y2) // || Y2 + YDim2 + eqWallThick < Y1)
                            {
                                return Enums.TypeRoomConfig.invalid;
                            }
                            else
                            {
                                return Enums.TypeRoomConfig.ver2Room;
                            }
                        }
                        else if ((Y2 + YDim2 + eqWallThick) > Y1 && ((Y2 + YDim2) < Y1))
                        {
                            if (X2 + XDim2 + eqWallThick < X1) // || X1 + XDim1 + eqWallThick < X2)
                            {
                                return Enums.TypeRoomConfig.invalid;
                            }
                            else
                            {
                                return Enums.TypeRoomConfig.ver2Room;
                            }
                        }
                    }
                    else if (direcX == Enums.TypeDirec.Xneg && direcY == Enums.TypeDirec.Ypos)
                    {
                        if (((Y1 + YDim1 + eqWallThick) > Y2) && ((Y1 + YDim1) < Y2) && ((X2 + XDim2 + eqWallThick) > X2) && ((X2 + XDim2) < X2))
                        {
                            return Enums.TypeRoomConfig.invalid;
                        }
                        else if ((Y1 + YDim1) > Y2 && (X2 + XDim2) > X1)
                        {
                            return Enums.TypeRoomConfig.ver2Room;
                        }
                        else if (((X2 + XDim2 + eqWallThick) > X1) && (X2 + XDim2) < X1)
                        {
                            if (Y2 + XDim2 + eqWallThick < Y1) // || Y1 + YDim1 + eqWallThick < Y2)
                            {
                                return Enums.TypeRoomConfig.invalid;
                            }
                            else
                            {
                                return Enums.TypeRoomConfig.ver2Room;
                            }
                        }
                        else if (((Y1 + YDim1 + eqWallThick) > Y2) && ((Y1 + YDim1) < Y2))
                        {
                            if (X1 + XDim1 + eqWallThick < X2) // || X2 + XDim2 + eqWallThick < X1)
                            {
                                return Enums.TypeRoomConfig.invalid;
                            }
                            else
                            {
                                return Enums.TypeRoomConfig.ver2Room;
                            }
                        }
                    }
                    else if (direcX == Enums.TypeDirec.Xneg && direcY == Enums.TypeDirec.Yneg)
                    {
                        if (((Y2 + YDim2 + eqWallThick) > Y1) && ((Y2 + YDim2) < Y1) && ((X2 + XDim2 + eqWallThick) > X2) && ((X2 + XDim2) < X2))
                        {
                            return Enums.TypeRoomConfig.invalid;
                        }
                        else if ((Y2 + YDim2) > Y1 && (X2 + XDim2) > X1)
                        {
                            return Enums.TypeRoomConfig.ver2Room;
                        }
                        else if (((X2 + XDim2 + eqWallThick) > X1) && (X2 + XDim2) < X1)
                        {
                            if (Y2 + YDim2 + eqWallThick < Y1 || Y1 + XDim1 + eqWallThick < Y2)
                            {
                                return Enums.TypeRoomConfig.invalid;
                            }
                            else
                            {
                                return Enums.TypeRoomConfig.ver2Room;
                            }
                        }
                        else if (((Y2 + YDim2 + eqWallThick) > Y1) && ((Y2 + YDim2) < Y1))
                        {
                            if (X1 + XDim1 + eqWallThick < X2 || X2 + XDim2 + eqWallThick < X1)
                            {
                                return Enums.TypeRoomConfig.invalid;
                            }
                            else
                            {
                                return Enums.TypeRoomConfig.ver2Room;
                            }
                        }
                    }
                }
            }
            return roomConfig;
        }
    }
}
