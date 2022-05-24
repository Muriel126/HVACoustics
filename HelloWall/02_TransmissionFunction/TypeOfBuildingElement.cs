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
        public static bool Basement { get; set; }
        

        public static Enums.TypeBuildingElement GetTypeOfBuildingElement(IfcStore model, string globalIdConnectedBuildingElement)
        {
            var geo = new GeometryHandler();

            IIfcBuildingElement theElement = model.Instances.FirstOrDefault<IIfcBuildingElement>(d => d.GlobalId == globalIdConnectedBuildingElement);
            List<string> listBoundedSpaces = new List<string>();
            List<string> controlListBoundedSpaces = new List<string>();
            Dictionary<string, Enums.TypeDirec> dictSpaceDirec = new Dictionary<string, Enums.TypeDirec>();
            Dictionary<string, string> dictZoneSpace = new Dictionary<string, string>();
            
            //Geometrie des Bauteils über eine BoundingBox feststellen
            XbimRect3D bbElement = geo.CreateBoundingBoxAroundBuildingElement(model, theElement);
            var xElement = bbElement.X;
            var yElement = bbElement.Y;
            var xDimElement = bbElement.SizeX;
            var yDimElement = bbElement.SizeY;

            //im ersten Schritt prüfen, ob es sich um eine Decke/Boden handelt über die Klasse IfcSlab oder IfcSlabStandardCase
            string completeXbimClass = theElement.GetType().ToString();
            var partsOfClassName = completeXbimClass.Split('.');
            var length = completeXbimClass.Split('.').Length;
            var elementClass = partsOfClassName[length-1];
            if (elementClass.ToString() == "IfcSlab" || elementClass.ToString() == "IfcSlabStandardCase")
            {
                return Enums.TypeBuildingElement.Floor;
            }

            //im nächsten Schritt werden nacheinander die verschiedenen Wandarten geprüft
            //zunächst wird geprüft, ob es sich um eine Außenwand handelt, indem die die Lage der angrenzenden Räume betrachtet wird
            //liegen alle auf einer Seite des Elements, handelt es sich um eine Außenwand
            var storey = theElement.IsContainedIn as IfcBuildingStorey;
            if (storey.Elevation < 0)
            {
                Basement = true;
            }

            var boundaries = theElement.ProvidesBoundaries;
            foreach(var b in boundaries)
            {
                IfcSpace space = b.RelatingSpace as IfcSpace;
                listBoundedSpaces.Add(space.GlobalId);
            }
            if (listBoundedSpaces.Any() == false && Basement == true)
            {
                return Enums.TypeBuildingElement.BasementWallExterior;
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

            var firstEntry = dictSpaceDirec[listBoundedSpaces[0]];

            for (int i=0; i < numOfSpaces; i++)
            {
                if (dictSpaceDirec[listBoundedSpaces[i]] != firstEntry)
                {
                    break;
                }
                if (i == numOfSpaces - 1 && Basement == true)
                {
                    return Enums.TypeBuildingElement.BasementWallExterior;
                }
                if (i == numOfSpaces-1)
                {
                    return Enums.TypeBuildingElement.ExteriorWall;
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

            //Überprüfung Gebäudetrennwand, wenn nicht dann als nächstes Wohnungstrennwand
            var references = theElement.ReferencedInStructures;
            if (references.Any() == true)
            {
                return Enums.TypeBuildingElement.HousePartyWall;
            }
            else
            {
                var relZones = model.Instances.OfType<IIfcRelAssignsToGroup>().Where(r => r.RelatingGroup is IIfcZone).ToList();
                foreach (var zone in relZones)
                {
                    foreach (var space in zone.RelatedObjects)
                    {
                        dictZoneSpace.Add(space.GlobalId, zone.RelatingGroup.Name);
                    }
                }
                var firstEntryZones = dictZoneSpace[listBoundedSpaces[0]];
                for (int i = 0; i < numOfSpaces; i++)
                {
                    if (firstEntryZones == dictZoneSpace[listBoundedSpaces[i]])
                    {
                        controlListBoundedSpaces.Add(listBoundedSpaces[i]); 
                    }                   
                }
                if (listBoundedSpaces.Count() == controlListBoundedSpaces.Count() && Basement == true)
                {
                    return Enums.TypeBuildingElement.BasementWallInterior;
                }
                if (listBoundedSpaces.Count() == controlListBoundedSpaces.Count())
                {
                    return Enums.TypeBuildingElement.InteriorWall;
                }
                else
                {
                    return Enums.TypeBuildingElement.PartyWall;
                }
            }
        }

        public static string GetDefiningTypeOfBuildingElement(IfcStore model, string globalIdConnectedBuildingElement)
        {
            IIfcBuildingElement theElement = model.Instances.FirstOrDefault<IIfcBuildingElement>(d => d.GlobalId == globalIdConnectedBuildingElement);

            var definingTypeOfBuildingElement = theElement.ObjectType.ToString();  //nach Revit-Familien benannt

            return definingTypeOfBuildingElement;
        }
    }
}
