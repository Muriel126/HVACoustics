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
        public static string globalIdConnectedBuildingElement1 { get; set; }
        public static string globalIdConnectedBuildingElement2 { get; set; }
        public static string globalIdConnectedBuildingElement3 { get; set; }
        public static Enums.TypeBuildingElement typeOfBuildingElement1 { get; set; }
        public static Enums.TypeBuildingElement typeOfBuildingElement2 { get; set; }
        public static Enums.TypeBuildingElement typeOfBuildingElement3 { get; set; }
        public static Enums.TypeBuildingConstruction constructionType1 { get; set; }
        public static Enums.TypeBuildingConstruction constructionType2 { get; set; }
        public static Enums.TypeBuildingConstruction constructionType3 { get; set; }
        public static int numberOfConnectedBuildingElements { get; set; }

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

            /*using (var model = IfcStore.Open(fileName, editor))
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
                Console.WriteLine("How many transmission paths (connected building elements) do you want to consider?");
                numberOfConnectedBuildingElements = int.Parse(Console.ReadLine());

                if (numberOfConnectedBuildingElements == 1)
                {
                    Source.CreateRelSourceToBuildingElement(model, globalIdSource, 1);
                    globalIdConnectedBuildingElement1 = Source.globalIdConnectedBuildingElement1;
                }
                if (numberOfConnectedBuildingElements ==2)
                {
                    Source.CreateRelSourceToBuildingElement(model, globalIdSource, 2);
                    globalIdConnectedBuildingElement1 = Source.globalIdConnectedBuildingElement1;
                    globalIdConnectedBuildingElement2 = Source.globalIdConnectedBuildingElement2;
                }
                if (numberOfConnectedBuildingElements == 3)
                {
                    Source.CreateRelSourceToBuildingElement(model, globalIdSource, 3);
                    globalIdConnectedBuildingElement1 = Source.globalIdConnectedBuildingElement1;
                    globalIdConnectedBuildingElement2 = Source.globalIdConnectedBuildingElement2;
                    globalIdConnectedBuildingElement3 = Source.globalIdConnectedBuildingElement3;
                }

                //Raumkonfiguration feststellen
                Console.WriteLine("Now enter the GlobalId of the room, you want to predict.");
                globalIdReciever = Console.ReadLine();

                model.SaveAs(fileNameNew);
            }*/
            using (var model2 = IfcStore.Open(fileNameNew, editor))
            {
                string globalIdSender = "188EOkmYj41AhoJC1EOfEz";
                string globalIdReciever = "188EOkmYj41AhoJC1EOfEA";
                numberOfConnectedBuildingElements = 2;

                var roomConfig = RoomConfigurations.GetRoomConfiguration(model2, globalIdSender, globalIdReciever);
                Console.WriteLine("The room configuration between the sender- and reciever-room is: {0} \n", roomConfig.ToString());
                Console.ReadKey();
                globalIdConnectedBuildingElement1 = "0i8nVeTTf6ox2YVT2SRF1_";
                globalIdConnectedBuildingElement2 = "0i8nVeTTf6ox2YVT2SRF1w";
                if (numberOfConnectedBuildingElements >= 1)
                {
                    typeOfBuildingElement1 = TypeOfBuildingElement.GetTypeOfBuildingElement(model2, globalIdConnectedBuildingElement1, globalIdSender, globalIdReciever, roomConfig);
                    Console.WriteLine("The type of building element is: {0}\n", typeOfBuildingElement1);
                    Console.ReadKey();

                    Console.WriteLine("In the next step we will get the construction of building element to which the source is connected.\n");
                    constructionType1 = ConstructionOfBuildingElements.GetConstruction(model2, globalIdConnectedBuildingElement1);
                    Console.WriteLine("The construction type is: {0}", constructionType1);
                    if (numberOfConnectedBuildingElements == 1)
                    {
                        Console.WriteLine("\nThe resulting data is:\n" +
                                        "Room Configuration: {0}/n" +
                                        "First type of building element: {1}\n" +
                                        "First construction type: {2}\n" , roomConfig, typeOfBuildingElement1, constructionType1);
                    }
                }
                if (numberOfConnectedBuildingElements >= 2)
                {
                    Console.WriteLine("\nNow we're going on with the second connected building element");
                    typeOfBuildingElement2 = TypeOfBuildingElement.GetTypeOfBuildingElement(model2, globalIdConnectedBuildingElement2, globalIdSender, globalIdReciever, roomConfig);
                    Console.WriteLine("The second type of building element is: {0}\n", typeOfBuildingElement2);
                    Console.ReadKey();

                    Console.WriteLine("In the next step we will get the construction of building element to which the source is connected.\n");
                    constructionType2 = ConstructionOfBuildingElements.GetConstruction(model2, globalIdConnectedBuildingElement2);
                    Console.WriteLine("The second construction type is: {0}", constructionType2);

                    if (numberOfConnectedBuildingElements == 2)
                    {
                        Console.WriteLine("\nThe resulting data is:\n" +
                                        "Room Configuration: {0}/n" +
                                        "First type of building element: {1}\n" +
                                        "First construction type: {2}\n" +
                                        "Second type of building element: {3}\n" +
                                        "Second construction type: {4}\n", roomConfig, typeOfBuildingElement1, constructionType1, typeOfBuildingElement2, constructionType2);
                    }
                }
                if (numberOfConnectedBuildingElements >= 3)
                {
                    typeOfBuildingElement3 = TypeOfBuildingElement.GetTypeOfBuildingElement(model2, globalIdConnectedBuildingElement3, globalIdSender, globalIdReciever, roomConfig);
                    Console.WriteLine("The second type of building element is: {0}\n", typeOfBuildingElement3);
                    Console.ReadKey();

                    Console.WriteLine("In the next step we will get the construction of building element to which the source is connected.\n");
                    constructionType3 = ConstructionOfBuildingElements.GetConstruction(model2, globalIdConnectedBuildingElement3);
                    Console.WriteLine("The second construction type is: {0}", constructionType3);

                    if (numberOfConnectedBuildingElements == 3)
                    {
                        Console.WriteLine("\nThe resulting data is:\n" +
                                        "Room Configuration: {0}/n" +
                                        "First type of building element: {1}\n" +
                                        "First construction type: {2}\n" +
                                        "Second type of building element: {3}\n" +
                                        "Second construction type: {4}\n" +
                                        "Third type of building element: {5}\n" +
                                        "Third construction type: {6}", roomConfig, typeOfBuildingElement1, constructionType1, typeOfBuildingElement2, constructionType2, typeOfBuildingElement3, constructionType3);
                    }
                }

                return 0;
            }
        }
    }
}
