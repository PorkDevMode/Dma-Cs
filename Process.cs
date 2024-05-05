using Framework;
using ImGuiNET;
using ClickableTransparentOverlay;
using Vortice.Direct3D11;
using System.Numerics;
using System.Threading.Tasks;
using System.Threading;
using System;

class Process : Overlay
{
    private static int Fov = 90;
    private static bool enableSpider = false;
    private static bool enableEsp = false;
    private static bool Initialized = false;
    public static Vector2 screenSize = new Vector2(1920, 1080);
    public static Vector2 drawPosition = new Vector2(150, 150);
    public static uint playerListSize;
    public static List<Player> playerList = new List<Player>();
    public static ViewMatrix CurrentMatrix;
    public static Vector2 ScreenSize = new Vector2(1920, 1080);
    #region Addresses
    // Admin addresses
    public static UInt64 timeAddress;
    public static UInt64 waterEffect;

    // Convar graphics addresses
    public static UInt64 fovAddress;

    // Localplayer addresses

    // Baseplayer addresses
    public static UInt64 basePlayer;
    public static UInt64 basePlayerMovement;
    public static UInt64 VisiblePlayerList;

    // Todsky addresses
    public static UInt64 todNight;
    public static UInt64 todDay;

    // Camera
    public static UInt64 playerCamera;
    #endregion
    protected override void Render()
    {
        DrawMenu();
        // DrawOverlay();
        
    }

    void DrawMenu()
    {
        ImGui.Begin("PorkHack");
        ImGui.Checkbox("Spiderman", ref enableSpider);
        ImGui.Checkbox("Esp", ref enableEsp);
        ImGui.SliderInt("Fov", ref Fov, 50, 100);
        ImGui.End();
    }

    void DrawOverlay()
    {
        Console.WriteLine("E");
        ImGui.SetNextWindowSize(screenSize);
        ImGui.SetNextWindowPos(new Vector2(0, 0));
        ImGui.Begin("Overlay",
            ImGuiWindowFlags.NoDecoration |
            ImGuiWindowFlags.NoBackground |
            ImGuiWindowFlags.NoBringToFrontOnFocus |
            ImGuiWindowFlags.NoMove |
            ImGuiWindowFlags.NoInputs |
            ImGuiWindowFlags.NoCollapse |
            ImGuiWindowFlags.NoScrollWithMouse
        );
    }

    static void Main(string[] args)
    {
        Process process = new Process();
        process.Start().Wait();

        Console.WriteLine("Initializing components.");

        Memory.init("RustClient.exe", "GameAssembly.dll");

        initMethods();

        Task cacheTask = UpdatePlayers();
        Task matrixTask = UpdateMatrixAsync();
        Spider();
        // Task espTask = Esp();
        Initialized = true;
        Console.WriteLine("Initialized!");
    }

    #region Initialize Methods
    // Initializes memory addresses
    public static void initMethods()
    {
        // Static admin
        Print("[ADMIN] Initializing");
        UInt64 adminBase = Memory.Read<UInt64>(Offsets.GameAssembly + Offsets.adminBase);
        UInt64 adminStatic = Memory.Read<UInt64>(adminBase + Offsets.Static);

        // Admin address definitions
        timeAddress = adminStatic + Offsets.adminTime;
        waterEffect = adminStatic + Offsets.adminWaterEffect;
        Print("[ADMIN] Initialized");

        // Static convar graphics
        Print("[GRAPHICS] Initializing");
        UInt64 graphicsBase = Memory.Read<UInt64>(Offsets.GameAssembly + Offsets.graphicsBase);
        UInt64 graphicsStatic = Memory.Read<UInt64>(graphicsBase + Offsets.Static);

        // Convar graphics definitions.
        fovAddress = graphicsStatic + Offsets.graphicsFov;
        Print("[GRAPHICS] Initialized");

        // Static localplayer base
        Print("[LOCAL PLAYER] Initializing");
        UInt64 localPlayerBase = Memory.Read<UInt64>(Offsets.GameAssembly + Offsets.localPlayerBase);
        UInt64 localPlayerStatic = Memory.Read<UInt64>(localPlayerBase + Offsets.Static);
        Print("[LOCAL PLAYER] Initialized");

        // Static baseplayer
        Print("[BASE PLAYER] Initializing");
        UInt64 basePlayerBase = Memory.Read<UInt64>(localPlayerStatic + Offsets.basePlayerBackingField);
        UInt64 basePlayerMovementAdd = Memory.Read<UInt64>(basePlayerBase + Offsets.basePlayerMovement);

        // Baseplayer address definitions
        basePlayer = basePlayerBase;
        basePlayerMovement = basePlayerMovementAdd;
        Print("[BASE PLAYER] Initialized");

        // Static Todsky definitions
        Print("[TODSKY] Initializing");
        UInt64 todSkyBase = Memory.Read<UInt64>(Offsets.GameAssembly + Offsets.todSkyBase);
        UInt64 todSkyStatic = Memory.Read<UInt64>(todSkyBase + Offsets.Static);
        todSkyStatic = Memory.Read<UInt64>(todSkyStatic + 0x0);
        UInt64 instanceValues = Memory.Read<UInt64>(todSkyStatic + 0x10);
        UInt64 instance = Memory.Read<UInt64>(instanceValues + 0x20);

        // TODSky address definitions
        todNight = Memory.Read<UInt64>(instance + Offsets.NightParameters);
        todDay = Memory.Read<UInt64>(instance + Offsets.DayParameters);
        Print("[TODSKY] Initialized");

        // playerlist
        UInt64 playerList = Memory.Read<UInt64>(Offsets.GameAssembly + Offsets.playerListBase);
        UInt64 playerListStatic = Memory.Read<UInt64>(playerList + 0xB8);
        UInt64 playerList1 = Memory.Read<UInt64>(playerListStatic + 0x20);
        UInt64 playerList2 = Memory.Read<UInt64>(playerList1 + 0x28);

        VisiblePlayerList = playerList2;

        // Main camera
        UInt64 playerCameraBase = Memory.Read<UInt64>(Offsets.GameAssembly + 0x3B39C98);
        UInt64 playerCameraStatic = Memory.Read<UInt64>(playerCameraBase + 0xB8);
        UInt64 playerCameraCurrent = Memory.Read<UInt64>(playerCameraStatic + 0x0);
        playerCamera = Memory.Read<UInt64>(playerCameraCurrent + 0x10);
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
    public static void Spider()
    {
        while (true)
        {
            VmmScatter scatter = Memory.createScatter();

            Memory.PrepareWrite(scatter, basePlayerMovement + Offsets.groundAngleNew, 0f);
            Memory.PrepareWrite(scatter, basePlayerMovement + Offsets.maxAngle, 100f);
            Memory.PrepareWrite(scatter, basePlayerMovement + Offsets.groundAngle, 0f);
            scatter.Execute();
            scatter.Close();
        }
    }
    // Caches the player list
    public static void cachePlayers()
    {
        if (VisiblePlayerList == 0)
        {
            playerListSize = 0;
            playerList.Clear();
            return;
        }

        uint size = Memory.Read<uint>(VisiblePlayerList + 0x10);
        UInt64 buffer = Memory.Read<UInt64>(VisiblePlayerList + 0x18);
        playerListSize = size;
        List<Player> TempPlayerList = new List<Player>();
        TempPlayerList.Capacity = (int)size;

        for (int i = 0; i < size; i++)
        {
            Player player = new Player();
            long EntityOffset = 0x20 + (i * 8);
            UInt64 PlayerMem = Memory.Read<UInt64>(buffer + (ulong)EntityOffset);
            player.BaseAddress = PlayerMem;
            player.Position = GetPlayerVector3(PlayerMem);
            // Vector2 cord = WorldToScreen(player.Position);
            // Console.WriteLine($"Player is at X / Y: X {cord.X} Y {cord.Y}"); WORLD2SCREEN TEST
            TempPlayerList.Add(player);
        }
        playerList = TempPlayerList;
    }
    // Gets your players viewmatrix
    public static ViewMatrix GetPlayerViewMatrix()
    {
        ViewMatrix model;
        unsafe
        {
            Offsets.vmm.MemReadStruct<ViewMatrix>(Offsets.processPid, playerCamera + 0x30C, out model);
            return model;
        }
    }
    // Constantly updates the viewmatrix
    public static async Task UpdateMatrixAsync()
    {
        while (true)
        {
            CurrentMatrix = GetPlayerViewMatrix();
            await Task.Delay(7);
        }
    }
    //Constantly caches players
    public static async Task UpdatePlayers()
    {
        while (true)
        {
            cachePlayers();
            await Task.Delay(2);
        }
    }
    public static async Task Esp()
    {
        Vector2 boxSize = new Vector2(50, 50);
        while (true)
        {
            if (enableEsp == true)
            {
                if (Initialized == true)
                {
                    for (int i = 0; i < playerList.Count; i++)
                    {
                        if (playerList[i].Position != new Vector3(0, 0, 0))
                        {
                            Vector2 drawPos = WorldToScreen(playerList[i].Position);
                            if (drawPos != new Vector2(0, 0))
                            {
                                Console.WriteLine("Drawing");
                                // drawList.AddRect(drawPos, boxSize, ImGui.ColorConvertFloat4ToU32(new Vector4(1, 0, 0, 1)));
                            }
                        }
                    }
                    await Task.Delay(7);
                }
            }
        }
    }

    // Gets a baseplayers position
    public static Vector3 GetPlayerVector3(UInt64 address)
    {
        UInt64 model = Memory.Read<UInt64>(address + Offsets.basePlayerModel);

        Vector3 playerPos = Memory.Read<Vector3>(model + Offsets.basePlayerModelPosition);

        return playerPos;
    }
    #endregion
    #region structs
    // public unsafe struct ViewMatrix
    // {
    //     private fixed float _matrix[4 * 4];
    // 
    //     public ViewMatrix(float[,] matrix)
    //     {
    //         if (matrix == null || matrix.GetLength(0) != 4 || matrix.GetLength(1) != 4)
    //         {
    //             throw new ArgumentException("Invalid matrix");
    //         }
    // 
    //         fixed (float* ptr = _matrix)
    //         {
    //             for (int i = 0; i < 4; i++)
    //             {
    //                 for (int j = 0; j < 4; j++)
    //                 {
    //                     ptr[i * 4 + j] = matrix[i, j];
    //                 }
    //             }
    //         }
    //     }
    //     public float this[int row, int column]
    //     {
    //         get
    //         {
    //             fixed (float* ptr = _matrix)
    //             {
    //                 return ptr[row * 4 + column];
    //             }
    //         }
    //     }
    // }
    public unsafe struct ViewMatrix
    {
        private fixed float matrix[4 * 4];

        public ViewMatrix(float[,] matrix)
        {
            if (matrix.GetLength(0) != 4 || matrix.GetLength(1) != 4)
                throw new ArgumentException("Matrix must be 4x4.");

            fixed (float* ptr = this.matrix)
            {
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        ptr[i * 4 + j] = matrix[i, j];
                    }
                }
            }
        }

        public float this[int row, int col]
        {
            get
            {
                if (row < 0 || row >= 4 || col < 0 || col >= 4)
                    throw new IndexOutOfRangeException("Index out of range.");

                fixed (float* ptr = matrix)
                {
                    return ptr[row * 4 + col];
                }
            }
            set
            {
                if (row < 0 || row >= 4 || col < 0 || col >= 4)
                    throw new IndexOutOfRangeException("Index out of range.");

                fixed (float* ptr = matrix)
                {
                    ptr[row * 4 + col] = value;
                }
            }
        }
    }

    public struct Player
    {
        public UInt64 BaseAddress;
        public String DisplayName;
        public Vector3 Position;
    }
    #endregion
    #region Visuals
    public static Vector2 WorldToScreen(Vector3 pos)
    {
        ViewMatrix vm;
        vm = CurrentMatrix;

        float w = vm[0, 3] * pos.X + vm[1, 3] * pos.Y + vm[2, 3] * pos.Z + vm[3, 3];

        if (w < 0.001f)
        {
            return new Vector2(0, 0);
        }

        float x = pos.X * vm[0, 0] + pos.Y * vm[1, 0] + pos.Z * vm[2, 0] + vm[3, 0];
        float y = pos.X * vm[0, 1] + pos.Y * vm[1, 1] + pos.Z * vm[2, 1] + vm[3, 1];

        float nx = x / w;
        float ny = y / w;

        Vector2 Result;

        Result.X = (ScreenSize.X * 0.5f * nx) + (nx + ScreenSize.X * 0.5f);
        Result.Y = (ScreenSize.Y * 0.5f * ny) + (ny + ScreenSize.Y * 0.5f);


        return Result;
    }
    #endregion
}