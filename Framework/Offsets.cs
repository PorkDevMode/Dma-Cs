﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework
{
    internal class Offsets
    {
        #pragma warning disable CS8618

        public static Vmm vmm;
        public static ulong GameAssembly;
        public static uint processPid;

        // Booleans
        public static bool _cacheplayers = true;

        // Static padding for all offsets
        public static ulong Static = 0xB8; // Static padding

        // Admin offsets
        public static ulong adminBase = 0x3B04FE0; // ConVar.Admin_TypeInfo
        public static ulong adminTime = 0x0; // public static float admintime;
        public static ulong adminWaterEffect = 0x11;// public static bool underwater_effect;

        // Convar graphics offsets
        public static ulong graphicsBase = 0x3B04FE0; // ConVar.Graphics_TypeInfo
        public static ulong graphicsFov = 0x18; // private static float _fov;

        // Local / base player offsets
        public static ulong localPlayerBase = 0x3B36E70; // LocalPlayer_TypeInfo
        public static ulong basePlayerBackingField = 0x0;

        public static ulong basePlayerMovement = 0x6C0;
        public static ulong basePlayerModel = 0x598;
        public static ulong basePlayerModelPosition = 0x1C8;
        public static ulong groundAngle = 0xCC; // private float groundAngle;
        public static ulong groundAngleNew = 0xD0; //private float groundAngleNew;
        public static ulong maxAngle = 0x94; //public float maxAngleWalking;

        // Todsky offsets
        public static ulong todSkyBase = 0x3B0C7F8; // TOD_Sky_TypeInfo
        public static ulong NightParameters = 0x60; // public TOD_NightParameters Night;
        public static ulong DayParameters = 0x58; // 	public TOD_DayParameters Day;
        public static ulong AmbientMultiplierDay = 0x50; // TOD_NightParameters -> public float AmbientMultiplier;
        public static ulong AmbientMultiplierNight = 0x50; // TOD_NightParameters -> public float AmbientMultiplier;
        public static ulong LightIntensityDay = 0x48; // TOD_NightParameters -> public float LightIntensity;
        public static ulong LightIntensityNight = 0x48; // TOD_NightParameters -> public float LightIntensity;

        // Playerlist offsets

        public static ulong playerListBase = 0x3B4EB58;
    }
}
