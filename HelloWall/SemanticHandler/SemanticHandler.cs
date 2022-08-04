using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.Ifc;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.ProductExtension;
using Xbim.Ifc4.SharedBldgElements;

namespace HVACoustics
{
    class SemanticHandler
    {
        public IIfcMaterialLayerSet GetMaterialLayerSet(IfcStore model, string globalIdConnectedBuildingElement)
        {
            IIfcElement theElement = model.Instances.FirstOrDefault<IIfcElement>(d => d.GlobalId == globalIdConnectedBuildingElement);
            IIfcMaterialLayerSetUsage AllLayerSetsUsage = theElement.Material as IIfcMaterialLayerSetUsage;
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
        public static IfcWallTypeEnum GetTypeOfBuildingElement(IfcStore model, string globalIdConnectedBuildingElement)
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
        public string GetGlobalIdOfConnectingBuildingElementOfTwoSpaces(IfcStore model, string globalIdSender, string globalIdReciever)
        {
            var sem = new SemanticHandler();

            List<string> listBoundedElementsReciever = new List<string>();
            List<string> listBoundedElementsSender = new List<string>();

            IIfcSpace recieverSpace = model.Instances.FirstOrDefault<IIfcSpace>(d => d.GlobalId == globalIdReciever);
            IIfcSpace senderSpace = model.Instances.FirstOrDefault<IIfcSpace>(d => d.GlobalId == globalIdSender);

            var relBoundedElementsReciever = recieverSpace.BoundedBy;
            var boundedElementReciever = relBoundedElementsReciever.Select(x => x.RelatedBuildingElement);
            
            var relBoundedElementsSender = senderSpace.BoundedBy;
            var boundedElementSender = relBoundedElementsSender.Select(x => x.RelatedBuildingElement);

            foreach (var e in boundedElementReciever)
            {
                if (listBoundedElementsReciever.Contains(e.GlobalId) == false)
                {
                    listBoundedElementsReciever.Add(e.GlobalId);
                }
            }
            foreach (var e in boundedElementSender)
            {
                if (listBoundedElementsSender.Contains(e.GlobalId) == false)
                {
                    listBoundedElementsSender.Add(e.GlobalId);
                }
            }
            foreach (var s in listBoundedElementsReciever)
            {
                if (listBoundedElementsSender.Contains(s))
                {
                    return s;
                }
            }
            return "default";
        }
    }
}
