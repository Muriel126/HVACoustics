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
    }
}
