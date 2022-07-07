using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.Ifc;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.SharedBldgElements;

namespace HVACoustics
{
    class ConstructionOfBuildingElement
    {
        public static IIfcMaterialLayerSet materialLayerSet { get; set; }

        public static Enums.TypeBuildingConstruction GetConstruction(IfcStore model, string globalIdConnectedBuildingElement, Enums.TypeRoomConfig roomConfig, string globalIdSender, 
            string globalIdReciever)
        {
            var sem = new SemanticHandler.SemanticHandler();

            var type = sem.GetTypeOfBuildingElement(model, globalIdConnectedBuildingElement);
            Console.WriteLine("The predefinied type of the building element is: {0}\n", type);
            if (roomConfig == Enums.TypeRoomConfig.ver || roomConfig == Enums.TypeRoomConfig.verNegOff || roomConfig == Enums.TypeRoomConfig.verPosOff || roomConfig == Enums.TypeRoomConfig.verOff) // hier auch ver1Room und ver2Room?
            {
                var globalIdConnectingSlab = sem.GetGlobalIdOfConnectingBuildingElementOfTwoSpaces(model, globalIdSender, globalIdReciever);
                materialLayerSet = sem.GetMaterialLayerSet(model, globalIdConnectingSlab);
            }
            else
            {
                materialLayerSet = sem.GetMaterialLayerSet(model, globalIdConnectedBuildingElement);
            }

            Console.WriteLine("The material layer set of the building element consists of the following materials:");
            foreach (IIfcMaterialLayer materialLayer in materialLayerSet.MaterialLayers)
            {
                Console.WriteLine("--------------------------------");
                Console.WriteLine("Layer: " + materialLayer.Name);
                Console.WriteLine("Material Name: " + materialLayer.Material.Name);
            }
            Console.WriteLine("--------------------------------");
            Console.WriteLine("Now enter which type of building element the material layer set represents: ");
            Console.WriteLine("a: Massivholzwand\n" +
                              "b: einfach Ständerwerk\n" +
                              "c: getrenntes Ständerwerk\n" +
                              "d: einfaches Ständerwerk mit Konterlattung\n" +
                              "e: einfach Ständerwerk Metall\n" +
                              "f: getrenntes Ständerwerk Metall\n" +
                              "g: Installationswand\n" +
                              "h: Holzbalkendecke\n" +
                              "i: Holzbetonverbund\n");
            string option = Console.ReadLine();
            if (option == "a")
            {
                return Enums.TypeBuildingConstruction.SolidTimber;      
            }
            if (option == "b")
            {
                return Enums.TypeBuildingConstruction.TimberFrameSingle;
            }
            if (option == "c")
            {
                return Enums.TypeBuildingConstruction.TimberFrameDouble;
            }
            if (option == "d")
            {
                return Enums.TypeBuildingConstruction.TimberFrameSingleWithCrossBattens;
            }
            if (option == "e")
            {
                return Enums.TypeBuildingConstruction.SteelFrame;
            }
            if (option == "f")
            {
                return Enums.TypeBuildingConstruction.SteelFrameDouble;
            }
            if (option == "g")
            {
                return Enums.TypeBuildingConstruction.PlumbingWall;
            }
            if (option == "h")
            {
                return Enums.TypeBuildingConstruction.TimberJoistFloor;
            }
            if (option == "i")
            {
                return Enums.TypeBuildingConstruction.TimberConcreteComposite;
            }
            return Enums.TypeBuildingConstruction.Default;
        }
    }
}
