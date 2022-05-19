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


namespace HVACoustics
{
    class RoomConfigurations
    {
        private static double tolerance = 0.2;
        private static double eqWallThick = 0.4;

        public static void GetGeometrySpace(IfcStore model)
        {
            var id = "2tbs39_az4k97nLauRotVn";
            IfcSpace space = model.Instances.FirstOrDefault<IfcSpace>(d => d.GlobalId == id);

            var rep = space.Representation.Representations;
            var shape = rep[0];
            var body = shape.Items[0];
            double X = ((IfcExtrudedAreaSolid)body).Position.Location.X;
            double Y = ((IfcExtrudedAreaSolid)body).Position.Location.Y;
            double XDim = ((IfcRectangleProfileDef)((IfcExtrudedAreaSolid)body).SweptArea).XDim;
            double YDim = ((IfcRectangleProfileDef)((IfcExtrudedAreaSolid)body).SweptArea).YDim;

            /*var points = ((IfcPolyline)((IfcArbitraryClosedProfileDef)((IfcExtrudedAreaSolid)body).SweptArea).OuterCurve).Points;
            List<double> xCords = points.Select(x => x.X).ToList();
            List<double> yCords = points.Select(x => x.Y).ToList();

            foreach (double i in xCords)
            {
                Console.WriteLine(i);
            }
            Vector2d vec = new Vector2d(2, 4);
            Console.WriteLine(vec);*/
        }

        public static Enums.TypeRoomConfig GetRoomConfiguration(IfcStore model, string senderID, string recieverID)
        {
            var geo = new GeometryHandler();
            var config = new RoomConfigurations();
            var helper = new Helper();

            List<double> dictCoorInOffsetXDirec = new List<double>();
            List<double> dictCoorInOffsetYDirec = new List<double>();
            double X1 = 0;
            double Y1 = 0;
            double XDim1 = 0;
            double YDim1 = 0;
            double X2 = 0;
            double Y2 = 0;
            double XDim2 = 0;
            double YDim2 = 0;
            double direcX1 = 0;
            double direcY1 = 0;
            double direcX2 = 0;
            double direcY2 = 0;
            string globalIdStorey1 = "";
            string globalIdStorey2 = "";
            int indexStorey1 = 0;
            int indexStorey2 = 0;
            int indexSpaceX1 = 0;
            int indexSpaceY1 = 0;
            int indexSpaceX2 = 0;
            int indexSpaceY2 = 0;

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
                    bool keyXExists = dictCoorInOffsetXDirec.Contains(Math.Round(bbSpace.X, 0));
                    bool keyYExists = dictCoorInOffsetYDirec.Contains(Math.Round(bbSpace.Y, 0));

                    if (keyXExists)
                    {
                        continue;
                    }
                    else
                    {
                        dictCoorInOffsetXDirec.Add(Math.Round(bbSpace.X, 0));
                    }
                    if (keyYExists)
                    {
                        continue;
                    }
                    else
                    {
                        dictCoorInOffsetYDirec.Add(Math.Round(bbSpace.Y, 0));
                    }
                }
                dictCoorInOffsetXDirec = dictCoorInOffsetXDirec.OrderBy(obj => obj).ToList();
                dictCoorInOffsetYDirec = dictCoorInOffsetYDirec.OrderBy(obj => obj).ToList();

               /* foreach (var x in dictCoorInOffsetXDirec)
                {
                    Console.WriteLine("list X1 {0}", x);
                }   
                foreach (var x in dictCoorInOffsetYDirec)
                {
                    Console.WriteLine("list Y1 {0}", x);
                }*/
                indexStorey1 = listStoreys.IndexOf(element);
            }

            var storey2 = reciever.Decomposes.Select(x => x.RelatingObject as IIfcBuildingStorey);
            foreach (var element in storey2)
            {
                var listStoreys = model.Instances.OfType<IIfcBuildingStorey>().ToList();
                globalIdStorey2 = element.GlobalId;
                
                indexStorey2 = listStoreys.IndexOf(element);
            }

            //now create a BoundingBox around the space to get the global location
            XbimRect3D prodBox1 = geo.CreateBoundingBoxAroundSpace(model, sender);
            XbimRect3D prodBox2 = geo.CreateBoundingBoxAroundSpace(model, reciever);
            X1 = Math.Round(prodBox1.Location.X, 2);
            var X1short = Math.Round(prodBox1.Location.X, 0);
            Y1 = Math.Round(prodBox1.Location.Y, 2);
            var Y1short = Math.Round(prodBox1.Location.Y, 0);
            X2 = Math.Round(prodBox2.Location.X, 2);
            var X2short = Math.Round(prodBox2.Location.X, 0);
            Y2 = Math.Round(prodBox2.Location.Y, 2);
            var Y2short = Math.Round(prodBox2.Location.Y, 0);
            indexSpaceX1 = dictCoorInOffsetXDirec.IndexOf(X1short);
            indexSpaceY1 = dictCoorInOffsetYDirec.IndexOf(Y1short);
            indexSpaceX2 = dictCoorInOffsetXDirec.IndexOf(X2short);
            indexSpaceY2 = dictCoorInOffsetYDirec.IndexOf(Y2short);

            //now get the direction of Shape of the space
            var rep1 = sender.Representation.Representations;
            var shape1 = rep1[0];
            var body1 = shape1.Items[0];
            if (((IIfcExtrudedAreaSolid)body1).Position.RefDirection != null)
            {
                direcX1 = ((IIfcExtrudedAreaSolid)body1).Position.RefDirection.X;
                direcY1 = ((IIfcExtrudedAreaSolid)body1).Position.RefDirection.Y;
            }
            else
            {
                direcX1 = 1;
                direcY1 = 0;
            }

            if (direcX1 == 1 || direcX1 == -1)
            {
                XDim1 = Math.Round(((IIfcRectangleProfileDef)((IIfcExtrudedAreaSolid)body1).SweptArea).XDim, 2);
                YDim1 = Math.Round(((IIfcRectangleProfileDef)((IIfcExtrudedAreaSolid)body1).SweptArea).YDim, 2);

            }
            if (direcY1 == 1 || direcY1 == -1)
            {
                XDim1 = Math.Round(((IIfcRectangleProfileDef)((IIfcExtrudedAreaSolid)body1).SweptArea).YDim, 2);
                YDim1 = Math.Round(((IIfcRectangleProfileDef)((IIfcExtrudedAreaSolid)body1).SweptArea).XDim, 2);
            }

            var rep2 = reciever.Representation.Representations;
            var shape2 = rep2[0];
            var body2 = shape2.Items[0];
            if (((IIfcExtrudedAreaSolid)body2).Position.RefDirection != null)
            {
                direcX2 = ((IIfcExtrudedAreaSolid)body2).Position.RefDirection.X;
                direcY2 = ((IIfcExtrudedAreaSolid)body2).Position.RefDirection.Y;
            }
            else
            {
                direcX2 = 1;
                direcY2 = 0;
            }
            if (direcX2 == 1 || direcX2 == -1)
            {
                XDim2 = Math.Round(((IIfcRectangleProfileDef)((IIfcExtrudedAreaSolid)body2).SweptArea).XDim, 2);
                YDim2 = Math.Round(((IIfcRectangleProfileDef)((IIfcExtrudedAreaSolid)body2).SweptArea).YDim, 2);
            }
            if (direcY2 == 1 || direcY2 == -1)
            {
                XDim2 = Math.Round(((IIfcRectangleProfileDef)((IIfcExtrudedAreaSolid)body2).SweptArea).YDim, 2);
                YDim2 = Math.Round(((IIfcRectangleProfileDef)((IIfcExtrudedAreaSolid)body2).SweptArea).XDim, 2);
            }
            /* Console.WriteLine(direcX1);
             Console.WriteLine(direcY1);
             Console.WriteLine(direcX2);
             Console.WriteLine(direcY2);*/

            //Console.WriteLine(X1);
            //Console.WriteLine(Y1);
            //Console.WriteLine(XDim1);
            //Console.WriteLine(YDim1);
            //Console.WriteLine(X2);
            //Console.WriteLine(Y2);
            //Console.WriteLine(XDim2);
            //Console.WriteLine(YDim2);
            //Console.WriteLine(elevationStorey1);
            //Console.WriteLine(elevationStorey2); 
            //Console.WriteLine(indexSpaceX1);
            //Console.WriteLine(indexSpaceY1);
            //Console.WriteLine(indexSpaceX2);
            //Console.WriteLine(indexSpaceY2);

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
                roomConfig = config.TryHorizontalConfigs(X1, Y1, XDim1, YDim1, X2, Y2, XDim2, YDim2, indexSpaceX1, indexSpaceY1, indexSpaceX2, indexSpaceY2);
            }
            else if (globalIdStorey1 != globalIdStorey2)
            {
                roomConfig = config.TryVerticalConfigsFirstDimension(X1, Y1, XDim1, YDim1, X2, Y2, XDim2, YDim2);
            }

            return roomConfig;
        }

        public Enums.TypeRoomConfig TryHorizontalConfigs(double X1, double Y1, double XDim1, double YDim1, double X2, double Y2, double XDim2, double YDim2, int indexSpaceX1, int indexSpaceY1, int indexSpaceX2, int indexSpaceY2)
        {
            var geo = new GeometryHandler();
            Enums.TypeRoomConfig roomConfig = new Enums.TypeRoomConfig();
            var direcX = geo.GetDirection(X1, X2, 0, 0);
            var direcY = geo.GetDirection(0, 0, Y1, Y2);
            //Console.WriteLine(direcX);
            //Console.WriteLine(direcY);
            if (X1 - tolerance < X2 && X1 + tolerance > X2 || X2 - tolerance < X1 && X2 + tolerance > X1) //==
            {              
                if (direcY == Enums.TypeDirec.Ypos)
                {  
                    if ((Y1 + YDim1 + eqWallThick) > Y2)
                    {
                        //Raum 1. Ord.
                        if (XDim1 == XDim2)
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
                        if (XDim1 == XDim2)
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
                //bedeutet dass X1 != X2 sonst wäre es in erste Abfrage gegangen
                //hier auch noch Rum höherer Ord.?

                return Enums.TypeRoomConfig.horNegOff;
            }
            else if (Y1 + YDim1 - tolerance < Y2 + YDim2 && Y1 + YDim1 + tolerance > Y2 + YDim2 || Y2 + YDim2 - tolerance < Y1 + YDim1 && Y2 + YDim2 + tolerance > Y1 + YDim1) //gleicher Endpunkt
            {
                //bedeutet dass X1 != X2 sonst wäre es in erste Abfrage gegangen
                //hier auch noch Rum höherer Ord.?

                return Enums.TypeRoomConfig.horNegOff;
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
                            if (XDim1 == XDim2)
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
                        if ((Y2 + YDim2 + eqWallThick) > Y1)
                        {
                            //Raum 1. Ord.
                            if (XDim1 == XDim2)
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
                            if (Y1 + YDim1 + eqWallThick < Y2 || Y2 + YDim2 + eqWallThick < Y1)
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
                            if (X1 + XDim1 + eqWallThick < X2 || X2 + XDim2 + eqWallThick < X1)
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
                            if (Y1 + YDim1 + eqWallThick < Y2 || Y2 + YDim2 + eqWallThick < Y1)
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
                            if (X1 + XDim1 + eqWallThick < X2 || X2 + XDim2 + eqWallThick < X1)
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
                        if (((Y1 + YDim1 + eqWallThick) > Y2) && ((Y1 + YDim1) < Y2) && ((X2 + XDim2 + eqWallThick) > X2) && ((X2 + XDim2) < X2))
                        {
                            return Enums.TypeRoomConfig.horOff;
                        }
                        else if (((X2 + XDim2 + eqWallThick) > X1) && (X2 + XDim2) < X1)
                        {
                            if (Y1 + YDim1 + eqWallThick < Y2 || Y2 + XDim2 + eqWallThick < Y1)
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
                            if (X1 + XDim1 + eqWallThick < X2 || X2 + XDim2 + eqWallThick < X1)
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
                        if (((Y2 + YDim2 + eqWallThick) > Y1) && ((Y2 + YDim2) < Y1) && ((X2 + XDim2 + eqWallThick) > X2) && ((X2 + XDim2) < X2))
                        {
                            return Enums.TypeRoomConfig.horOff;
                        }
                        else if (((X2 + XDim2 + eqWallThick) > X1) && (X2 + XDim2) < X1)
                        {
                            if (Y2 + YDim2 + eqWallThick < Y1 || Y1 + XDim1 + eqWallThick < Y2)
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
                            if (X1 + XDim1 + eqWallThick < X2 || X2 + XDim2 + eqWallThick < X1)
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
            //Console.WriteLine(direcX);
            //Console.WriteLine(direcY);

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
                        if (Y1 + YDim1 > Y2)
                        {
                            return Enums.TypeRoomConfig.verNegOff;
                        }
                        else
                        {
                            if ((Y1 + YDim1 + eqWallThick) > Y2)
                            {
                                return Enums.TypeRoomConfig.verOff;
                            }
                            else
                            {
                                string ord = "Raum höherer Ord.";
                            }
                        }
                    }
                    else if (direcY == Enums.TypeDirec.Yneg)
                    {
                        if (Y2 + YDim2 > Y1)
                        {
                            return Enums.TypeRoomConfig.verNegOff;
                        }
                        else
                        {
                            if ((Y2 + YDim2 + eqWallThick) > Y1)
                            {
                                return Enums.TypeRoomConfig.verOff;
                            }
                            else
                            {
                                string ord = "Raum höherer Ord.";
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
                    if (Y1 + YDim1 > Y2)
                    {
                        return Enums.TypeRoomConfig.verNegOff;
                    }
                    else
                    {
                        if ((Y1 + YDim1 + eqWallThick) > Y2)
                        {
                            return Enums.TypeRoomConfig.verOff;
                        }
                        else
                        {
                            string ord = "Raum höherer Ord.";
                        }
                    }
                }
                else if (direcY == Enums.TypeDirec.Yneg)
                {
                    if (Y2 + YDim2 > Y1)
                    {
                        return Enums.TypeRoomConfig.verNegOff;
                    }
                    else
                    {
                        if ((Y2 + YDim2 + eqWallThick) > Y1)
                        {
                            return Enums.TypeRoomConfig.verOff;
                        }
                        else
                        {
                            string ord = "Raum höherer Ord.";
                        }
                    }
                }
            }
            else if (Y1 + YDim1 - tolerance < Y2 + YDim2 && Y1 + YDim1 + tolerance > X2 + YDim2 || Y2 + YDim2 - tolerance < Y1 + YDim1 && Y2 + YDim2 + tolerance > Y1 + YDim1) //==
            {
                //bedeutet dass X1 != X2 sonst wäre es in erste Abfrage gegangen
                if (X1 - tolerance < X2 && X1 + tolerance > X2 || X2 - tolerance < X1 && X2 + tolerance > X1) //==
                {
                    return Enums.TypeRoomConfig.verNegOff;
                }
                else if (direcX == Enums.TypeDirec.Xpos)
                {
                    if (X1 + XDim1 > X2)
                    {
                        return Enums.TypeRoomConfig.verNegOff;
                    }
                    else
                    {
                        if ((X1 + XDim1 + eqWallThick) > X2)
                        {
                            return Enums.TypeRoomConfig.verOff;
                        }
                        else
                        {
                            string ord = "Raum höherer Ord.";
                        }
                    }
                }
                else if (direcX == Enums.TypeDirec.Xneg)
                {
                    if (X2 + XDim2 > X1)
                    {
                        return Enums.TypeRoomConfig.verNegOff;
                    }
                    else
                    {
                        if ((X2 + XDim2 + eqWallThick) > X1)
                        {
                            return Enums.TypeRoomConfig.verOff;
                        }
                        else
                        {
                            string ord = "Raum höherer Ord.";
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
                        if (X1 + XDim1 > X2)
                        {
                            return Enums.TypeRoomConfig.verNegOff;
                        }
                        else
                        {
                            if ((X1 + XDim1 + eqWallThick) > X2)
                            {
                                return Enums.TypeRoomConfig.verOff;
                            }
                            else
                            {
                                string ord = "Raum höherer Ord.";
                            }
                        }
                    }
                    if (direcX == Enums.TypeDirec.Xneg)
                    {
                        if (X2 + XDim2 > X1)
                        {
                            return Enums.TypeRoomConfig.verNegOff;
                        }
                        else
                        {
                            if ((X2 + XDim2 + eqWallThick) > X1)
                            {
                                return Enums.TypeRoomConfig.verOff;
                            }
                            else
                            {
                                string ord = "Raum höherer Ord.";
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
                            if (Y1 + YDim1 + eqWallThick < Y2 || Y2 + YDim2 + eqWallThick < Y1)
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
                            if (Y1 + YDim1 + eqWallThick < Y2 || Y2 + YDim2 + eqWallThick < Y1)
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
                            if (Y1 + YDim1 + eqWallThick < Y2 || Y2 + XDim2 + eqWallThick < Y1)
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
            //Console.WriteLine(direcX);
            //Console.WriteLine(direcY);

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
                        /* else
                         {
                             roomConfig = Enums.TypeRoomConfig.verNegOff;
                             Console.WriteLine(roomConfig);
                         }
                     }
                     else
                     {
                         roomConfig = Enums.TypeRoomConfig.verNegOff;
                         Console.WriteLine(roomConfig);
                     }*/
                    }
                }
                /*  else if (Y1 != Y2)
                  {
                      if (direcY == Enums.TypeDirec.Ypos)
                      {
                          if (Y1 + YDim1 > Y2)
                          {
                              roomConfig = Enums.TypeRoomConfig.verNegOff;
                              Console.WriteLine(roomConfig);
                          }
                          else
                          {
                              if ((Y1 + YDim1 + 0.4) > Y2)
                              {
                                  roomConfig = Enums.TypeRoomConfig.verOff;
                                  Console.WriteLine(roomConfig);
                              }
                              else
                              {
                                  string ord = "Raum höherer Ord.";
                              }
                          }
                      }
                      else if (direcY == Enums.TypeDirec.Yneg)
                      {
                          if (Y2 + YDim2 > Y1)
                          {
                              roomConfig = Enums.TypeRoomConfig.verNegOff;
                              Console.WriteLine(roomConfig);
                          }
                          else
                          {
                              if ((Y2 + YDim2 + 0.4) > Y1)
                              {
                                  roomConfig = Enums.TypeRoomConfig.verOff;
                                  Console.WriteLine(roomConfig);
                              }
                              else
                              {
                                  string ord = "Raum höherer Ord.";
                              }
                          }
                      }
                  }*/
            }
            /* else if (X1 + XDim1 - 0.2 < X2 + XDim2 && X1 + XDim1 + 0.2 > X2 + XDim2 || X2 + XDim2 - 0.2 < X1 + XDim1 && X2 + XDim2 + 0.2 > X1 + XDim1) //==
             {
                 //bedeutet dass X1 != X2 sonst wäre es in erste Abfrage gegangen
                 if (Y1 - 0.2 < Y2 && Y1 + 0.2 > Y2 || Y2 - 0.2 < Y1 && Y2 + 0.2 > Y1) //==
                 {
                     roomConfig = Enums.TypeRoomConfig.verNegOff;
                     Console.WriteLine(roomConfig);
                 }
                 else if (direcY == Enums.TypeDirec.Ypos)
                 {
                     if (Y1 + YDim1 > Y2)
                     {
                         roomConfig = Enums.TypeRoomConfig.verNegOff;
                         Console.WriteLine(roomConfig);
                     }
                     else
                     {
                         if ((Y1 + YDim1 + 0.4) > Y2)
                         {
                             roomConfig = Enums.TypeRoomConfig.verOff;
                             Console.WriteLine(roomConfig);
                         }
                         else
                         {
                             string ord = "Raum höherer Ord.";
                         }
                     }
                 }
                 else if (direcY == Enums.TypeDirec.Yneg)
                 {
                     if (Y2 + YDim2 > Y1)
                     {
                         roomConfig = Enums.TypeRoomConfig.verNegOff;
                         Console.WriteLine(roomConfig);
                     }
                     else
                     {
                         if ((Y2 + YDim2 + 0.4) > Y1)
                         {
                             roomConfig = Enums.TypeRoomConfig.verOff;
                             Console.WriteLine(roomConfig);
                         }
                         else
                         {
                             string ord = "Raum höherer Ord.";
                         }
                     }
                 }

             }*/
            /* else if (Y1 + YDim1 - 0.2 < Y2 + YDim2 && Y1 + YDim1 + 0.2 > X2 + YDim2 || Y2 + YDim2 - 0.2 < Y1 + YDim1 && Y2 + YDim2 + 0.2 > Y1 + YDim1) //==
             {
                 //bedeutet dass X1 != X2 sonst wäre es in erste Abfrage gegangen
                 if (X1 - 0.2 < X2 && X1 + 0.2 > X2 || X2 - 0.2 < X1 && X2 + 0.2 > X1) //==
                 {
                     roomConfig = Enums.TypeRoomConfig.verNegOff;
                     Console.WriteLine(roomConfig);
                 }
                 else if (direcX == Enums.TypeDirec.Xpos)
                 {
                     if (X1 + XDim1 > X2)
                     {
                         roomConfig = Enums.TypeRoomConfig.verNegOff;
                         Console.WriteLine(roomConfig);
                     }
                     else
                     {
                         if ((X1 + XDim1 + 0.4) > X2)
                         {
                             roomConfig = Enums.TypeRoomConfig.verOff;
                             Console.WriteLine(roomConfig);
                         }
                         else
                         {
                             string ord = "Raum höherer Ord.";
                         }
                     }
                 }
                 else if (direcX == Enums.TypeDirec.Xneg)
                 {
                     if (X2 + XDim2 > X1)
                     {
                         roomConfig = Enums.TypeRoomConfig.verNegOff;
                         Console.WriteLine(roomConfig);
                     }
                     else
                     {
                         if ((X2 + XDim2 + 0.4) > X1)
                         {
                             roomConfig = Enums.TypeRoomConfig.verOff;
                             Console.WriteLine(roomConfig);
                         }
                         else
                         {
                             string ord = "Raum höherer Ord.";
                         }
                     }
                 }
             }
             else if (X1 != X2)
             {
                 if (Y1 - 0.2 < Y2 && Y1 + 0.2 > Y2 || Y2 - 0.2 < Y1 && Y2 + 0.2 > Y1) //==
                 {
                     if (direcX == Enums.TypeDirec.Xpos)
                     {
                         if (X1 + XDim1 > X2)
                         {
                             roomConfig = Enums.TypeRoomConfig.verNegOff;
                             Console.WriteLine(roomConfig);
                         }
                         else
                         {
                             if ((X1 + XDim1 + 0.4) > X2)
                             {
                                 roomConfig = Enums.TypeRoomConfig.verOff;
                                 Console.WriteLine(roomConfig);
                             }
                             else
                             {
                                 string ord = "Raum höherer Ord.";
                             }
                         }
                     }
                     if (direcX == Enums.TypeDirec.Xneg)
                     {
                         if (X2 + XDim2 > X1)
                         {
                             roomConfig = Enums.TypeRoomConfig.verNegOff;
                             Console.WriteLine(roomConfig);
                         }
                         else
                         {
                             if ((X2 + XDim2 + 0.4) > X1)
                             {
                                 roomConfig = Enums.TypeRoomConfig.verOff;
                                 Console.WriteLine(roomConfig);
                             }
                             else
                             {
                                 string ord = "Raum höherer Ord.";
                             }
                         }
                     }
                 }
                 else if (Y1 != Y2)
                 {
                     if (direcX == Enums.TypeDirec.Xpos && direcY == Enums.TypeDirec.Ypos)
                     {
                         if (((Y1 + YDim1 + 0.4) > Y2) && ((Y1 + YDim1) < Y2) && ((X1 + XDim1 + 0.4) > X2) && ((X1 + XDim1) < X2))
                         {
                             roomConfig = Enums.TypeRoomConfig.verOff;
                             Console.WriteLine(roomConfig);
                         }
                         else if ((Y1 + YDim1) > Y2 && (X1 + XDim1) > X2)
                         {
                             roomConfig = Enums.TypeRoomConfig.verNegOff;
                             Console.WriteLine(roomConfig);
                         }
                         else if (((X1 + XDim1 + 0.4) > X2) && ((X1 + XDim1) < X2))
                         {
                             if (Y1 + YDim1 + 0.4 < Y2 || Y2 + YDim2 + 0.4 < Y1)
                             {
                                 roomConfig = Enums.TypeRoomConfig.verPosOff;
                                 Console.WriteLine(roomConfig);
                             }
                             else
                             {
                                 roomConfig = Enums.TypeRoomConfig.verNegOff;
                                 Console.WriteLine(roomConfig);
                             }
                         }
                         else if (((Y1 + YDim1 + 0.4) > Y2) && ((Y1 + YDim1) < Y2))
                         {
                             if (X1 + XDim1 + 0.4 < X2 || X2 + XDim2 + 0.4 < X1)
                             {
                                 roomConfig = Enums.TypeRoomConfig.verPosOff;
                                 Console.WriteLine(roomConfig);
                             }
                             else
                             {
                                 roomConfig = Enums.TypeRoomConfig.verNegOff;
                                 Console.WriteLine(roomConfig);
                             }
                         }
                     }
                    else if (direcX == Enums.TypeDirec.Xpos && direcY == Enums.TypeDirec.Yneg)
                    {
                        if (((Y2 + YDim2 + 0.4) > Y1) && ((Y2 + YDim2) < Y1) && ((X1 + XDim1 + 0.4) > X2) && ((X1 + XDim1) < X2))
                        {
                            roomConfig = Enums.TypeRoomConfig.verOff;
                            Console.WriteLine(roomConfig);
                        }
                        else if ((Y2 + YDim2) > Y1 && (X1 + XDim1) > X2)
                        {
                            roomConfig = Enums.TypeRoomConfig.verNegOff;
                            Console.WriteLine(roomConfig);
                        }
                        else if ((X1 + XDim1 + 0.4) > X2 && ((X1 + XDim1) < X2))
                        {
                            if (Y1 + YDim1 + 0.4 < Y2 || Y2 + YDim2 + 0.4 < Y1)
                            {
                                roomConfig = Enums.TypeRoomConfig.verPosOff;
                                Console.WriteLine(roomConfig);
                            }
                            else
                            {
                                roomConfig = Enums.TypeRoomConfig.verNegOff;
                                Console.WriteLine(roomConfig);
                            }
                        }
                        else if ((Y2 + YDim2 + 0.4) > Y1 && ((Y2 + YDim2) < Y1))
                        {
                            if (X1 + XDim1 + 0.4 < X2 || X2 + XDim2 + 0.4 < X1)
                            {
                                roomConfig = Enums.TypeRoomConfig.verPosOff;
                                Console.WriteLine(roomConfig);
                            }
                            else
                            {
                                roomConfig = Enums.TypeRoomConfig.verNegOff;
                                Console.WriteLine(roomConfig);
                            }
                        }
                    }
                    else if (direcX == Enums.TypeDirec.Xneg && direcY == Enums.TypeDirec.Ypos)
                    {
                        if (((Y1 + YDim1 + 0.4) > Y2) && ((Y1 + YDim1) < Y2) && ((X2 + XDim2 + 0.4) > X2) && ((X2 + XDim2) < X2))
                        {
                            roomConfig = Enums.TypeRoomConfig.verOff;
                            Console.WriteLine(roomConfig);
                        }
                        else if ((Y1 + YDim1) > Y2 && (X2 + XDim2) > X1)
                        {
                            roomConfig = Enums.TypeRoomConfig.verNegOff;
                            Console.WriteLine(roomConfig);
                        }
                        else if (((X2 + XDim2 + 0.4) > X1) && (X2 + XDim2) < X1)
                        {
                            if (Y1 + YDim1 + 0.4 < Y2 || Y2 + XDim2 + 0.4 < Y1)
                            {
                                roomConfig = Enums.TypeRoomConfig.verPosOff;
                                Console.WriteLine(roomConfig);
                            }
                            else
                            {
                                roomConfig = Enums.TypeRoomConfig.verNegOff;
                                Console.WriteLine(roomConfig);
                            }
                        }
                        else if (((Y1 + YDim1 + 0.4) > Y2) && ((Y1 + YDim1) < Y2))
                        {
                            if (X1 + XDim1 + 0.4 < X2 || X2 + XDim2 + 0.4 < X1)
                            {
                                roomConfig = Enums.TypeRoomConfig.verPosOff;
                                Console.WriteLine(roomConfig);
                            }
                            else
                            {
                                roomConfig = Enums.TypeRoomConfig.verNegOff;
                                Console.WriteLine(roomConfig);
                            }
                        }
                    }
                    else if (direcX == Enums.TypeDirec.Xneg && direcY == Enums.TypeDirec.Yneg)
                    {
                        if (((Y2 + YDim2 + 0.4) > Y1) && ((Y2 + YDim2) < Y1) && ((X2 + XDim2 + 0.4) > X2) && ((X2 + XDim2) < X2))
                        {
                            roomConfig = Enums.TypeRoomConfig.verOff;
                            Console.WriteLine(roomConfig);
                        }
                        else if ((Y2 + YDim2) > Y1 && (X2 + XDim2) > X1)
                        {
                            roomConfig = Enums.TypeRoomConfig.verNegOff;
                            Console.WriteLine(roomConfig);
                        }
                        else if (((X2 + XDim2 + 0.4) > X1) && (X2 + XDim2) < X1)
                        {
                            if (Y2 + YDim2 + 0.4 < Y1 || Y1 + XDim1 + 0.4 < Y2)
                            {
                                roomConfig = Enums.TypeRoomConfig.verPosOff;
                                Console.WriteLine(roomConfig);
                            }
                            else
                            {
                                roomConfig = Enums.TypeRoomConfig.verNegOff;
                                Console.WriteLine(roomConfig);
                            }
                        }
                        else if (((Y2 + YDim2 + 0.4) > Y1) && ((Y2 + YDim2) < Y1))
                        {
                            if (X1 + XDim1 + 0.4 < X2 || X2 + XDim2 + 0.4 < X1)
                            {
                                roomConfig = Enums.TypeRoomConfig.verPosOff;
                                Console.WriteLine(roomConfig);
                            }
                            else
                            {
                                roomConfig = Enums.TypeRoomConfig.verNegOff;
                                Console.WriteLine(roomConfig);
                            }
                        }
                    }
                }
            }*/
            return roomConfig;
        }
        public Enums.TypeRoomConfig TryVerticalConfigsThirdimension(double X1, double Y1, double XDim1, double YDim1, double X2, double Y2, double XDim2, double YDim2)
        {
            var geo = new GeometryHandler();
            Enums.TypeRoomConfig roomConfig = new Enums.TypeRoomConfig();

            var direcX = geo.GetDirection(X1, X2, 0, 0);
            var direcY = geo.GetDirection(0, 0, Y1, Y2);
            //Console.WriteLine(direcX);
            //Console.WriteLine(direcY);

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
                        /* else
                         {
                             roomConfig = Enums.TypeRoomConfig.verNegOff;
                             Console.WriteLine(roomConfig);
                         }
                     }
                     else
                     {
                         roomConfig = Enums.TypeRoomConfig.verNegOff;
                         Console.WriteLine(roomConfig);
                     }*/
                    }
                }
                /*  else if (Y1 != Y2)
                  {
                      if (direcY == Enums.TypeDirec.Ypos)
                      {
                          if (Y1 + YDim1 > Y2)
                          {
                              roomConfig = Enums.TypeRoomConfig.verNegOff;
                              Console.WriteLine(roomConfig);
                          }
                          else
                          {
                              if ((Y1 + YDim1 + 0.4) > Y2)
                              {
                                  roomConfig = Enums.TypeRoomConfig.verOff;
                                  Console.WriteLine(roomConfig);
                              }
                              else
                              {
                                  string ord = "Raum höherer Ord.";
                              }
                          }
                      }
                      else if (direcY == Enums.TypeDirec.Yneg)
                      {
                          if (Y2 + YDim2 > Y1)
                          {
                              roomConfig = Enums.TypeRoomConfig.verNegOff;
                              Console.WriteLine(roomConfig);
                          }
                          else
                          {
                              if ((Y2 + YDim2 + 0.4) > Y1)
                              {
                                  roomConfig = Enums.TypeRoomConfig.verOff;
                                  Console.WriteLine(roomConfig);
                              }
                              else
                              {
                                  string ord = "Raum höherer Ord.";
                              }
                          }
                      }
                  }*/
            }
            /* else if (X1 + XDim1 - 0.2 < X2 + XDim2 && X1 + XDim1 + 0.2 > X2 + XDim2 || X2 + XDim2 - 0.2 < X1 + XDim1 && X2 + XDim2 + 0.2 > X1 + XDim1) //==
             {
                 //bedeutet dass X1 != X2 sonst wäre es in erste Abfrage gegangen
                 if (Y1 - 0.2 < Y2 && Y1 + 0.2 > Y2 || Y2 - 0.2 < Y1 && Y2 + 0.2 > Y1) //==
                 {
                     roomConfig = Enums.TypeRoomConfig.verNegOff;
                     Console.WriteLine(roomConfig);
                 }
                 else if (direcY == Enums.TypeDirec.Ypos)
                 {
                     if (Y1 + YDim1 > Y2)
                     {
                         roomConfig = Enums.TypeRoomConfig.verNegOff;
                         Console.WriteLine(roomConfig);
                     }
                     else
                     {
                         if ((Y1 + YDim1 + 0.4) > Y2)
                         {
                             roomConfig = Enums.TypeRoomConfig.verOff;
                             Console.WriteLine(roomConfig);
                         }
                         else
                         {
                             string ord = "Raum höherer Ord.";
                         }
                     }
                 }
                 else if (direcY == Enums.TypeDirec.Yneg)
                 {
                     if (Y2 + YDim2 > Y1)
                     {
                         roomConfig = Enums.TypeRoomConfig.verNegOff;
                         Console.WriteLine(roomConfig);
                     }
                     else
                     {
                         if ((Y2 + YDim2 + 0.4) > Y1)
                         {
                             roomConfig = Enums.TypeRoomConfig.verOff;
                             Console.WriteLine(roomConfig);
                         }
                         else
                         {
                             string ord = "Raum höherer Ord.";
                         }
                     }
                 }

             }*/
            /* else if (Y1 + YDim1 - 0.2 < Y2 + YDim2 && Y1 + YDim1 + 0.2 > X2 + YDim2 || Y2 + YDim2 - 0.2 < Y1 + YDim1 && Y2 + YDim2 + 0.2 > Y1 + YDim1) //==
             {
                 //bedeutet dass X1 != X2 sonst wäre es in erste Abfrage gegangen
                 if (X1 - 0.2 < X2 && X1 + 0.2 > X2 || X2 - 0.2 < X1 && X2 + 0.2 > X1) //==
                 {
                     roomConfig = Enums.TypeRoomConfig.verNegOff;
                     Console.WriteLine(roomConfig);
                 }
                 else if (direcX == Enums.TypeDirec.Xpos)
                 {
                     if (X1 + XDim1 > X2)
                     {
                         roomConfig = Enums.TypeRoomConfig.verNegOff;
                         Console.WriteLine(roomConfig);
                     }
                     else
                     {
                         if ((X1 + XDim1 + 0.4) > X2)
                         {
                             roomConfig = Enums.TypeRoomConfig.verOff;
                             Console.WriteLine(roomConfig);
                         }
                         else
                         {
                             string ord = "Raum höherer Ord.";
                         }
                     }
                 }
                 else if (direcX == Enums.TypeDirec.Xneg)
                 {
                     if (X2 + XDim2 > X1)
                     {
                         roomConfig = Enums.TypeRoomConfig.verNegOff;
                         Console.WriteLine(roomConfig);
                     }
                     else
                     {
                         if ((X2 + XDim2 + 0.4) > X1)
                         {
                             roomConfig = Enums.TypeRoomConfig.verOff;
                             Console.WriteLine(roomConfig);
                         }
                         else
                         {
                             string ord = "Raum höherer Ord.";
                         }
                     }
                 }
             }
             else if (X1 != X2)
             {
                 if (Y1 - 0.2 < Y2 && Y1 + 0.2 > Y2 || Y2 - 0.2 < Y1 && Y2 + 0.2 > Y1) //==
                 {
                     if (direcX == Enums.TypeDirec.Xpos)
                     {
                         if (X1 + XDim1 > X2)
                         {
                             roomConfig = Enums.TypeRoomConfig.verNegOff;
                             Console.WriteLine(roomConfig);
                         }
                         else
                         {
                             if ((X1 + XDim1 + 0.4) > X2)
                             {
                                 roomConfig = Enums.TypeRoomConfig.verOff;
                                 Console.WriteLine(roomConfig);
                             }
                             else
                             {
                                 string ord = "Raum höherer Ord.";
                             }
                         }
                     }
                     if (direcX == Enums.TypeDirec.Xneg)
                     {
                         if (X2 + XDim2 > X1)
                         {
                             roomConfig = Enums.TypeRoomConfig.verNegOff;
                             Console.WriteLine(roomConfig);
                         }
                         else
                         {
                             if ((X2 + XDim2 + 0.4) > X1)
                             {
                                 roomConfig = Enums.TypeRoomConfig.verOff;
                                 Console.WriteLine(roomConfig);
                             }
                             else
                             {
                                 string ord = "Raum höherer Ord.";
                             }
                         }
                     }
                 }
                 else if (Y1 != Y2)
                 {
                     if (direcX == Enums.TypeDirec.Xpos && direcY == Enums.TypeDirec.Ypos)
                     {
                         if (((Y1 + YDim1 + 0.4) > Y2) && ((Y1 + YDim1) < Y2) && ((X1 + XDim1 + 0.4) > X2) && ((X1 + XDim1) < X2))
                         {
                             roomConfig = Enums.TypeRoomConfig.verOff;
                             Console.WriteLine(roomConfig);
                         }
                         else if ((Y1 + YDim1) > Y2 && (X1 + XDim1) > X2)
                         {
                             roomConfig = Enums.TypeRoomConfig.verNegOff;
                             Console.WriteLine(roomConfig);
                         }
                         else if (((X1 + XDim1 + 0.4) > X2) && ((X1 + XDim1) < X2))
                         {
                             if (Y1 + YDim1 + 0.4 < Y2 || Y2 + YDim2 + 0.4 < Y1)
                             {
                                 roomConfig = Enums.TypeRoomConfig.verPosOff;
                                 Console.WriteLine(roomConfig);
                             }
                             else
                             {
                                 roomConfig = Enums.TypeRoomConfig.verNegOff;
                                 Console.WriteLine(roomConfig);
                             }
                         }
                         else if (((Y1 + YDim1 + 0.4) > Y2) && ((Y1 + YDim1) < Y2))
                         {
                             if (X1 + XDim1 + 0.4 < X2 || X2 + XDim2 + 0.4 < X1)
                             {
                                 roomConfig = Enums.TypeRoomConfig.verPosOff;
                                 Console.WriteLine(roomConfig);
                             }
                             else
                             {
                                 roomConfig = Enums.TypeRoomConfig.verNegOff;
                                 Console.WriteLine(roomConfig);
                             }
                         }
                     }
                    else if (direcX == Enums.TypeDirec.Xpos && direcY == Enums.TypeDirec.Yneg)
                    {
                        if (((Y2 + YDim2 + 0.4) > Y1) && ((Y2 + YDim2) < Y1) && ((X1 + XDim1 + 0.4) > X2) && ((X1 + XDim1) < X2))
                        {
                            roomConfig = Enums.TypeRoomConfig.verOff;
                            Console.WriteLine(roomConfig);
                        }
                        else if ((Y2 + YDim2) > Y1 && (X1 + XDim1) > X2)
                        {
                            roomConfig = Enums.TypeRoomConfig.verNegOff;
                            Console.WriteLine(roomConfig);
                        }
                        else if ((X1 + XDim1 + 0.4) > X2 && ((X1 + XDim1) < X2))
                        {
                            if (Y1 + YDim1 + 0.4 < Y2 || Y2 + YDim2 + 0.4 < Y1)
                            {
                                roomConfig = Enums.TypeRoomConfig.verPosOff;
                                Console.WriteLine(roomConfig);
                            }
                            else
                            {
                                roomConfig = Enums.TypeRoomConfig.verNegOff;
                                Console.WriteLine(roomConfig);
                            }
                        }
                        else if ((Y2 + YDim2 + 0.4) > Y1 && ((Y2 + YDim2) < Y1))
                        {
                            if (X1 + XDim1 + 0.4 < X2 || X2 + XDim2 + 0.4 < X1)
                            {
                                roomConfig = Enums.TypeRoomConfig.verPosOff;
                                Console.WriteLine(roomConfig);
                            }
                            else
                            {
                                roomConfig = Enums.TypeRoomConfig.verNegOff;
                                Console.WriteLine(roomConfig);
                            }
                        }
                    }
                    else if (direcX == Enums.TypeDirec.Xneg && direcY == Enums.TypeDirec.Ypos)
                    {
                        if (((Y1 + YDim1 + 0.4) > Y2) && ((Y1 + YDim1) < Y2) && ((X2 + XDim2 + 0.4) > X2) && ((X2 + XDim2) < X2))
                        {
                            roomConfig = Enums.TypeRoomConfig.verOff;
                            Console.WriteLine(roomConfig);
                        }
                        else if ((Y1 + YDim1) > Y2 && (X2 + XDim2) > X1)
                        {
                            roomConfig = Enums.TypeRoomConfig.verNegOff;
                            Console.WriteLine(roomConfig);
                        }
                        else if (((X2 + XDim2 + 0.4) > X1) && (X2 + XDim2) < X1)
                        {
                            if (Y1 + YDim1 + 0.4 < Y2 || Y2 + XDim2 + 0.4 < Y1)
                            {
                                roomConfig = Enums.TypeRoomConfig.verPosOff;
                                Console.WriteLine(roomConfig);
                            }
                            else
                            {
                                roomConfig = Enums.TypeRoomConfig.verNegOff;
                                Console.WriteLine(roomConfig);
                            }
                        }
                        else if (((Y1 + YDim1 + 0.4) > Y2) && ((Y1 + YDim1) < Y2))
                        {
                            if (X1 + XDim1 + 0.4 < X2 || X2 + XDim2 + 0.4 < X1)
                            {
                                roomConfig = Enums.TypeRoomConfig.verPosOff;
                                Console.WriteLine(roomConfig);
                            }
                            else
                            {
                                roomConfig = Enums.TypeRoomConfig.verNegOff;
                                Console.WriteLine(roomConfig);
                            }
                        }
                    }
                    else if (direcX == Enums.TypeDirec.Xneg && direcY == Enums.TypeDirec.Yneg)
                    {
                        if (((Y2 + YDim2 + 0.4) > Y1) && ((Y2 + YDim2) < Y1) && ((X2 + XDim2 + 0.4) > X2) && ((X2 + XDim2) < X2))
                        {
                            roomConfig = Enums.TypeRoomConfig.verOff;
                            Console.WriteLine(roomConfig);
                        }
                        else if ((Y2 + YDim2) > Y1 && (X2 + XDim2) > X1)
                        {
                            roomConfig = Enums.TypeRoomConfig.verNegOff;
                            Console.WriteLine(roomConfig);
                        }
                        else if (((X2 + XDim2 + 0.4) > X1) && (X2 + XDim2) < X1)
                        {
                            if (Y2 + YDim2 + 0.4 < Y1 || Y1 + XDim1 + 0.4 < Y2)
                            {
                                roomConfig = Enums.TypeRoomConfig.verPosOff;
                                Console.WriteLine(roomConfig);
                            }
                            else
                            {
                                roomConfig = Enums.TypeRoomConfig.verNegOff;
                                Console.WriteLine(roomConfig);
                            }
                        }
                        else if (((Y2 + YDim2 + 0.4) > Y1) && ((Y2 + YDim2) < Y1))
                        {
                            if (X1 + XDim1 + 0.4 < X2 || X2 + XDim2 + 0.4 < X1)
                            {
                                roomConfig = Enums.TypeRoomConfig.verPosOff;
                                Console.WriteLine(roomConfig);
                            }
                            else
                            {
                                roomConfig = Enums.TypeRoomConfig.verNegOff;
                                Console.WriteLine(roomConfig);
                            }
                        }
                    }
                }
            }*/
            return roomConfig;
        }
    }
}
