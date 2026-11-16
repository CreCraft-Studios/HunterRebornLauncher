using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

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