// Copyright (C) 2026 CreCraft Studios
//
// This file is part of Hunter Reborn Launcher.
//
// Hunter Reborn Launcher is free software: you can redistribute it
// and/or modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation, either version 3
// of the License, or (at your option) any later version.
//
// This program is distributed WITHOUT ANY WARRANTY.
// See the LICENSE file for details.

using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CreCraftStudios.HunterReborn.Launcher
{
    public class DisplayModes
    {
        [DllImport("user32.dll", CharSet = CharSet.Ansi)]
        private static extern bool EnumDisplaySettings(string deviceName, int modeNum, ref DEVMODE devMode);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        private struct DEVMODE {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string dmDeviceName;
            public short dmSpecVersion, dmDriverVersion, dmSize, dmDriverExtra;
            public int dmFields, dmPositionX, dmPositionY, dmDisplayOrientation, dmDisplayFixedOutput;
            public short dmColor, dmDuplex, dmYResolution, dmTTOption, dmCollate;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string dmForName;
            public short dmLogPixels;
            public int dmBitsPerPel, dmPelsWidth, dmPelsHeight, dmDisplayFlags, dmDisplayFrequency;
            public int dmICMMethod, dmICMIntent, dmMediaType, dmDitherType;
            public int dmReserved1, dmReserved2, dmPanningWidth, dmPanningHeight;
        }

        public record Resolution(int Width, int Height, int Hz);

        public static List<Resolution> GetSupported() {
            List<Resolution> results = new List<Resolution>();

            var mode = new DEVMODE
            {
                dmSize = (short)Marshal.SizeOf<DEVMODE>()
            };

            for (int i = 0; EnumDisplaySettings(null, i, ref mode); i++) {
                results.Add(new Resolution(
                    mode.dmPelsWidth,
                    mode.dmPelsHeight,
                    mode.dmDisplayFrequency
                    ));
            }

            return results;
        }
    }
}