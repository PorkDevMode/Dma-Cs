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

        // Todsky addresses
        public static ulong todNight;
        public static ulong todDay;
        #endregion

        #region Initialize Methods
        // Initializes memory addresses
        public static void initMethods()
        {
            // Static admin
            Print("[ADMIN] Initializing");
            ulong adminBase = Memory.Read<ulong>(Offsets.GameAssembly + Offsets.adminBase);
            ulong adminStatic = Memory.Read<ulong>(adminBase + Offsets.Static);

            // Admin address definitions
            timeAddress = adminStatic + Offsets.adminTime;
            waterEffect = adminStatic + Offsets.adminWaterEffect;
            Print("[ADMIN] Initialized");

            // Static convar graphics
            Print("[GRAPHICS] Initializing");
            ulong graphicsBase = Memory.Read<ulong>(Offsets.GameAssembly + Offsets.graphicsBase);
            ulong graphicsStatic = Memory.Read<ulong>(graphicsBase + Offsets.Static);

            // Convar graphics definitions.
            fovAddress = graphicsStatic + Offsets.graphicsFov;
            Print("[GRAPHICS] Initialized");

            // Static localplayer base
            Print("[LOCAL PLAYER] Initializing");
            ulong localPlayerBase = Memory.Read<ulong>(Offsets.GameAssembly + Offsets.localPlayerBase);
            ulong localPlayerStatic = Memory.Read<ulong>(localPlayerBase + Offsets.Static);

            // Static baseplayer
            ulong basePlayerBase = Memory.Read<ulong>(localPlayerStatic + Offsets.basePlayerBackingField);
            ulong basePlayerMovementAdd = Memory.Read<ulong>(basePlayerBase + Offsets.basePlayerMovement);

            // Baseplayer address definitions
            basePlayer = basePlayerBase;
            basePlayerMovement = basePlayerMovementAdd;
            Print("[LOCAL PLAYER] Initialized");

            // Static Todsky definitions
            Print("[TODSKY] Initializing");
            ulong todSkyBase = Memory.Read<ulong>(Offsets.GameAssembly + Offsets.todSkyBase);
            ulong todSkyStatic = Memory.Read<ulong>(todSkyBase + Offsets.Static);
            todSkyStatic = Memory.Read<ulong>(todSkyStatic + 0x0);
            ulong instanceValues = Memory.Read<ulong>(todSkyStatic + 0x10);
            ulong instance = Memory.Read<ulong>(instanceValues + 0x20);

            // TODSky address definitions
            todNight = Memory.Read<ulong>(instance + Offsets.NightParameters);
            todDay = Memory.Read<ulong>(instance + Offsets.DayParameters);
            Print("[LOCAL PLAYER] Initialized");
        }
        #endregion

        #region Base Methods
        // print method because im lazy
        public static void Print(string message)
        {
            Console.WriteLine(message);
        }
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
        #endregion
        #region Methods
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
                VmmScatter scatter = Memory.createScatter();

                Memory.PrepareWrite(scatter, basePlayerMovement + Offsets.groundAngleNew, 0f);
                Memory.PrepareWrite(scatter, basePlayerMovement + Offsets.maxAngle, 100f);
                Memory.PrepareWrite(scatter, basePlayerMovement + Offsets.groundAngle, 0f);
                scatter.Execute();
                scatter.Close();
                Thread.Sleep(1);
            }
        }
        // Jump very high!
        public static void infiniteJump()
        {
            while (true)
            {
                // WILL RESEARCH THIS MORE, DK HOW TO DO

                // VmmScatter scatter = Memory.createScatter();
                // 
                // Memory.PrepareWrite(scatter, basePlayerMovement + Offsets.landTime, 0f);
                // Memory.PrepareWrite(scatter, basePlayerMovement + Offsets.jumpTime, 0f);
                // Memory.PrepareWrite(scatter, basePlayerMovement + Offsets.groundTime, 0f);
                // scatter.Execute();
                // scatter.Close();
                // Thread.Sleep(100);
                // float e = Memory.Read<float>(basePlayerMovement + Offsets.landTime);
                // float d = Memory.Read<float>(basePlayerMovement + Offsets.jumpTime);
                // float b = Memory.Read<float>(basePlayerMovement + Offsets.groundTime);
                // Print($"Landtime: {e}\nJumptime: {d}\nGroundtime: {b}");
                // Thread.Sleep(100);
            }
        }
        #endregion
    }
}
