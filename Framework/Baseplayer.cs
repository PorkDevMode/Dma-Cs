using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework
{
    public class BasePlayer
    {

        public ulong Class { get; private set; }
        public uint ActiveItemID { get; private set; }
        public ulong PlayerInventory { get; private set; }
        public ulong PlayerModel { get; private set; }
        public ulong DisplayName { get; private set; }

    }
}
