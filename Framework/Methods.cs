using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Framework;

namespace Framework
{
    internal class Methods
    {
        #region Addresses
        // Admin addresses
        public static ulong timeAddress;
        public static ulong waterEffect;

        // Convar graphics addresses
        public static ulong fovAddress;

        // Localplayer addresses

        // Baseplayer addresses
        public static ulong basePlayer;
        public static ulong basePlayerMovement;
        #endregion

        #region Initialize Methods
        // Initializes memory addresses
        public static void initMethods()
        {
            // Static admin
            ulong adminBase = Memory.Read<ulong>(Offsets.GameAssembly + Offsets.adminBase);
            ulong adminStatic = Memory.Read<ulong>(adminBase + Offsets.Static);
            timeAddress = adminStatic + Offsets.adminTime;
            waterEffect = adminStatic + Offsets.adminWaterEffect;

            // Static convar graphics
            ulong graphicsBase = Memory.Read<ulong>(Offsets.GameAssembly + Offsets.graphicsBase);
            ulong graphicsStatic = Memory.Read<ulong>(graphicsBase + Offsets.Static);
            fovAddress = graphicsStatic + Offsets.graphicsFov;

            // Static localplayer base
            ulong localPlayerBase = Memory.Read<ulong>(Offsets.GameAssembly + Offsets.localPlayerBase);
            ulong localPlayerStatic = Memory.Read<ulong>(localPlayerBase + Offsets.Static);

            // Static baseplayer
            ulong basePlayerBase = Memory.Read<ulong>(localPlayerStatic + Offsets.basePlayerBackingField);
            ulong basePlayerMovementAdd = Memory.Read<ulong>(basePlayerBase + Offsets.basePlayerMovement);

            // Baseplayer address definitions
            basePlayer = basePlayerBase;
            basePlayerMovement = basePlayerMovementAdd;
        }
        #endregion

        #region Base Methods
        // Changes convar graphics fov
        public static void changeFov(float fov)
        {
            if (!Memory.Write(fovAddress, fov))
            {
                Console.WriteLine("Failed to write fov");
            }
            else
            {
                Console.WriteLine("Successfully wrote fov value");
            }
        }

        // Changes admin time
        public static void changeTime(float time)
        {
            if (!Memory.Write(timeAddress, time))
            {
                Console.WriteLine("Failed to write admin time");
            }
            else
            {
                Console.WriteLine("Successfully wrote time value");
            }
        }
        // Permaday
        public static void infiniteDay()
        {
            while (true)
            {
                float time = Memory.Read<float>(timeAddress);

                if (time != -1f)
                {
                    changeTime(-1f);
                }
                Thread.Sleep(60000);
            }
        }
        // Climb like a mexican
        public static void spiderMan()
        {
            while (true)
            {
                Vmm vmm = Offsets.vmm;

                VmmScatter scatter = vmm.Scatter_Initialize(Offsets.processPid, Vmm.FLAG_NOCACHE);

                Memory.PrepareWrite(scatter, basePlayerMovement + 0xD0, 0f);
                Memory.PrepareWrite(scatter, basePlayerMovement + 0x94, 100f);
                Memory.PrepareWrite(scatter, basePlayerMovement + 0xCC, 0f);
                scatter.Execute();
                scatter.Close();
            }
        }
        #endregion
    }
}
