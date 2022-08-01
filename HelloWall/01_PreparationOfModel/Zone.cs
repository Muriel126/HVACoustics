using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.Ifc;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.ProductExtension;

namespace HVACoustics
{
    class Zone
    {
        public static IfcRelAssignsToGroup relGroup { get; set; }

        public static IfcZone CreateZone(IfcStore model)
        {
            using (var txn = model.BeginTransaction("Create Zone"))
            {
                Console.WriteLine("Enter name zone:");
                string nameZone = Console.ReadLine();
                IfcZone zone = model.Instances.New<IfcZone>();
                zone.Name = nameZone;

                Console.WriteLine("Add first GlobalId of room to " + nameZone);
                string GlobalIdRoom1 = Console.ReadLine();
                IfcSpace room1 = model.Instances.FirstOrDefault<IfcSpace>(d => d.GlobalId == GlobalIdRoom1);

                relGroup = model.Instances.New<IfcRelAssignsToGroup>(r =>
                {
                    r.GlobalId = Guid.NewGuid();
                    r.RelatedObjects.Add(room1);
                    r.RelatingGroup = zone;
                });

                for (int i = 0; i < 10; i++)
                {
                    Console.WriteLine("Add next GlobalId of room to " + nameZone + " if there is no further room type \"n\"");
                    string GlobalIdRoom = Console.ReadLine();
                    if (GlobalIdRoom == "n")
                    {
                        break;
                    }
                    else 
                    {
                        IfcSpace room = model.Instances.FirstOrDefault<IfcSpace>(d => d.GlobalId == GlobalIdRoom);
                        relGroup.RelatedObjects.Add(room);
                    }
                }

                txn.Commit();
                return zone;
            }
        }
        public static bool CheckSameZone(IfcStore model, string globalIdSender, string globalIdReciever)
        {
            Dictionary<string, string> dictZoneSpace = new Dictionary<string, string>();
            bool sameZone = new bool();

            IfcSpace sender = model.Instances.FirstOrDefault<IfcSpace>(d => d.GlobalId == globalIdSender);
            IfcSpace reciever = model.Instances.FirstOrDefault<IfcSpace>(d => d.GlobalId == globalIdReciever);

            var relZones = model.Instances.OfType<IIfcRelAssignsToGroup>().Where(r => r.RelatingGroup is IIfcZone).ToList();
            foreach (var zone in relZones)
            {
                foreach (var space in zone.RelatedObjects)
                {
                    dictZoneSpace.Add(space.GlobalId, zone.RelatingGroup.Name);
                }
            }

            if (dictZoneSpace[globalIdSender] == dictZoneSpace[globalIdReciever])
            {
                sameZone = true;
            }
            else
            {
                sameZone = false;
            }
            
            return sameZone;
        }
    }
}

