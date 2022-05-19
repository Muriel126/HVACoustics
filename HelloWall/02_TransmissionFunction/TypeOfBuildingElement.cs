using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.Ifc;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.ProductExtension;

namespace HVACoustics
{
    class TypeOfBuildingElement
    {
        public static string GetTypeOfBuildingElement(IfcStore model, string globalIdConnectedBuildingElement)
        {
            string typeOfBuildingElement = "";

            IIfcElement theElement = model.Instances.FirstOrDefault<IIfcElement>(d => d.GlobalId == globalIdConnectedBuildingElement);
            //typeOfBuildingElement = theElement.ObjectType.ToString();  //nach Revit-Familien benannt

            var properties = theElement.IsDefinedBy.Where(r => r.RelatingPropertyDefinition is IIfcPropertySet)
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
            }

            IIfcBuildingStorey storey = theElement.IsContainedIn as IfcBuildingStorey;
            var building = storey.Decomposes.Select(x => x.RelatingObject as IIfcBuilding);
            var numberOfBuildings = building.Count();
            


            return typeOfBuildingElement;
        }
    }
}
