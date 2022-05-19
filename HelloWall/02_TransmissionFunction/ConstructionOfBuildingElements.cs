using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.Ifc;
using Xbim.Ifc4.Interfaces;

namespace HVACoustics
{
    class ConstructionOfBuildingElements
    {
        public static string GetConstruction(IfcStore model, string globalIdConnectedBuildingElement)
        {
            string constructionType = "";
            IIfcElement theWall = model.Instances.FirstOrDefault<IIfcElement>(d => d.GlobalId == globalIdConnectedBuildingElement);

            List<string> MaterialList = new List<string>();
            List<string> ThicknessList = new List<string>();

            IIfcMaterialLayerSetUsage AllLayerSetsUsage = theWall.Material as IIfcMaterialLayerSetUsage;
            Console.WriteLine("The material layer set of the building element consists of the following materials:");
            foreach (IIfcMaterialLayer materialLayer in AllLayerSetsUsage.ForLayerSet.MaterialLayers)
            {
                    
                Console.WriteLine("--------------------------------");
                Console.WriteLine("Layer: " + materialLayer.Name);
                Console.WriteLine("Material Name: " + materialLayer.Material.Name);
            }

            Console.WriteLine();
            Console.WriteLine("--------------------------------");
            Console.WriteLine("Now enter which type of building element the material layer set represents: ");
            Console.WriteLine("a: Ständerwand");
            Console.WriteLine("b: Massivholzwand");
            Console.WriteLine("c: Beton / Estrich");
            Console.WriteLine();
            string option = Console.ReadLine();

            if (option == "a")
            {
                constructionType = "Ständerwand";
                return constructionType;
            }
            if (option == "b")
            {
                constructionType = "Massivholzwand";
                return constructionType;
            }
            if (option == "c")
            {
                constructionType = "Beton";
                return constructionType;
            }
            return constructionType = "no type found";
        }
    }
}
