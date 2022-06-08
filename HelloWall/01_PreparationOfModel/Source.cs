using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.Ifc;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.ProductExtension;

namespace HVACoustics
{
    class Source
    {
        public static string globalIdConnectedBuildingElement1 { get; set; }
        public static string globalIdConnectedBuildingElement2 { get; set; }
        public static string globalIdConnectedBuildingElement3 { get; set; }
        public static string globalIdSource { get; set; }
        public static string globalIdSender { get; set; }

        public static string GetSpaceWithSource(IfcStore model)
        {
            Console.WriteLine("Please enter the GlobalId of the source:");
            globalIdSource = Console.ReadLine();
            IfcDistributionElement source = model.Instances.FirstOrDefault<IfcDistributionElement>(d => d.GlobalId == globalIdSource);

            IfcSpace senderRoom = source.IsContainedIn as IfcSpace;

            globalIdSender = senderRoom.GlobalId;

            return globalIdSender;
        }

        public static IfcRelAssignsToProduct CreateRelSourceToBuildingElement(IfcStore model, string globalIdSource, int numberOfConnectedBuildingElements)
        {
            using (var txn = model.BeginTransaction("Create relation between source and building element"))
            {
                IfcDistributionElement source = model.Instances.FirstOrDefault<IfcDistributionElement>(d => d.GlobalId == globalIdSource);

                var relAssigns = model.Instances.New<IfcRelAssignsToProduct>();
                relAssigns.GlobalId = Guid.NewGuid();
                relAssigns.RelatingProduct = source;

                Console.WriteLine("Now enter the first connected building element to this source:");
                globalIdConnectedBuildingElement1 = Console.ReadLine();
                IfcBuildingElement buildingElement1 = model.Instances.FirstOrDefault<IfcBuildingElement>(d => d.GlobalId == globalIdConnectedBuildingElement1);

                if (numberOfConnectedBuildingElements > 1)
                {
                    Console.WriteLine("Enter the second connected building element to this source:");
                    string globalIdConnectedBuildingElement2 = Console.ReadLine();
                    IfcBuildingElement buildingElement2 = model.Instances.FirstOrDefault<IfcBuildingElement>(d => d.GlobalId == globalIdConnectedBuildingElement2);

                    if (numberOfConnectedBuildingElements > 2)
                    {
                        Console.WriteLine("Enter the third connected building element to this source:");
                        string globalIdConnectedBuildingElement3 = Console.ReadLine();

                        IfcBuildingElement buildingElement3 = model.Instances.FirstOrDefault<IfcBuildingElement>(d => d.GlobalId == globalIdConnectedBuildingElement3);

                        relAssigns.RelatedObjects.Add(buildingElement1);
                        relAssigns.RelatedObjects.Add(buildingElement2);
                        relAssigns.RelatedObjects.Add(buildingElement3);
                        
                    }
                    else
                    {
                        relAssigns.RelatedObjects.Add(buildingElement1);
                        relAssigns.RelatedObjects.Add(buildingElement2);
                    }
                }
                else
                {
                    relAssigns.RelatedObjects.Add(buildingElement1);
                }
                txn.Commit();
                return relAssigns;
            }
        }

    }
}
