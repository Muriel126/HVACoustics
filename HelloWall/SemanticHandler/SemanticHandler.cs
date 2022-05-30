using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.Ifc;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.ProductExtension;
using Xbim.Ifc4.SharedBldgElements;

namespace HVACoustics.SemanticHandler
{
    class SemanticHandler
    {
        public IIfcMaterialLayerSet GetMaterialLayerSet(IfcStore model, string globalIdConnectedBuildingElement)
        {
            IIfcElement theWall = model.Instances.FirstOrDefault<IIfcElement>(d => d.GlobalId == globalIdConnectedBuildingElement);
            IIfcMaterialLayerSetUsage AllLayerSetsUsage = theWall.Material as IIfcMaterialLayerSetUsage;
            IIfcMaterialLayerSet materialLayerSet = AllLayerSetsUsage.ForLayerSet;

            return materialLayerSet;
        }
        public string GetBuildingElementClassXbim(IfcStore model, IIfcBuildingElement element)
        {
            string completeXbimClass = element.GetType().ToString();
            var partsOfClassName = completeXbimClass.Split('.');
            var length = completeXbimClass.Split('.').Length;
            var elementClass = partsOfClassName[length - 1];

            return elementClass;
        }
        public IfcWallTypeEnum GetTypeOfBuildingElement(IfcStore model, string globalIdConnectedBuildingElement)
        {
            var sem = new SemanticHandler();

            IIfcBuildingElement theElement = model.Instances.FirstOrDefault<IIfcBuildingElement>(d => d.GlobalId == globalIdConnectedBuildingElement);
            var classElement = sem.GetBuildingElementClassXbim(model, theElement);

            if (classElement == "IfcWall" || classElement == "IfcWallStandardCase")
            {
                IfcWall theWall = model.Instances.FirstOrDefault<IfcWall>(d => d.GlobalId == globalIdConnectedBuildingElement);
                var type = theWall.PredefinedType;
            }
            if (classElement == "IfcSlab" || classElement == "IfSlabStandardCase")
            {
                IfcSlab theSlab = model.Instances.FirstOrDefault<IfcSlab>(d => d.GlobalId == globalIdConnectedBuildingElement);
                var type = theSlab.PredefinedType;
            }
            return IfcWallTypeEnum.NOTDEFINED;
        }
    }
}
