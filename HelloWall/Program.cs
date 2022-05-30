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
        public static string globalIdSource {get; set;}
        public static string globalIdSender { get; set; }
        public static string globalIdReciever { get; set; }
        public static string globalIdConnectedBuildingElement { get; set; }

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

           /* using (var model = IfcStore.Open(fileName, editor))
            {

                Console.WriteLine("First of all we create the zones in the building. How many zones do you want to create?");
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
                globalIdSource = Source.globalIdSource;
                globalIdSender = Source.globalIdSenderRoom;

                //Beziehung Quelle Bauteil erstellen
                Source.CreateRelSourceToBuildingElement(model, globalIdSource);
                globalIdConnectedBuildingElement = Source.globalIdConnectedBuildingElement1;

                //Raumkonfiguration feststellen
                Console.WriteLine("Now enter the GlobalId of the room, you want to predict.");
                globalIdReciever = Console.ReadLine();

                model.SaveAs(fileNameNew);
            }*/
            using (var model2 = IfcStore.Open(fileNameNew, editor))
            {
                //string globalIdSender = "188EOkmYj41AhoJC1EOfEE";
                //string globalIdReciever = "1U9qYuLWb7dgxXzDzBsmtr";

               // var roomConfig = RoomConfigurations.GetRoomConfiguration(model2, globalIdSender, globalIdReciever);
               // Console.WriteLine("The room configuration between the sender- and reciever-room is: {0} \n", roomConfig.ToString());
                //Console.ReadKey();
                string globalIdConnectedBuildingElement = "0i8nVeTTf6ox2YVT2SRF1u";

               // Enums.TypeBuildingElement typeOfBuildingElement = TypeOfBuildingElement.GetTypeOfBuildingElement(model2, globalIdConnectedBuildingElement, globalIdReciever, roomConfig);
               // Console.WriteLine("The type of the building element is: {0} ", typeOfBuildingElement);
               // Console.ReadKey();

                Console.WriteLine("In the next step we will get the construction of building element to which the source is connected.\n");
                Enums.TypeBuildingConstruction constructionType = ConstructionOfBuildingElements.GetConstruction(model2, globalIdConnectedBuildingElement);
                Console.WriteLine("The construction type is: {0}", constructionType);


                return 0;
            }
        }
    }
}
