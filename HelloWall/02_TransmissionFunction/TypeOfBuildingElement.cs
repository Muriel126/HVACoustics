using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.Common.Geometry;
using Xbim.Ifc;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.ProductExtension;
using static HVACoustics.TypeOfBuildingElement;

namespace HVACoustics
{
    class TypeOfBuildingElement
    {
        public static Enums.TypeDirec Direc { get; set; }

        public static Enums.TypeBuildingElement GetTypeOfBuildingElement(IfcStore model, string globalIdConnectedBuildingElement)
        {
            var geo = new GeometryHandler();

            List<string> listBoundedSpaces = new List<string>();
            Dictionary<string, Enums.TypeDirec> dictSpaceDirec = new Dictionary<string, Enums.TypeDirec>();
            IIfcBuildingElement theElement = model.Instances.FirstOrDefault<IIfcBuildingElement>(d => d.GlobalId == globalIdConnectedBuildingElement);
            XbimRect3D bbElement = geo.CreateBoundingBoxAroundBuildingElement(model, theElement);
            var xElement = bbElement.X;
            var yElement = bbElement.Y;
            var xDimElement = bbElement.SizeX;
            var yDimElement = bbElement.SizeY;

            //typeOfBuildingElement = theElement.ObjectType.ToString();  //nach Revit-Familien benannt

            var boundaries = theElement.ProvidesBoundaries;
            foreach(var b in boundaries)
            {
                IfcSpace space = b.RelatingSpace as IfcSpace;
                listBoundedSpaces.Add(space.GlobalId);
            }
            if (listBoundedSpaces.Any() == false)
            {
                return Enums.TypeBuildingElement.ExteriorWall;
            }
            foreach (var id in listBoundedSpaces)
            {
                IIfcSpace space = model.Instances.FirstOrDefault<IIfcSpace>(d => d.GlobalId == id);
                var bbSpace = geo.CreateBoundingBoxAroundSpace(model, space);
                var xSpace = bbSpace.X;
                var ySpace = bbSpace.Y;

                if(xDimElement > yDimElement)
                {
                    if (yElement < ySpace)
                    {
                        Direc = Enums.TypeDirec.Ypos;
                    }
                    if (yElement > ySpace)
                    {
                        Direc = Enums.TypeDirec.Yneg;
                    }
                }
                if (yDimElement > xDimElement)
                {
                    if (xElement < xSpace)
                    {
                        Direc = Enums.TypeDirec.Xpos;
                    }
                    if (xElement > xSpace)
                    {
                        Direc = Enums.TypeDirec.Xneg;
                    }
                }
                if (dictSpaceDirec.ContainsKey(id))
                {
                    continue;
                }
                dictSpaceDirec.Add(id, Direc);
            }
            int numOfSpaces = listBoundedSpaces.Count();
            foreach (KeyValuePair<string, Enums.TypeDirec> kvp in dictSpaceDirec)
            {
                Console.WriteLine("{0},  {1}", kvp.Key, kvp.Value);
            }

            var firstEntry = dictSpaceDirec[listBoundedSpaces[0]];
            Console.WriteLine(firstEntry);
            for (int i=0; i < numOfSpaces; i++)
            {
                Console.WriteLine(dictSpaceDirec[listBoundedSpaces[i]]);
                if (dictSpaceDirec[listBoundedSpaces[i]] != firstEntry)
                {
                    break;
                }
                if (i == numOfSpaces-1)
                {
                    return Enums.TypeBuildingElement.ExteriorWall;
                }

            }

            var references = theElement.ReferencedInStructures;
            foreach (var v in references)
            {
                if (v == null)
                {
                    Console.WriteLine("GOT YA!");
                }
                
            }

            /*var properties = theElement.IsDefinedBy.Where(r => r.RelatingPropertyDefinition is IIfcPropertySet)
                .SelectMany(r => ((IIfcPropertySet)r.RelatingPropertyDefinition).HasProperties)
                .OfType<IIfcPropertySingleValue>();

            foreach (var p in properties)
            {
                if (p.Name == "IsExternal")
                {
                    if (p.NominalValue.ToString() == "true")
                    {
                        typeOfBuildingElement = "Aussenwand";
                        //return typeOfBuildingElement;
                    }                   
                }   
            }*/

            IIfcBuildingStorey storey = theElement.IsContainedIn as IfcBuildingStorey;
            var building = storey.Decomposes.Select(x => x.RelatingObject as IIfcBuilding);
            var numberOfBuildings = building.Count();

            return Enums.TypeBuildingElement.Default;
        }
    }
}
