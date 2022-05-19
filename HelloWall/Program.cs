using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xbim.Ifc;
using Xbim.IO;
using Xbim.Ifc4.ActorResource;
using Xbim.Ifc4.DateTimeResource;
using Xbim.Ifc4.ExternalReferenceResource;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MaterialResource;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.ProductExtension;
using Xbim.Ifc4.PropertyResource;
using Xbim.Ifc4.QuantityResource;
using Xbim.Ifc4.SharedBldgElements;
using Xbim.Ifc4.SharedBldgServiceElements;
using HVACoustics;
using System.Linq;
using System.Reflection.Emit;

namespace Holzbau
{
    class Program
    {
        static int Main()
        {

            const string fileName = "2021_fhRo_RefBuild_v_02_mitTGA.ifc";
            const string fileNameNew = "gehealtes Referenzmodell.ifc";

            var editor = new XbimEditorCredentials
            {
                ApplicationDevelopersName = "Muriel Lauschke",
                ApplicationFullName = "HVACoustics",
                ApplicationIdentifier = "xbim",
                ApplicationVersion = "4.0",
                EditorsOrganisationName = "TH Rosenheim"
            };

            using (var model = IfcStore.Open(fileName, editor))
            {

                /*Console.WriteLine("First of all we create the zones in the building. How many zones do you want to create?");
                int numberOfZones = int.Parse(Console.ReadLine());
                int i = 0;
                while (i < numberOfZones)
                {
                    Zone.CreateZone(model);
                    i++;
                }


               Console.WriteLine("Now we need to create a relation between the source and the connected building elements.");
               //hier auch mehrere Quellen abfragen?
               //Raum mit Quelle auslesen ( oder optional abfragen ?)
               Source.GetRoomWithSource(model);
               string globalIdSource = Source.globalIdSource;
               string globalIdSender = Source.globalIdSenderRoom;

               //Beziehung Quelle Bauteil erstellen
               Source.CreateRelSourceToBuildingElement(model, globalIdSource);
               string globalIdConnectedBuildingElement = Source.globalIdConnectedBuildingElement1;

               //Raumkonfiguration feststellen
               Console.WriteLine("Now enter the GlobalId of the room, you want to predict.");
               string globalIdReciever = Console.ReadLine();*/

                string globalIdSender = "1U9qYuLWb7dgxXzDzBsmty";
                string globalIdReciever = "1U9qYuLWb7dgxXzDzBsmt5";

                string roomConfig = RoomConfigurations.GetRoomConfiguration(model, globalIdSender, globalIdReciever).ToString();
                Console.WriteLine("The room configuration between the sender- and reciever-room is: {0} \n", roomConfig);
                Console.ReadKey();

                string globalIdConnectedBuildingElement = "0i8nVeTTf6ox2YVT2SRF16";
                
                string typeOfBuildingElement = TypeOfBuildingElement.GetTypeOfBuildingElement(model, globalIdConnectedBuildingElement);
                Console.WriteLine("The type of the building element is: {0} \n", typeOfBuildingElement);
                Console.ReadKey();

                Console.WriteLine("In the next step we will get the construction of building element to which the source is connected.\n");
                Enums.TypeBuildingConstruction constructionType = ConstructionOfBuildingElements.GetConstruction(model, globalIdConnectedBuildingElement);
                Console.WriteLine("The construction type is: {0}", constructionType);

                model.SaveAs(fileNameNew);
                return 0;
            }
        }
    }
}
