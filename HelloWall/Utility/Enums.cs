using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVACoustics
{
    class Enums
    {
        public enum TypeRoomConfig
        {
            hor,
            horPosOff,
            horNegOff,
            horOff,
            ver,
            verPosOff,
            verNegOff,
            verOff,
            ver1Room,
            ver2Room,
            hor1Room,
            hor2Room
        }
        public enum TypeDirec
        {
            Xpos,
            Xneg,
            Ypos,
            Yneg,
            sameCor
        }
        public enum TypeBuildingElement
        {
            InteriorWall,
            PartyWall,
            HousePartyWall,
            ExteriorWall,
            Floor,
            PlumbingWall,
            BasementWallInterior,
            BasementWallExterior,
            Default
        }
        public enum TypeSlabConstruction
        {
            Concrete,
            SolidTimber,
            TimberJoistFloor,   //Holzbalkendecke
            TimberRibbOrBoxModules,  //Holztippen- oder Holzkastendecke
            TimberConcreteComposite   //Holzbetonverbundwand
        }
        public enum TypeBuildingConstruction
        {
            Concrete,
            Masonary,    //Mauerwerk
            PlumbingWall,   //Installationswand
            SolidTimber,   //Massivholz
            SteelFrame,   //einfach Ständerwerk Metall
            SteelFrameDouble,   //getrenntes Ständerwerk Metall
            TimberFrameSingle,   //einfach Ständerwerk
            TimberFrameDouble,   //getrenntes Ständerwerk
            TimberFrameSingleWithCrossBattens,   //einfach Ständerwerk mit Konterlattung
            TimberJoistFloor,   //Holzbalkendecke
            TimberConcreteComposite,   //Holzbetonverbund
            Default
        }
        public enum TypeOfPoint
        {
            start,
            end
        }
    }
}
