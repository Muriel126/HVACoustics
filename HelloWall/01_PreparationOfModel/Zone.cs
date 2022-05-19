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

                Console.WriteLine("Add second GlobalId of room to " + nameZone + " if there is no further room type \"n\"");
                string GlobalIdRoom2 = Console.ReadLine();
                IfcSpace room2 = model.Instances.FirstOrDefault<IfcSpace>(d => d.GlobalId == GlobalIdRoom2);
                if (GlobalIdRoom2 != "n")
                {
                    Console.WriteLine("Add third GlobalId of room to " + nameZone + " if there is no further room type \"n\"");
                    string GlobalIdRoom3 = Console.ReadLine();
                    IfcSpace room3 = model.Instances.FirstOrDefault<IfcSpace>(d => d.GlobalId == GlobalIdRoom3);
                    if (GlobalIdRoom3 != "n")
                    {
                        Console.WriteLine("Add forth GlobalId of room to " + nameZone + " if there is no further room type \"n\"");
                        string GlobalIdRoom4 = Console.ReadLine();
                        IfcSpace room4 = model.Instances.FirstOrDefault<IfcSpace>(d => d.GlobalId == GlobalIdRoom4);

                        if (GlobalIdRoom4 != "n")
                        {
                            Console.WriteLine("Add fifth GlobalId of room to " + nameZone + " if there is no further room type \"n\"");
                            string GlobalIdRoom5 = Console.ReadLine();
                            IfcSpace room5 = model.Instances.FirstOrDefault<IfcSpace>(d => d.GlobalId == GlobalIdRoom5);

                            if (GlobalIdRoom5 != "n")
                            {
                                Console.WriteLine("Add sixth GlobalId of room to " + nameZone + " if there is no further room type \"n\"");
                                string GlobalIdRoom6 = Console.ReadLine();
                                IfcSpace room6 = model.Instances.FirstOrDefault<IfcSpace>(d => d.GlobalId == GlobalIdRoom6);

                                var relGroup = model.Instances.New<IfcRelAssignsToGroup>(r =>
                                {
                                    r.GlobalId = Guid.NewGuid();
                                    r.RelatedObjects.Add(room1);
                                    r.RelatedObjects.Add(room2);
                                    r.RelatedObjects.Add(room3);
                                    r.RelatedObjects.Add(room4);
                                    r.RelatedObjects.Add(room5);
                                    r.RelatedObjects.Add(room6);
                                    r.RelatingGroup = zone;
                                });
                            }
                            else
                            {
                                var relGroup = model.Instances.New<IfcRelAssignsToGroup>(r =>
                                {
                                    r.GlobalId = Guid.NewGuid();
                                    r.RelatedObjects.Add(room1);
                                    r.RelatedObjects.Add(room2);
                                    r.RelatedObjects.Add(room3);
                                    r.RelatedObjects.Add(room4);
                                    r.RelatedObjects.Add(room5);
                                    r.RelatingGroup = zone;
                                });
                            }
                        }
                        else
                        {
                            var relGroup = model.Instances.New<IfcRelAssignsToGroup>(r =>
                            {
                                r.GlobalId = Guid.NewGuid();
                                r.RelatedObjects.Add(room1);
                                r.RelatedObjects.Add(room2);
                                r.RelatedObjects.Add(room3);
                                r.RelatedObjects.Add(room4);
                                r.RelatingGroup = zone;
                            });
                        }
                    }
                    else
                    {
                        var relGroup = model.Instances.New<IfcRelAssignsToGroup>(r =>
                        {
                            r.GlobalId = Guid.NewGuid();
                            r.RelatedObjects.Add(room1);
                            r.RelatedObjects.Add(room2);
                            r.RelatedObjects.Add(room3);
                            r.RelatingGroup = zone;
                        });
                    }
                }
                else 
                {
                    var relGroup = model.Instances.New<IfcRelAssignsToGroup>(r =>
                    {
                        r.GlobalId = Guid.NewGuid();
                        r.RelatedObjects.Add(room1);
                        r.RelatedObjects.Add(room2);
                        r.RelatingGroup = zone;
                    });
                }

                txn.Commit();
                return zone;
            }
        }
    }
}
